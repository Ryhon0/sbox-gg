using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class ClassicChatBox : Panel
{
	public static ClassicChatBox Current;

	public Panel Canvas;
	public Button EmojiButton;
	public Button SendButton;
	public EmojiPicker EmojiPicker;
	public Panel ChatBoxPanel;
	public TTSSettings TTSSettings;
	public TextEntry Input;

	//public List<object> ChatMessages { get; private set; }

	public ClassicChatBox()
	{
		Current = this;

		StyleSheet.Load( "/ui/chat/ClassicChatBox.scss" );

		Canvas = Add.Panel( "classicchat_canvas" );

		ChatBoxPanel = Add.Panel( "chatbox-panel" );

		Input = ChatBoxPanel.Add.TextEntry( "" );
		Input.AddEventListener( "onsubmit", () => Submit() );
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;

		EmojiButton = ChatBoxPanel.Add.Button( "😀", "emojibutton" );
		EmojiButton.AddEventListener( "onclick", () => ToggleEmojis() );
		EmojiPicker = ChatBoxPanel.AddChild<EmojiPicker>();
		EmojiPicker.Chat = this;
		EmojiPicker.Search.AddEventListener( "onblur", () => Input.Focus() );
		EmojiPicker.Search.AcceptsFocus = true;

		var TTSSettingsButton = ChatBoxPanel.Add.Button( "🤖", "ttsbutton" );
		TTSSettingsButton.AddEventListener( "onclick", () => ToggleTTSSettings() );
		TTSSettings = ChatBoxPanel.AddChild<TTSSettings>();

		SendButton = ChatBoxPanel.Add.Button( "➤", "send" );
		SendButton.AddEventListener( "onclick", () => Submit() );

		Sandbox.Hooks.Chat.OnOpenChat += Open;
	}

	public void ToggleTTSSettings()
	{
		TTSSettings.SetClass( "open", !TTSSettings.HasClass( "open" ) );
	}

	public void ToggleEmojis()
	{
		EmojiPicker.SetClass( "open", !EmojiPicker.HasClass( "open" ) );
	}
	public void ShowEmojis()
	{
		EmojiPicker.AddClass( "open" );
		EmojiPicker.Search.Focus();
	}
	public void HideEmojis()
	{
		Input.Focus();
		EmojiPicker.RemoveClass( "open" );
	}

	public void InsertEmoji( string e )
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

		foreach ( Panel message in Canvas.Children )
		{
			if ( message is ClassicChatEntry c )
			{
				if ( c.HasClass( "hide" ) )
				{
					c.AddClass( "show" );
				}
			}
		}
	}

	void Close()
	{
		RemoveClass( "open" );
		Input.Blur();

		foreach ( Panel message in Canvas.Children )
		{
			if ( message is ClassicChatEntry c )
				if ( c.HasClass( "show" ) )
				{
					c.RemoveClass( "show" );
					c.AddClass( "expired" );
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
		TTSSay( SamSpeed, SamPitch, SamMouth, SamThroat, SamSing, msg );
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
