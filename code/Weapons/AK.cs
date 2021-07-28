using Sandbox;
[Library( "ak" )]
public class AK : Weapon
{
	public override string ViewModelPath => "weapons/ak/v_ak.vmdl";
	public override string WorldModelPath => "weapons/ak/ak.vmdl";
	public override int ClipSize => 30;
	public override int RPM => 650;
	public override float Damage => 24;
	public override HoldType HoldType => HoldType.SMG;
}
