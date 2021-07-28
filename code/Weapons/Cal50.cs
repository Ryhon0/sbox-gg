using Sandbox;

[Library( "50cal" )]
public partial class Cal50 : Fix
{
	public static SoundEvent Attack = new SoundEvent( "weapons/50cal/50cal.vsnd" );
	public override string ShootShound => "Cal50.Attack";
	public override string WorldModelPath => "weapons/50cal/50cal.vmdl";
	public override string ViewModelPath => "weapons/50cal/v_50cal.vmdl";
	public override HoldType HoldType => HoldType.SMG;
	public override float Damage => 100;
	public override DamageFlags DamageFlags => DamageFlags.Blast;
	public override int ClipSize => 10;
	public override float AttackInterval => 0.1f;
	public override float ReloadTime => 3.5f;
	public override float Spread => ZoomedIn ? 0 : 1;

	public override void AttackPrimary()
	{
		if ( AmmoClip > 0 || AmmoClip == -1 ) Owner.Velocity += -Owner.EyeRot.Forward * 200;
		base.AttackPrimary();
	}
}
