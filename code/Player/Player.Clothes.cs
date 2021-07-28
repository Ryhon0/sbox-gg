using Sandbox;
using System.Linq;

partial class Player
{
	public void DressUp()
	{
		Wear( "models/clothes/hotdog/hotdog.vmdl" );
	}

	public ModelEntity Wear( string path )
	{
		var outfit = new ModelEntity();

		outfit.SetModel( path );
		outfit.SetParent( this, true );
		outfit.EnableShadowInFirstPerson = true;
		outfit.EnableHideInFirstPerson = true;
		outfit.Tags.Add( "clothes" );

		return outfit;
	}

	public void RemoveClothes()
	{
		foreach ( var c in Children )
		{
			Log.Info( c.EngineEntityName );
			if ( c.Tags.Has( "clothes" ) ) c.Delete();
		}
	}
}
