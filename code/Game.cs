﻿using Sandbox;
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
		"xbow",
		"smg",
		"pistol",
		"pump",
		"pipe",
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

		args.Killed.GetClientOwner()?.SetScore( "deaths", args.Killed.GetClientOwner().GetScore<int>( "deaths", 0 ) + 1 );
	}

	void GivePoint( Client c )
	{
		var rank = c.GetScore<int>( "rank", 0 ) + 1;
		c.SetScore( "rank", rank );

		c.Pawn?.PlaySound( "Game.NextLevel" );

		GiveWeapon( c.Pawn as Player, GetWeapon( rank ) );
	}

	public string GetWeapon( int rank )
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
