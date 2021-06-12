using Sandbox;

partial class Weapon
{
	public override bool CanPrimaryAttack()
	{
		if ( !IsMelee )
			if ( ReloadMagazine )
				if ( IsReloading ) return false;

		var chambered = TimeSincePrimaryAttack > 60f / RPM;
		var shooting = IsAutomatic ?
			Input.Down( InputButton.Attack1 ) :
			Input.Pressed( InputButton.Attack1 );

		return chambered && shooting;
	}
	public override void AttackPrimary()
	{
		if ( IsMelee )
		{
			ShootEffects();
			PlaySound( ShootShound );
			ShootBullet( 0, Force, Damage, 10f, 1 );
			return;
		}
		else
		{
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

			if ( !TakeAmmo( 1 ) )
			{
				DryFire();
				return;
			}

			IsReloading = false;

			ShootEffects();
			PlaySound( ShootShound );

			ShootBullet( Spread, Force, Damage, 3f, BulletsPerShot );
		}
	}
	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize, int count = 1 )
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

			foreach ( var tr in TraceBullet( Owner.EyePos, Owner.EyePos + forward * (IsMelee ? 75 : 5000), bulletSize ) )
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

		if ( !IsMelee )
			Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

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
