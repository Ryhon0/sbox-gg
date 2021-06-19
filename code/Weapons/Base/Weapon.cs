using Sandbox;

public enum FireType
{
	SemiAuto,
	FullAuto,
	Burst3,
	Burst2
}

public enum CrosshairType
{
	Dot,
	Circle,
	Sides,
	None,
	Cross
}

public partial class Weapon : BaseWeapon
{
	// Networked variables
	[Net, Predicted]
	public int AmmoClip { get; set; }
	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }
	[Net, Predicted]
	public bool IsReloading { get; set; }
	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	// Ammo
	public virtual int ClipSize => 1;
	public virtual bool ReloadMagazine => true;
	public virtual float ReloadTime => 2f;

	// Projectile specific
	public virtual string Projectile => null;
	public virtual float ProjectileSpeed => 1000;

	// Hitscan specific
	public virtual float BulletSize => IsMelee ? 50f : 1f;
	public virtual float Range => IsMelee ? 75 : 5000;

	// Stats
	public virtual bool IsMelee => false;
	public virtual float Force => 0.5f;
	public virtual float Damage => 10f;
	public virtual int BulletsPerShot => 1;
	public virtual bool IsAutomatic => true;
	public virtual int BulletsPerShot => 1;
	public virtual float Spread => 0.1f;
	public virtual int RPM => 600;
	public virtual float AttackInterval => 60f / RPM;
	public virtual float DeployTime => .75f;


	// Audio/Visual
	public virtual int HoldType => 1;
	public virtual CrosshairType CrosshairType => CrosshairType.Dot;
	public virtual string ShootShound => "rust_pistol.shoot";
	public virtual string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;
		IsReloading = false;
	}
	public override void Spawn()
	{
		base.Spawn();

		if ( WorldModelPath != null )
			SetModel( WorldModelPath );

		AmmoClip = ClipSize;
	}
	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new GGViewModel();
		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
		ViewModelEntity.SetModel( ViewModelPath );
	}
	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < DeployTime )
			return;

		if ( ReloadMagazine ? !IsReloading : true )
		{
			{
				if ( Input.Down( InputButton.Reload ) )
				{
					Reload();
				}

				//
				// Reload could have deleted us
				//
				if ( !this.IsValid() )
					return;

				if ( CanPrimaryAttack() )
				{
					AttackPrimary();
				}

				//
				// AttackPrimary could have deleted us
				//
				if ( !Owner.IsValid() )
					return;

				if ( CanSecondaryAttack() )
				{
					AttackSecondary();
				}
			}

			if ( ClipSize == 1 && TimeSincePrimaryAttack > AttackInterval )
			{
				Reload();
			}
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;

		CrosshairPanel = new Crosshair();
		CrosshairPanel.Parent = Local.Hud;
		CrosshairPanel.AddClass( CrosshairType.ToString().ToLower() );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", HoldType ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
