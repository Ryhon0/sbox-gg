using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

[Library( "fix" )]
public partial class Fix : Weapon, IAimableWeapon
{
	public static SoundEvent Shoot = new SoundEvent( "weapons/fix/fix_shoot.vsnd" );
	public override string ShootShound => "Fix.Shoot";
	public override float Spread => ZoomedIn ? 0 : 0.2f;
	public override float Damage => 100;
	public override bool IsAutomatic => false;
	public override int ClipSize => 8;
	public override float AttackInterval => 0.75f;
	public override float ReloadTime => 2.5f;
	public override string ViewModelPath => "weapons/fix/v_fix.vmdl";
	public override string WorldModelPath => "weapons/fix/fix.vmdl";
	public override HoldType HoldType => HoldType.Shotgun;

	public virtual float ZoomFov => 30f;
	public override CrosshairType CrosshairType => CrosshairType.None;
	[Net, Predicted]
	public bool ZoomedIn { get; set; }
	public bool IsAimed => ZoomedIn;
	float RegularFov = 90;

	public override bool CanSecondaryAttack()
	{
		return TimeSinceDeployed > DeployTime &&
			!IsReloading &&
			Input.Pressed( InputButton.Attack2 );
	}

	public override void AttackSecondary()
	{
		if ( ZoomedIn ) ScopeOut();
		else ScopeIn();
	}

	public virtual void ScopeOut()
	{
		if ( !ZoomedIn ) return;
		if ( Owner?.Camera is GGCamera c )
		{
			if ( IsClient )
			{
				Scope?.SetClass( "zoomed", false );
				if ( ViewModelEntity != null )
				{
					ViewModelEntity.EnableDrawing = true;
					ViewModelEntity.EnableShadowCasting = false;
				}
			}
			using ( Prediction.Off() )
				c.FieldOfView = 90;
			ZoomedIn = false;
		}
	}

	public virtual void ScopeIn()
	{
		if ( ZoomedIn ) return;
		if ( Owner?.Camera is GGCamera c )
		{
			if ( IsClient )
			{
				Scope.SetClass( "zoomed", true );
				ViewModelEntity.EnableDrawing = false;
			}
			using ( Prediction.Off() )
				c.FieldOfView = ZoomFov;
			ZoomedIn = true;
		}
	}

	public override void Reload()
	{
		ScopeOut();
		base.Reload();
	}

	protected override void ShootEffects()
	{
		if ( IsServer ) return;

		if ( !ZoomedIn )
		{
			if ( MuzzleFlash != null )
				Particles.Create( MuzzleFlash, EffectEntity, "muzzle" );
			if ( Brass != null )
				Particles.Create( Brass, EffectEntity, "ejection_point" );
		}

		if ( Owner == Local.Pawn )
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimBool( "fire", true );
		if ( CrosshairPanel is Crosshair c ) c.fireCounter += 2;
	}

	Scope Scope;
	public override void CreateHudElements()
	{
		base.CreateHudElements();

		Scope = new Scope();
		Scope.Parent = Local.Hud;
		Scope.AddClass( CrosshairType.ToString().ToLower() );
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		ScopeOut();
		Scope?.Delete();
	}
}

public class Scope : Panel
{
	Panel Crosshair;
	public Scope()
	{
		StyleSheet.Load( "/Weapons/Scope.scss" );
		AddClass( "scope" );

		Black.Add( Add.Panel( "l u" ) );
		Black.Add( Add.Panel( "u" ) );
		Black.Add( Add.Panel( "r u" ) );
		Black.Add( Add.Panel( "l" ) );
		Black.Add( Add.Panel( "r" ) );
		Black.Add( Add.Panel( "d l" ) );
		Black.Add( Add.Panel( "d" ) );
		Black.Add( Add.Panel( "d r" ) );

		foreach ( var p in Black )
		{
			p.AddClass( "black" );
		}
	}

	List<Panel> Black = new List<Panel>();
	public override void Tick()
	{
		var min = Math.Min( Screen.Height, Screen.Width );
		Style.Height = min;
		Style.Width = min;

		var max = Math.Max( Screen.Height, Screen.Width );
		foreach ( var p in Black )
		{
			p.Style.Width = max;
			p.Style.Height = max;
		}

		this.PositionAtCrosshair();
	}
}
