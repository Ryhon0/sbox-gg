using Sandbox;
using System.Collections.Generic;

[Library( "gg" )]
public partial class Game : Sandbox.Game
{
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
		"smg",
		"pistol",
		"pump",
		"pipe",
		"xbow",
		"knife",
	};

	[Event( "player_killed" )]
	void PlayerKilled( KillArgs args )
	{
		if ( args.Killer is Player p )
		{
			var owner = p.GetClientOwner();

			GivePoint( owner );
		}
	}

	void GivePoint( Client c )
	{
		var rank = c.GetScore<int>( "rank", 0 ) + 1;
		c.SetScore( "rank", rank );

		GiveWeapon( c.Pawn as Player, GetWeapon( rank ) );
	}

	string GetWeapon( int rank )
	{
		var i = rank.Wrap( 0, Weapons.Count );
		return Weapons[i];
	}

	void GiveWeapon( Player p, string weapon )
	{
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
