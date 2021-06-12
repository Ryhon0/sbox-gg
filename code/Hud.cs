using Sandbox.UI;

public partial class MinimalHudEntity : Sandbox.HudEntity<RootPanel>
{
	public MinimalHudEntity()
	{
		if ( !IsClient ) return;

		RootPanel.StyleSheet.Load( "Hud.scss" );

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Ammo>();

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();

		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard>();
		RootPanel.AddChild<VoiceList>();
	}
}
