
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Ammo : Panel
{
	public Label Weapon;

	public Ammo()
	{
		StyleSheet.Load("/Weapons/Base/UI/Ammo.scss");

		Weapon = Add.Label("100", "weapon");
	}

	public override void Tick()
	{
		var player = Local.Pawn;
		if (player == null) return;

		var weapon = player.ActiveChild as Weapon;

		SetClass("active", weapon != null);
		if (weapon == null) return;

		if (weapon.ClipSize == 1)
		{
			SetClass("hidden", true);
		}
		else
		{
			SetClass("hidden", false);
			Weapon.Text = $"{weapon.AmmoClip}  / {weapon.ClipSize}";
		}
	}
}
