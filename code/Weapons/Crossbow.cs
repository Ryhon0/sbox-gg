using Sandbox;

[Library( "xbow", Title = "Crossbow" )]
public class Crossbow : Weapon
{
	public override float Damage => 65;
	public override bool IsAutomatic => false;
	public override int RPM => 60;
	public override float ReloadTime => 2.2f;
	public override float Spread => 0.05f;
	public override bool IsMelee => false;
	public override string ShootShound => "rust_pistol.shoot";
	public override string WorldModelPath => "weapons/rust_crossbow/rust_crossbow.vmdl";
	public override string ViewModelPath => "weapons/rust_crossbow/v_rust_crossbow.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
}
