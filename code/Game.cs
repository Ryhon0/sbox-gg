
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;

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

		player.Respawn();
	}
}

