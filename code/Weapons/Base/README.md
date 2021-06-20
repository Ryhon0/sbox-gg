# Ryhon's Weapon Base

## How to Use
### Downloading
First you need to get the files, there's a few ways to do that:  
<details><summary>ZIP Download</summary><p>
The simplest way to use this library is to press the green "Code" button at the top of the page, press "Download ZIP".  
Once you download the ZIP file, create a `Weapons` folder in your `code` folder, then move all the files to a folder called `Base`.  
You should end up with something like this `(...)/steamapps/common/sbox/addons/myaddon/code/Weapons/Base/Weapon.cs`. This path can be changed, however the UI will not load properly until you change the path.
</p></details>

<details><summary>Git submodule</summary><p>

If you want to get updates (this might break your code, I do not recommend this), use the `git` command line tool and run the following line in your `code/Weapons/` folder:
```sh
git submodule add https://github.com/ryhon0/RWB Base
```
This option will break the "Download ZIP" button on GitHub, if you wish .  
Updates won't automatically download if you already have it downloaded on your computer, to do that, use the following command in the `Base`:
```sh
git pull
```
You won't be able to make changes to the code, if you're interested in doing that the next, the next option is what you want.

</p></details>

<details><summary>Fork</summary><p>
Press the "Fork" button at the top right of the page, this will create a copy of this repository and will not be updated automatically, you will be redirected to a page with that copy, any of the steps above to download it.  
If you're using Git, remember that you need to commit changes to both the weapon base and  
</p></details>

Once you've downloaded the library, you can use the default ammo counter by pasting the following line to your HUD code: 
```cs
RootPanel.AddChild<Ammo>();
```
By default all weapons will use the default crosshairs, you can also make it show how long will the reload take using this line in your `Game` class constructor:
```cs
Crosshair.UseReloadTimer = true;
```

## Creating Weapons
<details><summary>Basics</summary><p>

All properties of weapons are virtual readonly properties, this means you can have some logic inside them (e.g. weapon being more accuracte if you don't spam attack, changing fire modes, grenades exploding on impact only after an amount of time).

The `RPM` property says how many times you will attack per minute (e.g. 60 for 1 attack every 1 second, 600 for 10 attacks every second). If you prefer to use attack intervals instead, use the `AttackInterval` property, by defualt it's returning `60f / RPM`.

The `Damage` property says how much damage will this weapon to entities, the damge will be devided equally for each shot when using the `BulletsPerShot` property (e.g. shotguns, 100 damage with 10 bullets per shot will deal 10 damage per pellet). The `Force` property is how much visual force will be applied to the player's model (how much the model will react to a bullet). If you wish to make your weapon automatic, use the `IsAutomatic` property. If you want a melee weapon, use `IsMelee`, this will change the weapon's range (`Range`) and how thick the ray is (`BulletSize`). If you want to make your weapon more or less accurate, change the `Spread` property.

The `ClipSize` is how big will the magazine be once fully loaded, all weapons are fully loaded by defualt. If you want your weapon to reload shell-by-shell (e.g. shotgun), set the `ReloadMagazine` property to `false`. You can adjust how long will the reload take by changing the `ReloadTime` property, for shell-by-shell reloading, it's the time it takes to reload 1 shell. If you don't want unlimited reserve ammo for your player, make it inheret the `PlayerWithAmmo` class instead of `BasePlayer` and change the `AmmoType` property to an unique value if you want weapons to have separate reserve ammo. 
</p></details>

<details><summary>Animations, Models, Sound</summary><p>

If you want to change third person animations, change the `HoldType` property, the values are only valid for the default Citizen/Terry player model, however because it's an enum, the values can be set to anything you want. These values will set the `holdtype` AnimGraph parameter. For reloading in third person, the `b_reload` parameter is set

If you're making viewmodel animations, use the following AnimGraph parameter:
- `reload` - Set to `true` when reloading a weapon
- `reload_finished` - Set to `true` when using shell-by-shell reloading
- `fire` - Set to `true` when attacking

If you want to change the weapon's model, change the `WorldModelPath` and `ViewModelPath` properties. You can also change the `MuzzleFlash` property, the particle will be spawned at the `muzzle` attachment point. You can do the same with the `Brass` property, it will spawn an empty casing at the `ejection_point` attachment point. If you don't want either of those, set them to `null`

When attacking, a sound with the ID from the `AttackSound` property will be played, if it's `null`, it will be ignored. [Here's how to use custom sounds](https://wiki.facepunch.com/sbox/Sound_Events#creatingsoundeventsincode)
</p></details>

<details><summary>Projectiles</summary><p>

Projectiles are currently not completly finished, use them at your own risk.  
Projectile weapons will spawn a class with the `Projectile` property as their `[Library]` attribute name, to create your own, create a class inhereting from `Projectile`. The weapon has the `ProjectileSpeed` property for setting the projectile's speed.  

You can change the projectile's model using the `ModelPath` property. **The model must have physics or it will be stuck in air**.  
Projectiles can explode on impact with players using the `DestroyOnPlayerImpact` property, world using `DestroyOnWorldImpact` or stick to walls and players using `StickInWalls`, like arrows (This is very buggy and crashes a lot, don't use it). If you don't want the projectile to explode, set `Explosive` to false.  
Explosions will deal full damage if the entity is within the `MinimumDamageRadius` and half damage at the very edge of `ExplosionRadius`.  
If you wish to see those radiuses, use the following code in your `Game` class to show debug spheres:
```cs
Projectile.DebugDrawRadius = true;
```
</p></details>

<details><summary>Burst Fire</summary><p>

Creating burst fire weapons is very easy, just set the `ShotsPerTriggerPull` to the amount of shots you want to fire each time you press the attack button. Just like with regular attacks, bust fire has their own `BurstRPM` and `BurstInterval` properties, those are separate from the regular ones.  
Here's a diagram showing how they work:
```
Legend:
- - AttackInterval (0.1ms)
= - BurstInterval (0.1ms)
o - Shot

Regular:
o---o---o---o---o--o

Burst of 3:
o==o==o---o==o==o
```
</p></details>
