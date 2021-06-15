using Sandbox;

[Library( "pipe_grenade_projectile" )]
public class PipeGrenadeProjectile : Projectile
{
	public override string ModelPath => "models/citizen_props/sodacan01.vmdl";
	public override bool OverridePhysics => true;
	public override bool StickInWalls => true;
}
