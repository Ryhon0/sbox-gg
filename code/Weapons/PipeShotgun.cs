using Sandbox;

[Library( "pipe", Title = "Pisol" )]
public class PipeShotgun : Weapon
{
	static SoundEvent Attack = new SoundEvent( "weapons/rust_shotgun/sounds/rust-shotgun-attack.vsnd" );
	public override float Damage => 99;
	public override bool IsAutomatic => false;
	public override int ClipSize => 1;
	public override int RPM => 40;
	public override float ReloadTime => 4f;
	public override float Spread => 0f;
	public override string ShootShound => "PipeShotgun.Attack";
	public override string WorldModelPath => "weapons/rust_shotgun/rust_shotgun.vmdl";
	public override string ViewModelPath => "weapons/rust_shotgun/v_rust_shotgun.vmdl";
	public override CrosshairType CrosshairType => CrosshairType.Dot;
	public override string Brass => null;
	public override HoldType HoldType => HoldType.Shotgun;
}
