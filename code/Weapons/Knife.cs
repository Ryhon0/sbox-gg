using Sandbox;

[Library( "knife", Title = "Knife" )]
public class Knife : Weapon
{
	public override float Damage => 65;
	public override bool IsAutomatic => true;
	public override int RPM => 100;
	public override float ReloadTime => 2.2f;
	public override float Spread => 0.05f;
	public override bool IsMelee => true;
	public override string ShootShound => "rust_boneknife.attack";
	public override string WorldModelPath => "weapons/rust_boneknife/rust_boneknife.vmdl";
	public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
}
