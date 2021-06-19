using Sandbox;

partial class Weapon
{
	public override bool CanPrimaryAttack()
	{
		if ( Owner == null || Owner.Health <= 0 ) return false;

		if ( !IsMelee )
			if ( ReloadMagazine )
				if ( IsReloading ) return false;

		var chambered = TimeSincePrimaryAttack > AttackInterval;
		var shooting = IsAutomatic ?
			Input.Down( InputButton.Attack1 ) :
			Input.Pressed( InputButton.Attack1 );

		return chambered && shooting;
	}

	public override bool CanSecondaryAttack()
	{
		if ( Owner == null || Owner.Health <= 0 ) return false;

		return base.CanSecondaryAttack();
	}

	public override async void AttackPrimary()
	{
		if ( AmmoClip <= 0 )
		{
			DryFire();
			return;
		}

		var ShotsRemaining = ShotsPerTriggerPull;

		while ( ShotsRemaining > 0 && AmmoClip > 0 )
		{
			if ( Owner == null ) return;
			ShotsRemaining--;

			if ( IsMelee )
			{
				ShootEffects();
				PlaySound( ShootShound );
				ShootBullet( 0, Force, Damage, 10f, 1 );
			}
			else
			{

				using ( Prediction.Off() )
				{
					if ( TakeAmmo( 1 ) )
					{
						IsReloading = false;

						ShootEffects();
						PlaySound( ShootShound );

						if ( Projectile != null ) ShootProjectile( Projectile, Spread, ProjectileSpeed, Force, Damage, BulletsPerShot );
						else ShootBullet( Spread, Force, Damage, BulletSize, BulletsPerShot );
					}
				}
			}

			if ( ShotsRemaining > 0 && AmmoClip > 0 ) await Task.DelaySeconds( BurstInterval );

		}
	}

	public void ShootProjectile( string projectile, float spread, float projectilespeed, float force, float damage, int count = 1 )
	{
		if ( !IsServer ) return;
		for ( int i = 0; i < BulletsPerShot; i++ )
		{
			if ( Owner == null ) continue;

			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			var p = Create( projectile );
			p.Owner = Owner;

			p.Position = GetProjectilePosition();
			p.Rotation = Owner.EyeRot;

			var vel = forward * projectilespeed;
			p.Velocity = vel;

			if ( p is Projectile pp )
			{
				pp.Damage = damage;
				pp.Force = force;
				pp.Weapon = this;
			}
		}
	}

	Vector3 GetProjectilePosition( float MaxDistance = 30 )
	{
		var start = Owner.EyePos;
		var end = start + Owner.EyeRot.Forward * MaxDistance;
		var tr = Trace.Ray( start, end )
					.UseHitboxes()
					.HitLayer( CollisionLayer.Water, false )
					.Ignore( Owner )
					.Ignore( this )
					.Size( 1.0f )
					.Run();
		TraceBullet( start, end );
		return tr.Hit ? tr.EndPos : end;
	}

	public void ShootBullet( float spread, float force, float damage, float bulletSize, int count = 1 )
	{
		//
		// ShootBullet is coded in a way where we can have bullets pass through shit
		// or bounce off shit, in which case it'll return multiple results
		//
		for ( int i = 0; i < BulletsPerShot; i++ )
		{
			if ( Owner == null ) continue;
			var forward = Owner.EyeRot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * Range, bulletSize ) )
			{
				if ( tr.Hit ) tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				//
				// We turn predictiuon off for this, so any exploding effects don't get culled etc
				//
				using ( Prediction.Off() )
				{
					var damageInfo = DamageInfo.FromBullet( tr.EndPos, forward * 100 * force, damage / count )
						.UsingTraceResult( tr )
						.WithAttacker( Owner )
						.WithWeapon( this );

					tr.Entity.TakeDamage( damageInfo );
				}
			}
		}
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		if ( MuzzleFlash != null )
			Particles.Create( MuzzleFlash, EffectEntity, "muzzle" );
		if ( Brass != null )
			Particles.Create( Brass, EffectEntity, "ejection_point" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.OnEvent( "fire" );
	}
	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}
	public virtual void DryFire()
	{
		PlaySound( "pistol.dryfire" );
		if ( !IsReloading ) Reload();
	}
}
