using Sandbox;

public class Projectile : ModelEntity
{
	public virtual float Damage { get; set; } = 100;
	public virtual string ModelPath => "weapons/rust_crossbow/rust_crossbow_bolt.vmdl";
	public virtual bool DestroyOnWorldImpact => false;
	public virtual bool DestroyOnPlayerImpact => false;
	public virtual bool StickInWalls => true;
	public virtual bool HasGravity => false;
	public Entity Weapon;

	bool Stuck;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( ModelPath );
		MoveType = HasGravity ? MoveType.MOVETYPE_FLYGRAVITY : MoveType.MOVETYPE_FLY;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );
		if ( !IsServer ) return;
		if ( other == Owner ) return;

		if ( other is Player p )
		{
			if ( DestroyOnPlayerImpact ) Explode();
			if ( Owner is Player op ) op.DidDamage( Position, Damage, other.Health.LerpInverse( 100, 0 ) );
		}
		else if ( DestroyOnWorldImpact ) Explode();

		var damageInfo = DamageInfo.FromBullet( Position, Rotation.Forward * 200, Damage )
													.WithAttacker( Owner )
													.WithWeapon( Weapon );
		other.TakeDamage( damageInfo );

		if ( StickInWalls )
		{
			Velocity = default;
			MoveType = MoveType.None;

			var velocity = Rotation.Forward * 50;

			var start = Position;
			var end = start + velocity;

			var tr = Trace.Ray( start, end )
					.UseHitboxes()
					//.HitLayer( CollisionLayer.Water, !InWater )
					.Ignore( Owner )
					.Ignore( this )
					.Size( 1.0f )
					.Run();

			// TODO: Parent to bone so this will stick in the meaty heads
			SetParent( other, tr.Bone );
			Owner = null;

			//
			// Surface impact effect
			//
			tr.Normal = Rotation.Forward * -1;
			tr.Surface.DoBulletImpact( tr );

			Stuck = true;

			// delete self in 60 seconds
			_ = DeleteAsync( 60.0f );
		}
	}

	public void Explode()
	{
		Delete();
	}
}
