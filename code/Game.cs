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

	public override void Spawn()
	{
		base.Spawn();
		StartRound();
		Weapons = new List<string>()
		{
			"vec45",
			"smg",
			"pistol",
			"pump",
			"pipe",
			"aa12",
			"knife",
		};
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );
		var player = new Player();
		client.Pawn = player;
		client.SetScore( "rank", 0 );
		player.Respawn();
		UpdateWeapons( To.Single( client ), 0 );
	}

	[Net, Predicted]
	List<string> Weapons { get; set; }

	TimeSince TimeSinceRoundFinish;
	float VictoryScreenLength = 5f;
	bool GameFinished;

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
					GameFinished = true;
					TimeSinceRoundFinish = 0;

				}
			}

		}

		args.Killed.GetClientOwner()?.SetScore( "deaths", args.Killed.GetClientOwner().GetScore<int>( "deaths", 0 ) + 1 );
	}

	[Event.Tick]
	void Tick()
	{
		if ( GameFinished )
		{
			if ( TimeSinceRoundFinish > VictoryScreenLength )
				StartRound();
		}
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
		var i = rank;
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

	void StartRound()
	{
		foreach ( var c in Client.All )
		{
			c.SetScore( "rank", 0 );
			c.SetScore( "deaths", 0 );
			(c.Pawn as Player).Respawn();
		}

		GameFinished = false;
		UpdateWeapons( To.Everyone, 0 );
		ShowWinner( To.Everyone, null );
	}

	}

	public void RequestWeapon( Player p )
	{
		GiveWeapon( p, GetWeapon( p.GetClientOwner().GetScore<int>( "rank", 0 ) ) );
	}
}
