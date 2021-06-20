using Sandbox;

[Library( "gl", Title = "Grenade Launcher" )]
class GrenadeLauncher : Weapon
{
	public override string Projectile => "pipe_grenade";
	public override float ProjectileSpeed => 2000;
	public override int RPM => 40;
	public override string ShootShound => "M16.GrenadeThonk";
	public override float Damage => 120;
	public override float Force => 200;
	public override bool IsAutomatic => false;
	public override float ReloadTime => 0.7f;
	public override float Spread => 0.05f;
	public override int ClipSize => 4;
	public override string WorldModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override string Brass => null;
	public override bool ReloadMagazine => false;
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override HoldType HoldType => HoldType.Shotgun;

	protected override void ShootEffects()
	{

		if ( MuzzleFlash != null )
			Particles.Create( MuzzleFlash, EffectEntity, "muzzle" );
		if ( Brass != null )
			Particles.Create( Brass, EffectEntity, "ejection_point" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool( "fire_double", true );
		CrosshairPanel?.OnEvent( "fire" );
	}
}

[Library( "pipe_grenade" )]
class PipeGrenade : Projectile
{
	public override string ModelPath => "weapons/m16/grenade.vmdl";
	public override bool DestroyOnPlayerImpact => true;
	public override bool DestroyOnWorldImpact => false;
	public override bool Explosive => true;
	public override float ExplosionRadius => 75;
	public override float MinimumDamageRadius => ExplosionRadius;
	public override bool StickInWalls => false;

	public override async void Spawn()
	{
		base.Spawn();

		await Task.DelaySeconds( 2 );
		Explode();
	}
}
