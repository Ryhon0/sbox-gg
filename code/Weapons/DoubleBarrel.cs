using Sandbox;

[Library( "db", Title = "Double  Barrel Shotgun" )]
public class DoubleBarrelShotgun : Weapon
{
	static SoundEvent Attack = new SoundEvent( "weapons/rust_shotgun/sounds/rust-shotgun-attack.vsnd" );
	public override float Damage => 100;
	public override bool IsAutomatic => false;
	public override int ClipSize => 2;
	public override float AttackInterval => 0;
	public override float ReloadTime => 2f;
	public override float Spread => .3f;
	public override int BulletsPerShot => 10;
	public override string ShootShound => "DoubleBarrelShotgun.Attack";
	public override string WorldModelPath => "weapons/rust_shotgun/rust_shotgun.vmdl";
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Sides;
	public override HoldType HoldType => HoldType.Shotgun;

	public override void AttackSecondary()
	{
		AttackPrimary();
		AttackPrimary();
	}
}
