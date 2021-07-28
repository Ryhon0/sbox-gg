using Sandbox;

class GGCamera : FirstPersonCamera
{
	Vector3 lastPos;
	float DefaultFOV = 90;
	public override void Activated()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		Pos = pawn.EyePos;
		Rot = pawn.EyeRot;

		lastPos = Pos;
		FieldOfView = DefaultFOV;
	}

	public override void BuildInput( InputBuilder input )
	{
		//
		// If we're using the mouse then
		// increase pitch sensitivity
		//
		if ( input.UsingMouse )
		{
			input.AnalogLook.pitch *= 1.5f;
		}
		input.AnalogLook *= FieldOfView / DefaultFOV;

		// add the view move, clamp pitch
		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
		input.ViewAngles.roll = 0;

		// Just copy input as is
		input.InputDirection = input.AnalogMove;
	}

	public override void Update()
	{
		var pawn = Local.Pawn;
		if ( pawn == null ) return;

		var eyePos = pawn.EyePos;
		if ( eyePos.Distance( lastPos ) < 300 ) // TODO: Tweak this, or add a way to invalidate lastpos when teleporting
		{
			Pos = Vector3.Lerp( eyePos.WithZ( lastPos.z ), eyePos, 20.0f * Time.Delta );
		}
		else
		{
			Pos = eyePos;
		}

		Rot = pawn.EyeRot;

		//FieldOfView = 80;

		Viewer = pawn;
		lastPos = Pos;
	}
}
