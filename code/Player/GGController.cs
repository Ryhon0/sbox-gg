using Sandbox;
class GGController : WalkController
{
	const float DefaultSprintSpeed = 320.0f;
	const float DefaultWalkSpeed = 150.0f;
	const float DefaultDefaultSpeed = 190.0f;

	float GetSpeed( bool canSprint = true )
	{
		var ws = Duck.GetWishSpeed();
		if ( ws >= 0 ) return ws;

		if ( canSprint )
		{
			if ( Input.Down( InputButton.Run ) ) return SprintSpeed;
			if ( Input.Down( InputButton.Walk ) ) return WalkSpeed;
		}

		return DefaultSpeed;
	}

	public override float GetWishSpeed()
	{
		if ( Pawn is Player p )
		{
			if ( p.ActiveChild is Weapon w )
			{
				if ( w is IAimableWeapon aw )
					return aw.IsAimed ? GetSpeed( false ) * 0.4f : GetSpeed();

				if ( w.IsMelee ) return GetSpeed() * 1.2f;
			}

			return GetSpeed();
		}

		return base.GetWishSpeed();
	}
}
