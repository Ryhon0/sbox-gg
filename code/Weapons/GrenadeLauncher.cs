using Sandbox;

[Library( "gl" )]
class GrenadeLauncher : Weapon
{
	public override string Projectile => "pipe_grenade_projectile";
	public override float ReloadTime => 0;
	public override int RPM => 100;
	public override string ShootShound => null;
}
