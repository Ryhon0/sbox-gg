using Sandbox.UI;

[UseTemplate]
public partial class GunGameHUD : Panel
{
    public static GunGameHUD Current;

    public Panel CurrentWeapon { get; set; }
    public Panel NextWeapon { get; set; }
    public Label RotationName { get; set; }
    public Label Winner { get; set; }
    public Panel WeaponsPanel { get; set; }

    public GunGameHUD()
    {
        Current = this;

        StyleSheet.Load("/ui/GunGame/GunGameHUD.scss");
    }

    public void UpdateWeapons(int score)
    {
        var game = (Game.Current as Game);

        RotationName.Text = game.RotationName;

        var current = game.GetWeapon(score);
        var next = game.GetWeapon(score + 1);

        if (current != null)
        {
            WeaponsPanel.SetClass("hidden", false);
            CurrentWeapon.Style.Set("background-image", $"url(/ui/weapons/{current}.png)");

            if (next != null) NextWeapon.Style.Set("background-image", $"url(/ui/weapons/{next}.png)");
            else NextWeapon.Style.Set("background-image", $"url(/ui/win.png)");
        }
        else
        {
            WeaponsPanel.SetClass("hidden", true);
        }
    }

    public Panel WinScreen { get; set; }
    public void ShowWinner(Player c)
    {
        LoadWorld();
        if (c != null)
        {
            WinnerID = c.NetworkIdent;
            Winner.Text = $"🏆 {c.GetClientOwner().Name} wins! 🏆";
        }
        else WinnerID = 0;

        WinScreen.SetClass("visible", c != null);
    }
}
