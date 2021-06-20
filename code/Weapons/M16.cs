using Sandbox;

[Library( "m16", Title = "M16" )]
public partial class M16 : Weapon
{
	public override float Damage => 20;
	public override bool IsAutomatic => false;
	public override int ShotsPerTriggerPull => 3;
	public override int RPM => 800;
	public override float BurstRPM => 800;
	public override float ReloadTime => 4f;
	public override float Spread => 0.05f;
	public override int ClipSize => 30;
	public override string ShootShound => "rust_smg.shoot";
	public override string WorldModelPath => "weapons/rust_smg/rust_smg.vmdl";
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override HoldType HoldType => HoldType.SMG;

	[Net, Predicted]
	bool HasGrenade { get; set; } = true;
	public override bool CanSecondaryAttack()
	{
		return HasGrenade && base.CanSecondaryAttack();
	}

	public static SoundEvent GrenadeThonk = new SoundEvent( "weapons/m16/grenade_launcher_thonk.vsnd" );
	public override void AttackSecondary()
	{
		if ( IsServer )
			ShootProjectile( "m16grenade", Spread, 750, 200, 120 );
		PlaySound( "M16.GrenadeThonk" );
		ViewModelEntity?.SetAnimBool( "fire", true );
		HasGrenade = false;
	}
}

[Library( "m16grenade" )]
public partial class M16Grenade : Projectile
{
	public override string ModelPath => "weapons/m16/grenade.vmdl";
	public override bool DestroyOnPlayerImpact => Live;
	public override bool DestroyOnWorldImpact => Live;
	public override bool StickInWalls => false;
	public override bool Explosive => Live;

	bool Live => TimeSinceDeployed > 0.1f;

	TimeSince TimeSinceDeployed { get; set; }
	bool HitSurface;

	public override void Spawn()
	{
		base.Spawn();
		TimeSinceDeployed = 0;
	}

	public override void StartTouch( Entity e )
	{
		base.StartTouch( e );
		HitSurface = true;
		if ( IsServer ) DeleteAsync( 10f );
	}

	public override void Explode()
	{
		if ( !HitSurface )
			base.Explode();
	}
}
