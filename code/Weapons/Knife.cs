using Sandbox;

[Library( "knife", Title = "Knife" )]
public class Knife : Weapon
{
	static SoundEvent Attack = new SoundEvent( "weapons/rust_boneknife/sounds/rust-knife-attack.vsnd" );

	public override float Damage => 65;
	public override bool IsAutomatic => true;
	public override int RPM => 150;
	public override float ReloadTime => 2.2f;
	public override float Spread => 0.05f;
	public override bool IsMelee => true;
	public override string ShootShound => "Knife.Attack";
	public override string WorldModelPath => null;
	public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
	public override string Brass => null;
	public override string MuzzleFlash => null;
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override HoldType HoldType => HoldType.Universal;

	public override void SimulateAnimator(PawnAnimator anim)
	{
		base.SimulateAnimator(anim);
		anim.SetParam("holdtype_attack", 2f);
	}
}
