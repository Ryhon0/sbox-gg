using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Linq;

partial class GunGameHUD
{
	private Angles CamAngles;
	private AnimSceneObject playerPreview;
	private AnimSceneObject playerCostumePreview;
	private AnimSceneObject crownPreview;

	public ScenePanel heroScene { get; set; }
	public int WinnerID;

	public Panel HeroPortrait { get; set; }
	private void LoadWorld()
	{
		if ( heroScene != null ) return;

		using ( SceneWorld.SetCurrent( new SceneWorld() ) )
		{
			playerPreview = new AnimSceneObject( Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );

			var light = new Light( new Vector3( -100, 100, 150 ), 2000, Color.White );
			light.Falloff = 0;
			//light.Falloff = 0f;
			light = new Light( new Vector3( 700, -30, 170 ), 2000, Color.White );
			light.Falloff = 0;

			//light.Falloff = 0.2f; 
			light = new Light( new Vector3( 100, 100, 150 ), 2000, Color.White );
			light.Falloff = 0;

			// Clothes
			playerCostumePreview = new AnimSceneObject( Model.Load( "models/clothes/hotdog/hotdog.vmdl" ), Transform.Zero );
			playerPreview.AddChild( "outfit", playerCostumePreview );

			crownPreview = new AnimSceneObject( Model.Load( "models/clothes/crown/crown.vmdl" ), Transform.Zero );
			playerPreview.AddChild( "crown", crownPreview );

			heroScene = HeroPortrait.Add.ScenePanel( SceneWorld.Current, new Vector3( 175, 0, 30 ), Rotation.From(CamAngles), 30 );
			heroScene.Style.Width = 720;
			heroScene.Style.Height = 720;

			Angles angles = new( 25, 180, 0 );
			Vector3 pos = Vector3.Up * 40 + angles.Direction * -200;

			heroScene.World = SceneWorld.Current;
			heroScene.Position = pos;
			heroScene.Angles = angles;
			heroScene.FieldOfView = 28;
			heroScene.AmbientColor = Color.Gray * 0.2f;
		}
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

		playerPreview.Update( RealTime.Delta );
		playerCostumePreview.Update( RealTime.Delta );
		crownPreview.Update( RealTime.Delta );
	}

	void CopyParams( PlayerAnimator from, AnimSceneObject to )
	{
		from.Params["lookat_pos"] = new Vector3( 10, 0, 0 );
		foreach ( var animParam in from.Params )
		{
			if ( animParam.Value is int intAnimValue )
				to.SetAnimInt( animParam.Key, intAnimValue );
			else if ( animParam.Value is bool boolAnimValue )
				to.SetAnimBool( animParam.Key, boolAnimValue );
			else if ( animParam.Value is float floatAnimValue )
				to.SetAnimFloat( animParam.Key, floatAnimValue );
			else if ( animParam.Value is Vector3 vector3AnimValue )
				to.SetAnimVector( animParam.Key, vector3AnimValue );
		}
	}
}
