using Sandbox;

[Library( "xbow", Title = "Crossbow" )]
public class Crossbow : Weapon
{
	static SoundEvent Attack = new SoundEvent( "weapons/rust_crossbow/sounds/crossbow-attack-1.vsnd" );
	public override float Damage => 100;
	public override int RPM => 250;
	public override float ReloadTime => 3;
	public override float Spread => 0f;
	public override string ShootShound => "Crossbow.Attack";
	public override string WorldModelPath => "weapons/rust_crossbow/rust_crossbow.vmdl";
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";
	public override string Projectile => "xbow_bolt";
	public override string Brass => null;
	public override string MuzzleFlash => null;
	public override float ProjectileSpeed => 5000;
	public override CrosshairType CrosshairType => CrosshairType.Dot;

	[Event.Tick]
	void OnTick()
	{
		ViewModelEntity?.SetAnimBool( "loaded", AmmoClip != 0 );
	}
}

[Library( "xbow_bolt" )]
public class CrossbowBolt : Projectile
{
	// Temporary, projectiles need some more work
	public override bool DestroyOnPlayerImpact => true;
	public override bool DestroyOnWorldImpact => true;

	public override bool StickInWalls => true;

	public override string ModelPath => "models/light_arrow.vmdl";
}
