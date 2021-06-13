using Sandbox;

[Library( "xbow", Title = "Crossbow" )]
public class Crossbow : Weapon
{
	public override float Damage => 65;
	public override bool IsAutomatic => true;
	public override int RPM => 600;
	public override float ReloadTime => 0;
	public override float Spread => .1f;
	public override string ShootShound => "rust_crossbow.shoot";
	public override int BulletsPerShot => 10;
	public override string WorldModelPath => "weapons/rust_crossbow/rust_crossbow.vmdl";
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";
	public override string Projectile => "xbow_bolt";
	public override float ProjectileSpeed => 5000;
	public override CrosshairType CrosshairType => CrosshairType.Dot;
}
