using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

partial class GunGameHUD
{
	public Panel LastWeaponWarningPanel { get; set; }

	public void ShowLastWeaponWarning( Player p )
	{
		var w = new LastWeaponWarning( p, LastWeaponWarningPanel );
		w.Parent = LastWeaponWarningPanel;
	}
}

public class LastWeaponWarning : Panel
{
	Image LastWeaponAvatar;
	Label LastWeaponText;

	public LastWeaponWarning( Player p, Panel e )
	{
		StyleSheet.Load( "/ui/GunGame/GunGameHUD.scss" );

		AddClass( "last-weapon-warning" );
		LastWeaponAvatar = Add.Image();
		LastWeaponText = Add.Label();

		Parent = e;
		var client = p.GetClientOwner();
		LastWeaponAvatar.SetTexture( $"avatar:{client.SteamId}" );
		LastWeaponText.Text = $"{client.Name} has reached the final level!";

		_ = Lifetime();
	}

	async Task Lifetime()
	{
		await Task.Delay( 1 );
		AddClass( "start" );
		await Task.Delay( 3000 );
		AddClass( "end" );
		await Task.Delay( 500 );

		Delete();
	}
}
