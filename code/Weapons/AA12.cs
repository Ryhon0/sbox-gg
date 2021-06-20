using Sandbox;

[Library( "aa12", Title = "AA12" )]
public class AA12 : Weapon
{
	public override float Damage => 60;
	public override bool IsAutomatic => true;
	public override int BulletsPerShot => 8;
	public override int RPM => 400;
	public override float ReloadTime => 4f;
	public override float Spread => 0.2f;
	public override int ClipSize => 10;
	public override string ShootShound => "PipeShotgun.Attack";
	public override string WorldModelPath => "weapons/rust_smg/rust_smg.vmdl";
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Sides;
	public override HoldType HoldType => HoldType.SMG;
}
