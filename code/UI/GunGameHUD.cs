using Sandbox;
using Sandbox.UI;

public partial class GunGameHUD : Panel
{
	public static GunGameHUD Current;

	WeaponIcon CurrentWeapon;
	WeaponIcon NextWeapon;

	public GunGameHUD()
	{
		Current = this;
		StyleSheet.Load( "/ui/GunGame.scss" );

		CurrentWeapon = new WeaponIcon();
		NextWeapon = new WeaponIcon();

		NextWeapon.Parent = CurrentWeapon.Parent = this;
		CurrentWeapon.AddClass( "current" );
		NextWeapon.AddClass( "next" );


		CurrentWeapon.Style.Set( "background-image", $"url(/ui/weapons/pistol.png)" );
		NextWeapon.Style.Set( "background-image", $"url(/ui/weapons/smg.png)" );

	}

	public void UpdateWeapons( Client c )
	{
		if ( c == null ) return;

		var score = c.GetScore<int>( "rank", 0 ) + 1;
		var game = (Game.Current as Game);

		var current = game.GetWeapon( score );
		var next = game.GetWeapon( score + 1 );

		CurrentWeapon.Style.Set( "background-image", $"url(/ui/weapons/{current}.png)" );
		NextWeapon.Style.Set( "background-image", $"url(/ui/weapons/{next}.png)" );
	}
}

public class WeaponIcon : Panel
{
	public WeaponIcon()
	{
		AddClass( "weapon-icon" );
	}
}
