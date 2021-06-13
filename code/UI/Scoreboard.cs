
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

	public Scoreboard()
	{
		StyleSheet.Load( "/ui/Scoreboard.scss" );
	}

	protected override void AddHeader()
	{
		Header = Add.Panel( "header" );
		Header.Add.Label( "player", "name" );
		Header.Add.Label( "rank", "rank" );
		Header.Add.Label( "deaths", "deaths" );
		Header.Add.Label( "ping", "ping" );
	}

	public override void Tick()
	{
		base.Tick();

		Canvas.SortChildren( c =>
		{
			var snd = c.GetChild( 1 );
			var l = (snd as Label);
			int.TryParse( l.Text, out int rank );
			return -rank;
		} );
	}

	protected override void AddPlayer( PlayerScore.Entry entry )
	{
		base.AddPlayer( entry );

	}
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
	public Label Rank;

	public ScoreboardEntry()
	{
		Kills.Delete();
		Deaths.Delete();
		Ping.Delete();


		Rank = Add.Label( "", "rank" );
		Deaths = Add.Label( "", "deaths" );
		Ping = Add.Label( "", "ping" );
	}

	public override void UpdateFrom( PlayerScore.Entry entry )
	{
		base.UpdateFrom( entry );

		Rank.Text = entry.Get<int>( "rank", 0 ).ToString();

		SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
	}
}
