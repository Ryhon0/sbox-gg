using Sandbox.UI;

public partial class GunGameHUD : Panel
{
	public static GunGameHUD Current;

	public Panel CurrentWeapon { get; set; }
	public Panel NextWeapon { get; set; }
	public Label Winner { get; set; }

	public GunGameHUD()
	{
		Current = this;

		SetTemplate( "/ui/GunGameHUD.html" );
		StyleSheet.Load( "/ui/GunGameHUD.scss" );

		UpdateWeapons( 0 );
	}

	public void UpdateWeapons( int score )
	{
		var game = (Game.Current as Game);

		var current = game.GetWeapon( score );
		var next = game.GetWeapon( score + 1 );

		if ( current != null )
		{
			CurrentWeapon.Style.Set( "background-image", $"url(/ui/weapons/{current}.png)" );

			if ( next != null ) NextWeapon.Style.Set( "background-image", $"url(/ui/weapons/{next}.png)" );
			else NextWeapon.Style.Set( "background-image", $"url(/ui/win.png)" );
		}
		else
		{
			CurrentWeapon.Style.Set( "background-image", $"url(/ui/win.png)" );
			NextWeapon.Parent.AddClass( "hidden" );
		}
	}

	public void ShowWinner( Player c )
	{
		Winner.Text = $"🏆 {c.GetClientOwner().Name} wins! 🏆";
	}
}
