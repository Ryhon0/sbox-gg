using Sandbox;

[Library( "pipe", Title = "Pistol" )]
public class PipeShotgun : Weapon
{
	public override float Damage => 100;
	public override bool IsAutomatic => false;
	public override int ClipSize => 1;
	public override int RPM => 300;
	public override float ReloadTime => 4f;
	public override float Spread => 0f;
	public override string ShootShound => "rust_shotgun.shoot";
	public override string WorldModelPath => "weapons/rust_shotgun/rust_shotgun.vmdl";
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override int HoldType => 4;
}
