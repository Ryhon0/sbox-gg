using Sandbox;

[Library( "pistol", Title = "Pistol" )]
public class Pistol : Weapon
{
	public override float Damage => 40;
	public override bool IsAutomatic => false;
	public override int RPM => 600;
	public override float ReloadTime => 2.2f;
	public override float Spread => 0.05f;
	public override string ShootShound => "rust_pistol.shoot";
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Cross;
	public override int ClipSize => 15;
}
