﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

partial class GunGameHUD
{
	private Angles CamAngles;
	static SceneCapture sceneCapture;
	private AnimSceneObject playerPreview;
	private AnimSceneObject playerCostumePreview;
	private float startTime;

	public Image heroImage { get; set; }
	public int WinnerID;

	[Event.Hotload]
	void OnHotload()
	{
		LoadWorld();
	}

	private void DeleteScene()
	{
		sceneCapture?.Delete();
		sceneCapture = null;
	}

	private void LoadWorld()
	{
		var par = heroImage.Parent;
		heroImage.Delete();
		heroImage = Add.Image( "scene:portrait", "hero-image" );
		heroImage.Parent = par;
		DeleteScene();

		using ( SceneWorld.SetCurrent( new SceneWorld() ) )
		{
			playerPreview = new AnimSceneObject( Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );

			Light.Point( Vector3.Up * 150.0f, 200.0f, Color.White * 5000.0f );
			Light.Point( Vector3.Up * 100.0f + Vector3.Forward * 100.0f, 200, Color.White * 15000.0f );

			sceneCapture = SceneCapture.Create( "portrait", 512, 512 );

			sceneCapture.AmbientColor = new Color( 0.8f, 0.8f, 0.8f );
			sceneCapture.SetCamera( Vector3.Up * 100 + CamAngles.Direction * -50, CamAngles, 45 );

			// Clothes
			playerCostumePreview = new AnimSceneObject( Model.Load( "models/clothes/hotdog/hotdog.vmdl" ), Transform.Zero );
			playerPreview.AddChild( "outfit", playerCostumePreview );


			startTime = Time.Now;
		}
	}

	public override void OnDeleted()
	{
		sceneCapture?.Delete();
		sceneCapture = null;

		base.OnDeleted();
	}

	[Event.Tick]
	public override void Tick()
	{
		base.Tick();

		Player player = Entity.All.OfType<Player>().FirstOrDefault( p => p.NetworkIdent == WinnerID );

		CamAngles.yaw = 180;
		if ( player == null ) return;
		if ( player.GetActiveAnimator() is PlayerAnimator animator )
		{
			// Animation overrides
			CopyParams( animator, playerPreview );
		}

		playerPreview.Update( Time.Now - startTime );
		playerCostumePreview.Update( Time.Now - startTime );
		sceneCapture?.SetCamera( new Vector3( 175, 0, 30 ), CamAngles, 30 );
	}

	void CopyParams( PlayerAnimator from, AnimSceneObject to )
	{
		from.Params["lookat_pos"] = new Vector3( 10, 0, 0 );
		foreach ( var animParam in from.Params )
		{
			if ( animParam.Value is int intAnimValue )
				to.SetAnimParam( animParam.Key, intAnimValue );
			else if ( animParam.Value is bool boolAnimValue )
				to.SetAnimParam( animParam.Key, boolAnimValue );
			else if ( animParam.Value is float floatAnimValue )
				to.SetAnimParam( animParam.Key, floatAnimValue );
			else if ( animParam.Value is Vector3 vector3AnimValue )
				to.SetAnimParam( animParam.Key, vector3AnimValue );
		}
	}
}