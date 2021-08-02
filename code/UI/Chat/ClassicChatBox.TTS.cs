using Sandbox;
using System.Collections.Generic;
using System.Linq;

partial class ClassicChatBox
{
	[ClientVar( "tts_enabled" )]
	public static bool TTSEnabled { get; set; } = false;
	[ClientVar( "sam_speed" )]
	public static int SamSpeed { get; set; } = 72;
	[ClientVar( "sam_pitch" )]
	public static int SamPitch { get; set; } = 64;
	[ClientVar( "sam_mouth" )]
	public static int SamMouth { get; set; } = 128;
	[ClientVar( "sam_throat" )]
	public static int SamThroat { get; set; } = 128;
	[ClientVar( "sam_sing" )]
	public static bool SamSing { get; set; } = false;

	[ServerCmd( "tts_say" )]
	public static void TTSSay( int speed = 72, int pitch = 64, int mouth = 128, int throat = 128, bool sing = false, string message = "" )
	{
		SAMSay( To.Everyone, ConsoleSystem.Caller.Pawn.NetworkIdent, message, speed, pitch, mouth, throat, sing );
	}

	public static SoundEvent TTSSound = new SoundEvent();
	[ClientCmd( "sam_say", CanBeCalledFromServer = true )]
	static void SAMSay( int fromident, string message = "", int speed = 72, int pitch = 64, int mouth = 128, int throat = 128, bool sing = false )
	{
		if ( !TTSEnabled ) return;

		var from = Entity.All.FirstOrDefault( e => e.NetworkIdent == fromident );
		if ( from == null ) return;

		SAM.speed = speed;
		SAM.pitch = pitch;
		SAM.mouth = mouth;
		SAM.throat = throat;
		SAM.singmode = sing;

		// HACK!
		message += ".";
		string output = null;
		int[] ints = null;
		bool phonetic = false;
		if ( phonetic )
		{
			ints = SAM.IntArray( message );

			var L = new List<int>( ints );
			L.Add( 155 );
			ints = L.ToArray();

			output = message + "\0x9b";
		}
		else
		{
			output = SAM.TextToPhonemes( message + "[", out ints );
		}

		Log.Info( $"output :{output.Length}" );
		Log.Info( $"ints :{ints.Length}" );
		if ( output.Length > 255 ) return;

		SAM.SetInput( ints );

		var buf = SAM.SAMMain();
		if ( buf == null )
		{
			Log.Error( "Buffer was null" );
		}
		else
		{
			Log.Info( "Buffer size is " + buf.GetSize() );
		}

		var stream = Sound.FromEntity( "ClassicChatBox.TTSSound", from )
			.CreateStream( 22050 );

		stream.WriteData( buf.GetFloats().Select( b => (short)(b * short.MaxValue) ).ToArray() );
		stream.SetVolume( 1 );
	}
}
