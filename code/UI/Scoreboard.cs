
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

		Parent.SortChildren( c => -1 - int.Parse( (c.GetChild( 1 ) as Label).Text ) );
	}
}
