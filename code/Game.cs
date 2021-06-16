using Sandbox;
using System.Collections.Generic;

[Library( "gg" )]
public partial class Game : Sandbox.Game
{
	static SoundEvent NextLevel = new SoundEvent( "sounds/electrical/powerup.vsnd", 1 );

	public Game()
	{
		if ( IsServer )
		{
			new MinimalHudEntity();
		}

		if ( IsClient )
		{

		}
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );
		var player = new Player();
		client.Pawn = player;
		client.SetScore( "rank", 0 );
		player.Respawn();
	}

	List<string> Weapons = new List<string>()
	{
		"vec45",
		"smg",
		"pistol",
		"pump",
		"pipe",
		"aa12",
		"xbow",
		"knife",
	};

	[Event( "player_killed" )]
	void PlayerKilled( KillArgs args )
	{
		if ( args.Killer is Player p )
		{
			var owner = p.GetClientOwner();

			var score = GivePoint( owner );

			using ( Prediction.Off() )
			{
				var c = args.Killer.GetClientOwner();
				UpdateWeapons( To.Single( owner ), score );
			}

			if ( score == Weapons.Count )
			{
				using ( Prediction.Off() )
				{
					ShowWinner( p );
				}
			}

		}

		args.Killed.GetClientOwner()?.SetScore( "deaths", args.Killed.GetClientOwner().GetScore<int>( "deaths", 0 ) + 1 );
	}

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

	int GivePoint( Client c )
	{
		var rank = c.GetScore<int>( "rank", 0 ) + 1;
		c.SetScore( "rank", rank );

		c.Pawn?.PlaySound( "Game.NextLevel" );

		GiveWeapon( c.Pawn as Player, GetWeapon( rank ) );
		return rank;
	}

	public string GetWeapon( int rank )
	{
		var i = rank; rank.Wrap( 0, Weapons.Count );
		if ( Weapons.Count > i ) return Weapons[i];
		else return null;
	}

	void GiveWeapon( Player p, string weapon )
	{
		if ( weapon == null || p == null ) return;

		if ( !(p.Inventory as GGInventory).IsCarryingId( weapon ) )
		{
			p.Inventory.DeleteContents();
			var w = Entity.Create( weapon );
			p.Inventory.Add( w, true );
		}
	}

		var d = p.Inventory.DropActive();
		d?.Delete();

		var w = Entity.Create( weapon );
		p.Inventory.Add( w );
		p.ActiveChild = w;
	}

	public void RequestWeapon( Player p )
	{
		GiveWeapon( p, GetWeapon( p.GetClientOwner().GetScore<int>( "rank", 0 ) ) );
	}
}
