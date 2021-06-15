using Sandbox;

[Library( "explosive_barrel_projectile" )]
public class ExplosiveBarrelProjectile : Projectile
{
	public override string ModelPath => "models/rust_props/barrels/fuel_barrel.vmdl";
	public override bool OverridePhysics => false;
}
