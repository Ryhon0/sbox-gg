using Sandbox;

public partial class Player : Sandbox.Player
{
	public override void Respawn()
	{
		Inventory.DeleteContents();
		SetModel( "models/citizen/citizen.vmdl" );
		this.SetMaterialGroup( Rand.Int( 0, 1 ) == 0 ? 0 : 3 );

		var outfit = new ModelEntity();

		outfit.SetModel( "models/clothes/hotdog/hotdog.vmdl" );
		outfit.SetParent( this, true );
		outfit.EnableShadowInFirstPerson = true;
		outfit.EnableHideInFirstPerson = true;

		Controller = new GGController();
		Animator = new PlayerAnimator();
		Camera = new FirstPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		(Game.Current as Game).RequestWeapon( this );

		base.Respawn();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );
	}
	DamageInfo LastDamage;
	public override void TakeDamage( DamageInfo info )
	{
		LastDamage = info;

		if ( info.Attacker != null )
		{
			if ( !(info.Weapon as Weapon)?.IsMelee ?? false )
			{
				// Check for headshot
				if ( info.HitboxIndex == 5 )
				{
					info.Damage *= 2.0f;
				}
			}
			else
			{
				// Check for backstab
				var facing = Rotation.Forward;
				var attackedfrom = info.Attacker.Rotation.Forward;
				var dot = facing.Dot( attackedfrom );
				if ( dot > 0 ) info.Damage *= 2;
			}
		}

		base.TakeDamage( info );

		if ( info.Attacker is Player attacker && attacker != this )
		{
			// Note - sending this only to the attacker!
			attacker.DidDamage( To.Single( attacker ), info.Position, info.Damage, Health.LerpInverse( 100, 0 ) );

			TookDamage( To.Single( this ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position );
		}
	}
	[ClientRpc]
	public void TookDamage( Vector3 pos )
	{
		//DebugOverlay.Sphere( pos, 5.0f, Color.Red, false, 50.0f );

		DamageIndicator.Current?.OnHit( pos );
	}
	[ClientRpc]
	public void DidDamage( Vector3 pos, float amount, float healthinv )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + healthinv * 1 );

		HitIndicator.Current?.OnHit( pos, amount );
	}
}

public class KillArgs
{
	public DamageInfo Info;
	public Player Killed;
	public KillArgs( DamageInfo info, Player killed )
	{
		Info = info;
		Killed = killed;
	}
}
