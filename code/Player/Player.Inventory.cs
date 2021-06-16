using Sandbox;
using System;
using System.Linq;

public partial class Player
{
	public Player()
	{
		Inventory = new GGInventory( this );
	}

	public override void StartTouch( Entity other )
	{
		if ( other is BaseWeapon ) other.Delete();

		base.StartTouch( other );
	}
}

partial class GGInventory : BaseInventory
{
	public GGInventory( Player player ) : base( player )
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		var player = Owner as Player;
		var weapon = ent as Weapon;

		if ( weapon != null && IsCarryingType( ent.GetType() ) )
		{
			var ammo = weapon.AmmoClip;

			// Despawn it
			ent.Delete();
			return false;
		}

		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}

	public bool IsCarryingId( string id )
	{
		return List.Any( x => x.EntityName == id );
	}
}
