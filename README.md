# Risk of Bulletstorm
A mod for Risk of Rain 2 that plans to add custom content, modified from Enter The Gungeon. Very configurable.

Feel free to message me in the [Risk of Rain 2 Modding Discord](https://discord.gg/5MbXZvd) for anything broken or any suggestions, including any networking issues.
***
# Dependencies
 - [Hard] BepInEx and R2API
 - [Hard] ThinkInvis - TILER2
# Compatibility with other mods
 2. **Bandit Reloaded** : Item Displays
 3. **HAND** : Item Displays
 4. **Various**: Unofficial support (As in I manually added their projectiles in without really asking) for changing the accuracy for projectiles (such as from the uncommon Scope item) for:
	- LuaFubuki: Gauss, Lunar Chimera, Void Reaver
	- KomradeSpectre: Aetherium,
	- Zerodomai: Tristana
	- duckduckgreyduck: ArtificerExtended
	- rob: Direseeker, PlayableTemplar, Paladin, Twitch
	- RyanP: ExpandedSkills
	- TheMysticSword: AspectAbilities
	- Enigma: Cloudburst

# Known Issues:
* Master Round isn't given if the teleporter event was forcefully completed (such as with RORCheats)
* Item Displays on REX and MULT aren't setup properly

# Planned
1. BetterUI, ItemStatsMod Support
2. Multiplayer Support
3. Jarlyk's Hailstorm support (Curse increases mimic chance)
4. Better messages for the Master Round

***
# Items
## Pickups
Every 25 kills *(+10% per stage)*, the game will have a 40% chance to *spawn a random weighted pickup*. "Forgive Me, Please" will not trigger this. If it does, tell me because it's not supposed to.

|Icon| Chance |Item | Description |
|:--:|:--:|:--:|--|
|![](https://i.imgur.com/RNzbyB9.png)| 15% | Armor | Destroys itself to block one hit of damage dealing more than 20% health or deals fatal damage, and fires a Blank.
|![](https://i.imgur.com/kGKjmcw.png)| 45% | Blank| Press T to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use.
|![](https://i.imgur.com/XnEVLst.png)| 70% | Spread Ammo|Pick up to restore *all* allies' cooldowns and restores one equipment charge to all equipment.

## White

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/r7DDd9U.png)|Cartographer's Ring|Upon starting the next stage, 10% chance (+5% chance per stack) of automatically revealing all interactables. Counts based off all players' item counts. Stacking beyond 100% has no effect. (¹)
|![](https://i.imgur.com/mgb7N2g.png)|Mustache|Upon purchase, increases your regeneration by `2 health` `(+2 per stack)` for `10` seconds.
|![](https://i.imgur.com/MFuBwOI.png)|Scope|Reduces spread by -10% (-5% per stack). Stacking beyond 100% has no effect. (²)

(¹) *I recommend installing my other mod, ["HideScannerIndicatorOnUse"](https://thunderstore.io/package/DestroyedClone/HideScannerIndicatorOnUse/), so that the indicator that pops over interactables disappears upon interaction.*

(²) *You can enable all projectiles, but by default it follows a whitelist. Feel free to recommend me other projectiles to add to the whitelist, including any modded projectiles.*

## Green

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/mwbq6Jh.png)|Disarming Personality|*Hyperbolically* reduces purchase costs by 10% (-5% per stack) up to 60%. Chance is shared among players. 
|![](https://i.imgur.com/FqM3osk.png)|Enraging Photo|Gain a temporary `100% damage bonus` that can last up to 2 seconds `(+0.5 seconds duration per stack).` The duration scales from the amount of damage taken, from `1% to 10%`. (¹)
|![](https://i.imgur.com/yxhT1zA.png)|Roll Bomb| Using your utility `drops 1 bomb` for `200% damage`. `(+1 bomb dropped per stack)`

(¹) Duration increased on base and per stack, and is a percentage based on how hard you were damaged with a lower/higher bound. EX: (duration of 5 seconds with a bound of 0% health lost and 20% health loss, so a 10% health loss would result in 2.5 seconds (or half).

## Red

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/msag2Ku.png)|Backpack|Grants 1 extra equipment slot per stack.![](https://i.imgur.com/G5NGtxV.png) - Hold [Left Shift] + [NumberRow keys] to switch between slots.![](https://i.imgur.com/G5NGtxV.png) - Press [Left Shift] + [=] to list all equipment.![](https://i.imgur.com/G5NGtxV.png) - Press [F] or [G] to cycle left/right through equipment.![](https://i.imgur.com/G5NGtxV.png) - Adds onto MULT's additional slot. *(MULT w/ 1 backpack = 3 slots, though you can't use retool to switch to the modded ones)* |
|![](https://i.imgur.com/wAGP4zm.png)|**Unity**| +0.2 `(+0.05 per stack)` base damage per item in inventory.
|![](https://i.imgur.com/3b82HWP.png)|Number 2|Increases your base stats for every dead player.![](https://i.imgur.com/G5NGtxV.png)*Singleplayer: Starts with one player dead.*

 **Number 2 Stat Bonuses**
 
| Stat | Bonus 
|:--:|:--:|
| baseAttackSpeedAdd | 0.25
| baseDamageAdd | 2
| baseHealthAdd | 25
| baseMoveSpeedAdd | 1
| baseRegenAdd | 0.5
| armorAdd | 2
| critAdd | 5


## Lunar

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/J0Gpgqo.png)|**Metronome**|Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150% (75 kills). Gain 25 extra stacks per pickup. Lose 75 stacks upon using a different skill.|
|![](https://i.imgur.com/xWR2pab.png)|**Ring of Miserly Protection**|Grants +100% (+50% per stack) increased maximum health `...but one shatters upon using a shrine` (does not include combat).|


## Boss

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/sst33CW.png)|**Master Round**|+10% max health per stack.![](https://i.imgur.com/G5NGtxV.png)`Granted upon clearing a teleporter boss, as long as no player took more than 3 hits (scales with stages cleared) each.` |

# Equipment
## Normal

|Icon| Item | Description | CD |
|:--:|:--:|--|:--:|
| ![](https://i.imgur.com/vSRIX8I.png) | **Bomb**| Throws a bomb for `300% damage`. | 14s |
| ![](https://i.imgur.com/XKVAmkD.png) | **Charm Horn**| Upon use, blows the horn to `Charm` enemies within `20 meters` for 10 seconds. | 85s |
| ![](https://i.imgur.com/CHAxjQN.png) | **Friendship Cookie** | Revives all players and gives 3 seconds of immunity. If in singleplayer, gives an Infusion instead. Consumed on use. | 0s |
| ![](https://i.imgur.com/PwZygkK.png) | **Magazine Rack**| Place to create a zone of no cooldowns within a radius of 3.5m for 2.5 seconds. | 14s |
| ![](https://i.imgur.com/zmdbNX8.png) | **Meatbun** | Heals the player for 33% health. Grants +10% damage until the player takes more than 5% damage. Buff stacks up to 5 times. | 90s |
| ![](https://i.imgur.com/p8HO3m1.png) | **Medkit** | Heals the player for 75% max health and restores 50% barrier. | 55s |
| ![](https://i.imgur.com/ISfLK77.png) | **Molotov** | Throw to cover an area in flames for 10% damage per second for 8 seconds. | 55s |
| ![](https://i.imgur.com/Cvvu8rB.png) | **Orange** | Consuming permanently reduces equipment recharge rate by 10%, increases health by 10%, fully heals the player, but is removed from the inventory. | 0s |
| ![](https://i.imgur.com/hPjK5TE.png) | **Portable Barrel Device** | Places a barrel, but doesn't give any money or experience for picking it up. | 30s |
| ![](https://i.imgur.com/6nzBNeh.png) | **Ration** | Heals for 40% health. Automatically used upon taking fatal damage. Consumed on use. | 0s |

## Lunar

|Icon| Item | Description |
|:--:|:--:|--|
|![](https://i.imgur.com/T71srU2.png)|**Spice**|Increases and decreases stats per consumed Spice.  It's been slightly adjusted from Gungeon's values. These values are constant for each stack, not additive, except for the 6+ stack.

| Spice Consumed | Health Multiplier | Attack Speed | Accuracy | Enemy Bullet Speed | Damage | Curse
|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| 1 | +25% | +20% | +0%  | +0%   | +0%   | +0.5
| 2 | +50% | +40% | +25% | +0%   | +0%   | +1
| 3 | +25% | +40% | +25% | -10%  | +20%  | +1
| 4 | +0%  | +40% | +15% | -15%  | +35% | +1 
| 5 | 5 curse | +40% | -10% | -15%  | +50% | +1
| 6+ | +5 curse | +0% | -10% | -0%  | +15% | +1
 
 - Consuming beyond 5 will continue to add the bonuses from 6+. It's not exact, but the health reduction is added via PermanentCurse. Maximum health reduced by curse is reduced by a factor of `1 + 0.01 * n`, where `n` is the number of stacks.

## Special

 - **Curse** - Certain items apply curse. The chance of encountering a Jammed enemy increases as you gain more Curse. This is independent of elite affixes.

| Curse Count | Enemy Chance | Boss Chance |
|:--:|:--:|:--:|
| 0 | 0% | 0% |
| 0.5 - 2.0 | 1% | 0% |
| 2.5 - 4.0 | 2% | 0% |
| 4.5 - 6.0 | 5% | 0% |
| 6.5 - 8.0 | 10% | 20% |
| 8.5 - 9.5 | 25% | 30% |
| 10+ | 50% | 50% |


 - **Accuracy** - Certain items (Spice, Scope) affect the accuracy.
	 - +Acc: Tighter bullet spread, projectiles fire closer towards your crosshair
	 - -Acc: Wider bullet spread, projectiles tend to become inaccurate.

## Buffs

 - **[Enraged]**: +100% damage.

---AI Specific:---
 - **[Charmed]**: Only targets any team except the team they're charmed by, and won't retaliate against the charmer's team.
 - **[Jammed]**: +100% damage, +100% crit chance, +20% attack speed, +20% movement speed. 

# Changelog

1.1.1 - Patches
* *Master Round---*
	* Fixed giving only one per player.
	* Modified event message conditions to make more consistent between players.
	* Lunar Variant changed to Legendary to prevent appearing where it shouldn't.
* Spice
	* Amount of consumed Spice now shows in inventory.
	* Reordered methods to hopefully prevent weird shenanigans with Fuel Cells+Gestures.
	* Clarified readme table.
	* Added config option to make Spice chance based per-player. Downside is that it won't change the item visibly, so it'll be more confusing and chance-based on pickup. 
	* Added cap of 40 consumed per player to match origin
* Portable Barrel
	* Changed max deployabled barrels to a per-person basis
* Orange
	* Consumed oranges now give a single item, visible for tracking
* Mustache
	* Changed to regen instead of a flat heal.
* Enraging Photo
	* Duration increased on base and per stack, and is now is a percentage based on how hard you were damaged with a lower/higher bound. EX: (duration of 5 seconds with a bound of 0% health lost and 20% health loss, so a 10% health loss would result in 2.5 seconds (or half).
* Scope
	* Nerfed stacking and moved to white tier for more effective stacking sense.
* Unity
	* Nerfed damage and stacking, but now scales of total item count rather than unique item count.
* Curse
	* Curse amount shown in inventory
	* New Jammed Enemy visuals
* Pickups
	* Buffed rates and added indicator
* Buffed overall stats:
	* Bomb, Roll Bomb, Number2
* Fixed Bomb and Molotov from not working on Equipment Drones.
* Various visual updates, including a ton of IDRs

1.1.0 - Content Update:
 -  *Added---* 
	 - Pickups: Armor, Blank, and Spread Ammo
	 - Whites: Cartographer's Ring
	 - Greens: Disarming Personality, Enraging Photo, Roll Bomb
	 - Reds: Backpack, Number 2, Unity
	 - Lunars: Ring of Miserly Protection
	 - Boss: Master Round
	 - Equipment: Charm Horn, Friendship Cookie, Magazine Rack, Meatbun, Medkit, Molotov, Orange, Portable ~~Table~~ Barrel Device, Ration
	 - Lunar Equipment: Spice
 - *Mustache---* Added config for interaction with Blood Shrines. Added interaction with Sacrifice.
 - *Scope---* now affects ALOT of projectiles but not all. Can affect all projectiles via config (strongly discouraged).
 - *Metronome---* Added config for stack loss. Added stack display via buff count. Component changes should prevent the negative damage bug.
 - *Bomb---* Added assets, increased cooldown
 - *EliteSpawningOverhaul---* Temporarily removed dependency due to changing how Jammed is applied. Deciding whether to keep or not.
 - *ClassicItems---* Temporarily removed soft dependency due to it completely breaking the plugin.

1.0.1 - Reverted dependency of R2API. Adjusted item descriptions.

1.0.0 - Released!

## Credits
Rico - Equipment Removal
Rein - Nonspecific, DirectorAPI
KomradeSpectre, Chen - Lots, and TILER2 Help
KingEnderBrine - keypress stuff
OKIGotIt, Ghor - Spice
rob - help with hooks
Minicooper237 - Projectile inaccuracy formula
Enigma - Equipment Drone Fix