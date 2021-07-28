using Sandbox;

[Library( "vec45", Title = "Vector45" )]
public class Vector45 : Weapon
{
	public override float Damage => 16;
	public override bool IsAutomatic => true;
	public override int RPM => 1200;
	public override float ReloadTime => 3f;
	public override float Spread => 0.1f;
	public override int ClipSize => 20;
	public override string ShootShound => "rust_smg.shoot";
	public override string WorldModelPath => "weapons/vec45/vec45.vmdl";
	public override string ViewModelPath => "weapons/vec45/v_vec45.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override HoldType HoldType => HoldType.SMG;
}
