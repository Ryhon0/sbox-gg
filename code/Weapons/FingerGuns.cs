using Sandbox;

[Library( "finger_gun", Title = "Finger Gun" )]
public class FingerGun : Weapon
{
	public static SoundEvent Attack = new SoundEvent( "weapons/finger_guns/pew.vsnd" );
	public override float Damage => 55;
	public override bool IsAutomatic => false;
	public override int RPM => 300;
	public override float ReloadTime => 1f;
	public override float Spread => 0f;
	public override string ShootShound => "FingerGun.Attack";
	public override string ViewModelPath => "weapons/finger_guns/finger_guns.vmdl";
	public override string WorldModelPath => null;
	public override string Brass => null;
	public override string MuzzleFlash => null;
	public override HoldType HoldType => HoldType.Pistol;
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override int ClipSize => 5;
}
