using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;


public partial class ClassicChatEntry : Panel
{
	public Label NameLabel { get; internal set; }
	public Label Message { get; internal set; }
	public Image Avatar { get; internal set; }

	private RealTimeSince TimeSinceBorn = 0;

	public ClassicChatEntry()
	{
		Avatar = Add.Image();
		NameLabel = Add.Label( "Name", "name" );
		Message = Add.Label( "Message", "message" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( TimeSinceBorn > 3 && !ClassicChatBox.Current.HasClass( "open" ) )
		{
			Hide();
		}
	}

	public void Hide()
	{
		AddClass( "hide" );
	}
}
