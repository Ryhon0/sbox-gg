using Sandbox;

partial class Weapon
{

	public override void Reload()
	{
		if ( IsReloading && !ReloadMagazine ) (Owner as AnimEntity).SetAnimBool( "reload", true );
		if ( IsMelee || IsReloading ) return;
		if ( AmmoClip >= ClipSize ) return;
		if ( AvailableAmmo <= 0 ) return;

		TimeSinceReload = 0;
		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
	}
	public int OwnerTakeAmmo( int type, int count )
	{
		if ( Owner is PlayerWithAmmo p )
			return p.TakeAmmo( AmmoType, count );
		return count;
	}
	public virtual void OnReloadFinish()
	{
		if ( ReloadMagazine )
		{
			AmmoClip = OwnerTakeAmmo( AmmoType, ClipSize );
			IsReloading = false;
		}
		else
		{
			if ( AmmoClip >= ClipSize )
				return;

			AmmoClip++;

			if ( OwnerTakeAmmo( AmmoType, 1 ) != 0 && AmmoClip < ClipSize )
			{
				Reload();
				TimeSinceReload = 0;
			}
			else
			{
				ViewModelEntity?.SetAnimBool( "reload_finished", true );
				IsReloading = false;
			}
		}
	}
	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimBool( "reload", true );

		// TODO - player third person model reload
	}
}
