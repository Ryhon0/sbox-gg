using Sandbox;

public class Projectile : BasePhysics
{
	public static bool DebugDrawRadius = false;

	public virtual float Damage { get; set; } = 100;
	public virtual float Force { get; set; } = 100;
	public virtual string ModelPath => "models/light_arrow.vmdl";
	public virtual bool DestroyOnWorldImpact => false;
	public virtual bool DestroyOnPlayerImpact => false;
	public virtual bool StickInWalls => false;
	public virtual bool Explosive => false;
	public virtual float ExplosionRadius => 100;
	public virtual float MinimumDamageRadius => ExplosionRadius / 2;
	public virtual DamageFlags DamageFlags { get; set; }

	public Weapon Weapon;

	bool Stuck;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( ModelPath );

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
	}

	protected override ModelPropData GetModelPropData()
	{
		var d = base.GetModelPropData();
		d.ImpactDamage = 0f;
		d.MinImpactDamageSpeed = float.MaxValue;
		return d;
	}

	public override void StartTouch( Entity e )
	{
		base.StartTouch( e );
		if ( !IsServer ) return;
		if ( e == Owner ) return;

		if ( e is WaterFunc ) return;

		if ( e is Player p )
		{
			if ( DestroyOnPlayerImpact ) ExplodeOrDestroy();
		}
		else
		{
			if ( DestroyOnWorldImpact ) ExplodeOrDestroy();
		}


		if ( !Explosive )
		{
			var me = e as ModelEntity;
			var pos = me?.PhysicsBody?.MassCenter ?? e.Position;

			var damageInfo = DamageInfo.FromBullet( Position, (pos - Position).Normal * Force, Damage )
					.WithAttacker( Owner )
					.WithWeapon( Weapon )
					.WithFlag( DamageFlags );
			e.TakeDamage( damageInfo );
		}


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
			SetParent( e, tr.Bone );
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

	void ExplodeOrDestroy()
	{
		if ( Explosive ) Explode();
		else Delete();
	}

	public virtual void Explode()
	{
		if ( !this.IsValid() ) return;

		if ( DebugDrawRadius )
		{
			DebugOverlay.Sphere( Position, ExplosionRadius, Color.Yellow, true, 1 );
			DebugOverlay.Sphere( Position, MinimumDamageRadius, Color.Red, true, 1 );
		}

		PlaySound( "rust_pumpshotgun.shootdouble" );
		Particles explosion = Particles.Create( "particles/explosion.vpcf", Position );

		foreach ( var e in Physics.GetEntitiesInSphere( Position, ExplosionRadius ) )
		{
			var me = e as ModelEntity;
			// TODO shoot rays to determine if enemy is behind a wall and if damage should be dealt
			var pos = me?.PhysicsBody?.MassCenter ?? e.Position;
			var distance = Vector3.DistanceBetween( Position, pos );

			// Ignore if target is too far away
			if ( distance > ExplosionRadius ) continue;

			// Deal full damage within minimum damage radius
			var damage = Damage;
			// Deal half or more damage outside minimu damage range
			if ( distance > MinimumDamageRadius )
			{
				var maxrange = ExplosionRadius - MinimumDamageRadius;
				var maxdist = ExplosionRadius - distance;
				damage *= maxdist / maxrange / 2 + .5f;
			}

			var dmg = DamageInfo.Explosion( pos, Force, damage )
				.WithWeapon( Weapon )
				.WithAttacker( Owner )
				.WithForce( (pos - Position).Normal * Force * distance / ExplosionRadius );
			e.TakeDamage( dmg );
		}

		Delete();
	}
}
