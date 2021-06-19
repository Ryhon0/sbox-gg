using Sandbox;
class GGController : WalkController
{
	const float DefaultSprintSpeed = 320.0f;
	const float DefaultWalkSpeed = 150.0f;
	const float DefaultDefaultSpeed = 190.0f;

	public override float GetWishSpeed()
	{
		return
			this.Client != null && this.Client.Pawn is Sandbox.Player p && p.ActiveChild is Weapon w && w.IsMelee
			? base.GetWishSpeed() * 1.2f :
			base.GetWishSpeed();
	}
}
