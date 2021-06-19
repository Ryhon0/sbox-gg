using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

public partial class ClassicChatBox : Panel
{
	public static ClassicChatBox Current;

	public Panel Canvas { get; protected set; }
	public Button EmojiButton { get; set; }
	public Button SendButton { get; set; }
	public Panel EmojiCanvas { get; protected set; }
	public Panel ChatBoxPanel;
	public TextEntry Input { get; protected set; }

	//public List<object> ChatMessages { get; private set; }

	public ClassicChatBox()
	{
		Current = this;

		StyleSheet.Load( "/ui/chat/ClassicChatBox.scss" );

		Canvas = Add.Panel( "classicchat_canvas" );

		ChatBoxPanel = Add.Panel( "chatbox-panel" );

		Input = ChatBoxPanel.Add.TextEntry( "" );
		Input.AddEvent( "onsubmit", () => Submit() );
		Input.AddEvent( "onblur", () => Close() );
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;

		EmojiButton = ChatBoxPanel.Add.Button( "😀", "emoji" );
		EmojiButton.AddEvent( "onclick", () => ToggleEmojis() );
		EmojiCanvas = EmojiButton.Add.Panel( "emoji-canvas" );

		SendButton = ChatBoxPanel.Add.Button( "➤", "send" );
		SendButton.AddEvent( "onclick", () => Submit() );

		// Only 100 emojis for now, showing all of them lags the game
		foreach ( var e in EmojiList.Entries.Values.Take( 100 ) )
		{
			var eb = EmojiCanvas.Add.Button( e, "emoji" );
			eb.AddEvent( "onclick", () => InsertEmoji( e ) );
		}

		Sandbox.Hooks.Chat.OnOpenChat += Open;
	}


	void ToggleEmojis()
	{
		EmojiCanvas.SetClass( "open", !EmojiCanvas.HasClass( "open" ) );
	}
	void ShowEmojis()
	{
		EmojiCanvas.AddClass( "open" );
	}
	void HideEmojis()
	{
		EmojiCanvas.RemoveClass( "open" );
	}

	void InsertEmoji( string e )
	{
		var shift = Sandbox.Input.Down( InputButton.Run );
		if ( shift ) ShowEmojis();

		foreach ( var c in e )
			Input.OnKeyTyped( c );
	}

	void Open()
	{
		AddClass( "open" );
		Input.Focus();

		foreach ( ClassicChatEntry message in Canvas.Children )
		{
			if ( message.HasClass( "hide" ) )
			{
				message.AddClass( "show" );
			}
		}
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();

		foreach ( ClassicChatEntry message in Canvas.Children )
		{
			if ( message.HasClass( "show" ) )
			{
				message.RemoveClass( "show" );
				message.AddClass( "expired" );
			}
		}
	}

	void Submit()
	{
		HideEmojis();
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );
	}


	public void AddEntry( string name, string message, string avatar )
	{
		var e = Canvas.AddChild<ClassicChatEntry>();
		//e.SetFirstSibling();
		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );

		// Add to array of messages to showcase later
		//ChatMessages.Add( e );
	}

	//
	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null )
	{
		Current?.AddEntry( name, message, avatar );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		Current?.AddEntry( null, message, avatar );
	}

	[ServerCmd( "say" )]
	public static void Say( string message )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Info( $"{ConsoleSystem.Caller}: {message}" );
		AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}" );
	}

}


/*
public static partial class ClassicChat
{
    public static event Action OnOpenChat;

    [ClientCmd( "openchatt" )]
    internal static void MessageMode()
    {
        OnOpenChat?.Invoke();
    }

}

*/
