using Sandbox;

[Library( "pump", Title = "Pump Shotgun" )]
public class PumpShotgun : Weapon
{
	public override float Damage => 120;
	public override bool IsAutomatic => false;
	public override int RPM => 100;
	public override float ReloadTime => 0.75f;
	public override int BulletsPerShot => 8;
	public override float Spread => 0.2f;
	public override int ClipSize => 6;
	public override string ShootShound => "rust_pumpshotgun.shoot";
	public override string WorldModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";
	public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
	public override string Brass => null;
	public override bool ReloadMagazine => false;
	public override CrosshairType CrosshairType => CrosshairType.Sides;
	public override HoldType HoldType => HoldType.Shotgun;
}
