using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[Library("gg")]
public partial class Game : Sandbox.Game
{
    static SoundEvent NextLevel = new SoundEvent("sounds/electrical/powerup.vsnd", 1);

    public Game()
    {
        Crosshair.UseReloadTimer = true;
        if (IsServer)
        {
            new MinimalHudEntity();
        }

        if (IsClient)
        {

        }
    }

    public override void Spawn()
    {
        base.Spawn();
        StartRound();

    }

    public override void DoPlayerNoclip(Client player)
    {
    }

    public override void ClientJoined(Client client)
    {
        base.ClientJoined(client);
        var player = new Player();
        client.Pawn = player;
        client.SetScore("rank", 0);
        player.Respawn();
        UpdateWeaponList(To.Single(client), string.Join(',', Weapons), RotationName);
        UpdateWeapons(To.Single(client), 0);
    }

    public string RotationName = "Unknown";
    public List<string> Weapons = new List<string>();

    TimeSince TimeSinceRoundFinish;
    float VictoryScreenLength = 5f;
    bool GameFinished;

    [Event("player_killed")]
    void PlayerKilled(KillArgs args)
    {
        if (args.Info.Attacker is Player p)
        {
            // Check for double skip when using shotguns
            if (args.Info.Weapon.ClassInfo.Name == GetWeapon(args.Info.Attacker.GetClientOwner().GetScore<int>("rank")) &&
                args.Info.Attacker != args.Killed)
            {
                var owner = p.GetClientOwner();

                var score = GivePoint(owner);

                using (Prediction.Off())
                {
                    var c = args.Info.Attacker.GetClientOwner();
                    UpdateWeapons(To.Single(owner), score);
                }

                if (score == Weapons.Count)
                {
                    using (Prediction.Off())
                        ShowWinner(To.Everyone, p);

                    GameFinished = true;
                    TimeSinceRoundFinish = 0;
                }
                else if (score == Weapons.Count - 1)
                {
                    using (Prediction.Off())
                        ShowLastWeaponWarning(To.Everyone, p);
                }
            }
        }

        args.Killed.GetClientOwner()?.SetScore("deaths", args.Killed.GetClientOwner().GetScore<int>("deaths", 0) + 1);
    }

    [Event.Tick]
    void Tick()
    {
        if (GameFinished)
        {
            if (TimeSinceRoundFinish > VictoryScreenLength)
                StartRound();
        }
    }

    int GivePoint(Client c)
    {
        var rank = c.GetScore<int>("rank", 0) + 1;
        c.SetScore("rank", rank);

        c.Pawn?.PlaySound("Game.NextLevel");

        GiveWeapon(c.Pawn as Player, GetWeapon(rank));
        return rank;
    }

    public string GetWeapon(int rank)
    {
        var i = rank;
        if (Weapons.Count > i) return Weapons[i];
        else return null;
    }

    void GiveWeapon(Player p, string weapon)
    {
        if (weapon == null || p == null) return;

        if (!(p.Inventory as GGInventory).IsCarryingId(weapon))
        {
            p.Inventory.DeleteContents();
            var w = Entity.Create(weapon);
            p.Inventory.Add(w, true);
        }
    }

    void StartRound()
    {
        if (IsServer)
        {
            LoadRandomWeaponRotation();
            UpdateWeaponList(To.Everyone, string.Join(',', Weapons), RotationName);
        }

        foreach (var c in Client.All)
        {
            c.SetScore("rank", 0);
            c.SetScore("deaths", 0);
            (c.Pawn as Player).Respawn();
        }

        GameFinished = false;
        UpdateWeapons(To.Everyone, 0);
        ShowWinner(To.Everyone, null);
    }

    string RotationRegex = @"^.*\.gg$";
    void LoadRandomWeaponRotation()
    {
        // Check if user defined rotations exist
        if (FileSystem.Data.DirectoryExists("rotation"))
        {
            // Check if there are any custom rotations defined
            var custom = FileSystem.Data.FindFile("rotation", "*");
            if (custom.Any())
            {
                var regex = new Regex(RotationRegex);
                custom = custom.Where(f => regex.IsMatch(f));
            }

            if (custom.Any())
            {
                var crotf = custom.GetRandom();
                Log.Info("Loading custom rotation: " + crotf);
                LoadWeaponRotation(FileSystem.Data.ReadAllText("rotation/" + crotf));
                return;
            }
        }
        else
        {
            // Create rotation directory and example rotation file
            FileSystem.Data.CreateDirectory("rotation");
            FileSystem.Data.WriteAllText("rotation/example_rotation.gg.example", FileSystem.Mounted.ReadAllText("config/rotation/default.gg"));
        }

        var rotf = FileSystem.Mounted.FindFile("config/rotation", "*").GetRandom();
        Log.Info("Loading built-in rotation: " + rotf);
        LoadWeaponRotation(FileSystem.Mounted.ReadAllText("config/rotation/" + rotf));
    }

    void LoadWeaponRotation(string text)
    {
        var lines = text.Split('\n');
        var name = "Unknown";
        var list = "";

        if (lines.Length == 2)
        {
            name = lines[0];
            list = lines[1];
        }
        else list = lines[0];

        RotationName = name;
        Weapons = list.Split(',').ToList();
    }

    [ClientRpc]
    void UpdateWeaponList(string weapons, string name)
    {
        RotationName = name;
        Weapons = weapons.Split(',').ToList();
    }

    public void RequestWeapon(Player p)
    {
        GiveWeapon(p, GetWeapon(p.GetClientOwner().GetScore<int>("rank", 0)));
    }
}
