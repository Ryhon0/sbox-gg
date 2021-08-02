using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class TTSSettings : Panel
{
	public TTSSettings()
	{
		StyleSheet.Load( "UI/Chat/TTSSettings/TTSSettings.scss" );

		var enable = new ConvarToggleButton( this, "tts_enabled", "1", "0" );
		enable.Text = "Enable TTS";

		Add.Label( "Speed" );
		var speed = new ConvarSlider( "sam_speed", 0, 255 );
		AddChild( speed );

		Add.Label( "Pitch" );
		var pitch = new ConvarSlider( "sam_pitch", 0, 255 );
		AddChild( pitch );

		Add.Label( "Throat" );
		var throat = new ConvarSlider( "sam_throat", 0, 255 );
		AddChild( throat );

		Add.Label( "Mouth" );
		var mouth = new ConvarSlider( "sam_mouth", 0, 255 );
		AddChild( mouth );

		var sing = new ConvarToggleButton( this, "sam_sing", "1", "0" );
		sing.Text = "Sing Mode";

		var reset = Add.Label( "Reset", "resetbutton" );
		reset.AddEventListener( "OnClick", () =>
		 {
			 ConsoleSystem.Run( "sam_speed", 72 );
			 ConsoleSystem.Run( "sam_pitch", 64 );
			 ConsoleSystem.Run( "sam_mouth", 128 );
			 ConsoleSystem.Run( "sam_throat", 128 );
			 ConsoleSystem.Run( "sam_sing", 0 );
		 } );
	}
}
