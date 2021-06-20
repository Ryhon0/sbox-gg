using Sandbox;
using System.Collections.Generic;
using System.Linq;

[Library( "gg" )]
public partial class Game : Sandbox.Game
{
	static SoundEvent NextLevel = new SoundEvent( "sounds/electrical/powerup.vsnd", 1 );

	public Game()
	{
		Crosshair.UseReloadTimer = true;
		Projectile.DebugDrawRadius = true;
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
			"pistol",
			"smg",
			"vec45",
			"m16",
			"aa12",
			"pump",
			"gl",
			"db",
			"xbow",
			"pipe",
			"knife",
		};
	}

	public override void DoPlayerNoclip( Client player )
	{
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
	public List<string> Weapons { get; set; }

	TimeSince TimeSinceRoundFinish;
	float VictoryScreenLength = 5f;
	bool GameFinished;

	[Event( "player_killed" )]
	void PlayerKilled( KillArgs args )
	{
		if ( args.Info.Attacker is Player p )
		{
			// Check for double skip when using shotguns
			if ( args.Info.Weapon.ClassInfo.Name == GetWeapon( args.Info.Attacker.GetClientOwner().GetScore<int>( "rank" ) ) &&
				args.Info.Attacker != args.Killed )
			{
				var owner = p.GetClientOwner();

				var score = GivePoint( owner );

				using ( Prediction.Off() )
				{
					var c = args.Info.Attacker.GetClientOwner();
					UpdateWeapons( To.Single( owner ), score );
				}

				if ( score == Weapons.Count )
				{
					using ( Prediction.Off() )
						ShowWinner( To.Everyone, p );

					GameFinished = true;
					TimeSinceRoundFinish = 0;
				}
				else if ( score == Weapons.Count - 1 )
				{
					using ( Prediction.Off() )
						ShowLastWeaponWarning( To.Everyone, p );
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

		LoadWeaponRotation();
		GameFinished = false;
		UpdateWeapons( To.Everyone, 0 );
		ShowWinner( To.Everyone, null );
	}

	void LoadWeaponRotation()
	{
		// This is so borken
		/*
        if ( !IsServer ) return;
        if ( FileSystem.Data.DirectoryExists( "rotation" ) )
        {
            // Why is this not working AAAAA
            //var files = FileSystem.Data.FindFile( "rotation", "*\\.gg$" );
            if ( FileSystem.Data.FileExists( "rotation/default.gg" ) ) //files.Any() )
            {
                var file = "rotation/default.gg"; //"rotation/" + files.GetRandom();
                var rot = FileSystem.Data.ReadAllText( file );
                Weapons = rot.Split( ';' ).ToList();
                if ( Weapons.Count < 2 )
                {
                    Log.Warning( $"File `{file}` doesn't have enough weapons, using default" );
                }
                else
                {
                    using ( Prediction.Off() )
                        UpdateWeaponList( To.Everyone, rot );

                    Log.Info( $"Loaded rotation from {file}" );
                }
            }
            else
            {
                Log.Info( "No rotation file found, using default" );
            }
        }
        else
        {
            FileSystem.Data.CreateDirectory( "rotation" );
            FileSystem.Data.WriteAllText( "rotation/example_rotation.gg.example", string.Join( ';', Weapons ) );
        }
        */
	}

	[ClientRpc]
	void UpdateWeaponList( string weapons )
	{
		(Game.Current as Game).Weapons = new List<string>();
		(Game.Current as Game).Weapons = weapons.Split( ';' ).ToList();
	}

	public void RequestWeapon( Player p )
	{
		GiveWeapon( p, GetWeapon( p.GetClientOwner().GetScore<int>( "rank", 0 ) ) );
	}
}
