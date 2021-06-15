using Sandbox;

public partial class Projectile : BasePhysics
{
	[Net]
	public float Damage { get; set; } = 60;
	public virtual string ModelPath => "models/light_arrow.vmdl";
	public virtual bool DestroyOnWorldImpact => false;
	public virtual bool DestroyOnPlayerImpact => false;
	public virtual bool StickInWalls => true;
	public virtual bool HasGravity => false;
	public virtual bool OverridePhysics => true;

	public Weapon Weapon;

	bool Stuck;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( ModelPath );
	}

	[Event( "tick" )]
	void Tick()
	{
		Velocity = 0;
	}

	public override void StartTouch( Entity other )
	{

	}


	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		var other = eventData.Entity;
		if ( Stuck ) return;

		base.StartTouch( other );
		if ( other == Owner ) return;

		if ( other is Player p )
		{
			if ( DestroyOnPlayerImpact ) Explode();
		}
		else if ( DestroyOnWorldImpact ) Explode();

		if ( StickInWalls )
		{
			Stuck = true;
			EnableAllCollisions = false;
			var velocity = Rotation.Forward * 100000;

			var start = eventData.Pos;
			var end = start + velocity;

			var tr = Trace.Ray( start, end )
					.UseHitboxes()
					//.HitLayer( CollisionLayer.Water, !InWater )
					.Ignore( Owner )
					.Ignore( this )
					.Size( 1.0f )
					.Run();

			if ( tr.Hit )
			{
				SetParent( other, tr.Bone );

				var damageInfo = DamageInfo.FromBullet( Position, Rotation.Forward * 200, tr.HitboxIndex == 5 ? Damage * Weapon.HeadshotMultiplier : Damage )
													.WithAttacker( Owner )
													.WithWeapon( Weapon );
				other.TakeDamage( damageInfo );

				Position = tr.EndPos;
			}
			Owner = null;

			//
			// Surface impact effect
			//
			tr.Normal = Rotation.Forward * -1;
			tr.Surface.DoBulletImpact( tr );

			// delete self in 60 seconds
			_ = DeleteAsync( 60.0f );
		}

		if ( OverridePhysics ) return;
		base.OnPhysicsCollision( eventData );
	}

	public void Explode()
	{
		Delete();
	}

}
