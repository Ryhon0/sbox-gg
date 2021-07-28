using Sandbox;

public enum CrosshairType
{
	Dot,
	Circle,
	Sides,
	None,
	Cross
}
public enum HoldType
{
	Unarmed = 0,
	Pistol = 1,
	SMG = 2,
	Shotgun = 3,
	Universal = 4
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
	public virtual int AmmoType => 0;

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
	public virtual DamageFlags DamageFlags => DamageFlags.Bullet;
	public virtual bool IsAutomatic => true;
	public virtual int BulletsPerShot => 1;
	public virtual float Spread => 0.1f;
	public virtual int RPM => 600;
	public virtual float AttackInterval => 60f / RPM;
	public virtual float DeployTime => .75f;

	// Burst fire
	public virtual int ShotsPerTriggerPull => 1;
	public virtual float BurstRPM => RPM;
	public virtual float BurstInterval => 60f / BurstRPM;

	// Audio/Visual
	public virtual HoldType HoldType => HoldType.Pistol;
	public virtual CrosshairType CrosshairType => CrosshairType.Dot;
	public virtual string ShootShound => "rust_pistol.shoot";
	public virtual string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public virtual string MuzzleFlash => "particles/pistol_muzzleflash.vpcf";
	public virtual string Brass => "particles/pistol_ejectbrass.vpcf";

	public int AvailableAmmo
	{
		get
		{
			if ( Owner is PlayerWithAmmo p )
				return p.AmmoCount( AmmoType );

			return ClipSize;
		}
		set
		{
			if ( Owner is PlayerWithAmmo p )
			{
				p.SetAmmo( AmmoType, value );
			}
		}
	}

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
		ViewModelEntity?.SetAnimBool( "empty", AmmoClip <= 0 );

		if ( TimeSinceDeployed < DeployTime )
			return;

		if ( ReloadMagazine ? !IsReloading : true )
		{
			{
				if ( CanReload() )
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
					TimeSincePrimaryAttack = 0;
				}

				//
				// AttackPrimary could have deleted us
				//
				if ( !owner.IsValid() )
					return;

				if ( CanSecondaryAttack() )
				{
					AttackSecondary();
					TimeSinceSecondaryAttack = 0;
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
		anim.SetParam( "holdtype", (int)HoldType ); // TODO this is shit
		anim.SetParam( "aimat_weight", 1.0f );
	}
}
