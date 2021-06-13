﻿
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label Weapon;
	public Label Inventory;

	public Ammo()
	{
		Weapon = Add.Label( "100", "weapon" );
		Inventory = Add.Label( "100", "inventory" );
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if ( player == null ) return;

		var weapon = player.ActiveChild as Weapon;

		SetClass( "active", weapon != null );
		if ( weapon == null ) return;

		if ( weapon.ClipSize == 1 )
		{
			Weapon.Text = Inventory.Text = "";
		}
		else
		{
			Weapon.Text = $"{weapon.AmmoClip}";
			Inventory.Text = $" / {weapon.ClipSize}";
		}
	}
}
