using Sandbox;

partial class Game
{
	[ClientRpc]
	void ShowWinner( Player p )
	{
		GunGameHUD.Current.ShowWinner( p );
	}

	[ClientRpc]
	void UpdateWeapons( int score )
	{
		GunGameHUD.Current.UpdateWeapons( score );
	}

}
