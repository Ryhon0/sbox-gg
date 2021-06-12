using Sandbox;

partial class Weapon
{

	public override void Reload()
	{
		if ( IsMelee || IsReloading ) return;
		if ( AmmoClip >= ClipSize ) return;

		TimeSinceReload = 0;
		IsReloading = true;

		(Owner as AnimEntity).SetAnimBool( "b_reload", true );

		StartReloadEffects();
	}
	public virtual void OnReloadFinish()
	{
		if ( ReloadMagazine )
		{
			AmmoClip = ClipSize;
			IsReloading = false;
		}
		else
		{
			if ( AmmoClip >= ClipSize )
				return;

			AmmoClip++;

			if ( AmmoClip < ClipSize )
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
