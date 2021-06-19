using Sandbox;

[Library( "xbow", Title = "Crossbow" )]
public class Crossbow : Weapon
{
	static SoundEvent Attack = new SoundEvent( "weapons/rust_crossbow/sounds/crossbow-attack-1.vsnd" );
	public override float Damage => 65;
	public override int RPM => 250;
	public override float ReloadTime => 3;
	public override float Spread => .1f;
	public override string ShootShound => "Crossbow.Attack";
	public override string WorldModelPath => "weapons/rust_crossbow/rust_crossbow.vmdl";
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";
	public override string Projectile => "xbow_bolt";
	public override float ProjectileSpeed => 5000;
	public override CrosshairType CrosshairType => CrosshairType.Dot;

	[Event.Tick]
	void OnTick()
	{
		ViewModelEntity?.SetAnimBool( "loaded", AmmoClip != 0 );
	}
}
}
