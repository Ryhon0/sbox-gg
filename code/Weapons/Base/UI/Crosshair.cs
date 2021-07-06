using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Crosshair : Panel
{
	public static bool UseReloadTimer = false;

	public int fireCounter;
	Label ReloadTimer;
	public Crosshair()
	{
		StyleSheet.Load( "/Weapons/Base/UI/Crosshair.scss" );

		for ( int i = 0; i < 5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}

		ReloadTimer = Add.Label( "", "reloadtimer" );
	}

	public override void Tick()
	{
		base.Tick();
		this.PositionAtCrosshair();

		if ( UseReloadTimer && Local.Pawn != null && Local.Pawn.ActiveChild is Weapon w
			&& w.IsReloading && (w.ReloadTime - w.TimeSinceReload) > 0 )
			ReloadTimer.Text = (w.ReloadTime - w.TimeSinceReload).ToString( "0.0s" );
		else ReloadTimer.Text = "";

		SetClass( "fire", fireCounter > 0 );

		if ( fireCounter > 0 )
			fireCounter--;
	}
}
