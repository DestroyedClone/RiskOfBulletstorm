# Risk of Bulletstorm
A mod for Risk of Rain 2 that plans to add custom content, modified from Enter The Gungeon. Very configurable.

Likely not going to be updated.
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


***
# Items
## Pickups
Every 25 kills *(+10% per stage)*, the game will have a 40% chance to *spawn a random weighted pickup*. "Forgive Me, Please" will not trigger this. If it does, tell me because it's not supposed to.

-
|Icon| Chance |Item | Description |
|:--:|:--:|:--:|--|
|![](=)| 10% | Armor | Destroys itself to block one hit of damage dealing more than 20% health or deals fatal damage, and fires a Blank.
|![](=)| 35% | Blank| Press T to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use.
|![](=)| 60% | Spread Ammo|Pick up to restore *all* allies' cooldowns and restores one equipment charge to all equipment.
-

## White

|Icon| Item | Description |
|:--:|:--:|--|
|![](=)|Cartographer's Ring|Upon starting the next stage, 10% chance (+5% chance per stack) of automatically revealing all interactables. Counts based off all players' item counts. Stacking beyond 100% has no effect.<br>!! *I recommend installing my other mod, ["HideScannerIndicatorOnUse"](https://thunderstore.io/package/DestroyedClone/HideScannerIndicatorOnUse/), so that the indicator that pops over interactables disappears upon interaction.* !!</br>
|![](=)|Mustache|Upon purchase, increases your regeneration by `2 health` `(+2 per stack)` for `10` seconds.
|![](=)|Scope|Reduces spread by -10% (-5% per stack). <br>*You can enable all projectiles, but by default it follows a whitelist. Feel free to recommend me other projectiles to add to the whitelist, including any modded projectiles.*</br>

## Green
|Icon| Item | Description |
|:--:|:--:|--|
|![](=)|Disarming Personality|*Hyperbolically* reduces purchase costs by 10% (-5% per stack) up to 60%. Chance is shared among players.
|![](=)|Enraging Photo|Gain a temporary `100% damage bonus` that can last up to 2 seconds `(+0.5 seconds duration per stack).` The duration scales from the amount of damage taken, from `1% to 10%`.
|![](=)|Roll Bomb| Using your utility `drops 1 bomb` for `200% damage`.<br>`(+1 bomb dropped per stack)`</br>

## Red
|Icon| Item | Description |
|:--:|:--:|--|
|![](=)|Backpack|Grants 1 extra equipment slot per stack.<br> - Hold [Left Shift] + [NumberRow keys] to switch between slots.</br><br> - Press [Left Shift] + [=] to list all equipment.</br><br> - Press [F] or [G] to cycle left/right through equipment.</br><br> - Adds onto MULT's additional slot. *(MULT w/ 1 backpack = 3 slots, though you can't use retool to switch to the modded ones)*</br>
|![](=)|Number 2|Increases your base stats for every dead player.<br>*Singleplayer: Starts with one player dead.*</br>
 - 
| Stat | Bonus 
|--:|--|
| baseAttackSpeedAdd | 0.25
| baseDamageAdd | 2
| baseHealthAdd | 25
| baseMoveSpeedAdd | 1
| baseRegenAdd | 0.5
| armorAdd | 2
| critAdd | 5
 - 
|Icon| Unity | Description |
 - **Unity**: +0.4 (+0.05 per stack) base damage per unique item in inventory.

## Lunar
 - **Metronome**: Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150% (75 kills). Gain 25 extra stacks per pickup. Lose 75 stacks upon using a different skill. Shows a buff for tracking.
 - **Ring of Miserly Protection**: Grants +125% (+75% per stack) increased maximum health ...but one shatters upon using a shrine (does not include combat).

## Boss
 - **Master Round**: +10% max health per stack. Granted upon clearing a teleporter boss, as long as no player took more than 3 hits (scales with stages cleared) each. 

# Equipment
## Normal
- 
|Icon| Item | Description | CD |
|:--:|:--:|--|:--:|
| ![](=) | **Bomb**| Throws a bomb for `300% damage`. | 14s |
| ![](=) | **Charm Horn**| Upon use, blows the horn to `Charm` enemies within `20 meters` for 10 seconds. | 85s |
| ![](=) | **Friendship Cookie** | Revives all players and gives 3 seconds of immunity. If in singleplayer, gives an Infusion instead. Consumed on use. | 0s |
| ![](=) | **Magazine Rack**| Place to create a zone of no cooldowns within a radius of 3.5m for 2.5 seconds. | 14s |
| ![](=) | **Meatbun** | Heals the player for 33% health. Grants +10% damage until the player takes more than 5% damage. Buff stacks up to 5 times. | 90s |
| ![](=) | **Medkit** | Heals the player for 75% max health and restores 50% barrier. | 55s |
| ![](=) | **Molotov** | Throw to cover an area in flames for 10% damage per second for 8 seconds. | 55s |
| ![](=) | **Orange** | Consuming permanently reduces equipment recharge rate by 10%, increases health by 10%, fully heals the player, but is removed from the inventory. | 0s |
| ![](=) | **Portable Barrel Device** | Places a barrel, but doesn't give any money or experience for picking it up. | 30s |
| ![](=) | **Ration** | Heals for 40% health. Automatically used upon taking fatal damage. Consumed on use. | 0s |
| ![](=) |  |  | s |
- 
## Lunar
 - **Spice**: Increases and decreases stats per consumed Spice.  It's been slightly adjusted from Gungeon's values. These values are constant for each stack, not additive, except for the 6+ stack.
 - 
| Spice Consumed | Health Multiplier | Attack Speed | Accuracy | Enemy Bullet Speed | Damage | Curse
|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| 1 | +25% | +20% | +0%  | +0%   | +0%   | +0.5
| 2 | +50% | +40% | +25% | +0%   | +0%   | +1
| 3 | +25% | +40% | +25% | -10%  | +20%  | +1
| 4 | +0%  | +40% | +15% | -15%  | +35% | +1 
| 5 | 5 curse | +40% | -10% | -15%  | +50% | +1
| 6+ <br>(additive)</br> | +5 curse | +0% | -10% | -0%  | +15% | +1
 - 
 - Consuming beyond 5 will continue to add the bonuses from 6+. It's not exact, but the health reduction is added via PermanentCurse. Maximum health reduced by curse is reduced by a factor of `1 + 0.01 * n`, where `n` is the number of stacks.

## Special
 - **Curse** - Certain items apply curse. The chance of encountering a Jammed enemy increases as you gain more Curse. This is independent of elite affixes.
 - 
| Curse Count | Enemy Chance | Boss Chance |
|:--:|:--:|:--:|
| 0 | 0% | 0% |
| 0.5 - 2.0 | 1% | 0% |
| 2.5 - 4.0 | 2% | 0% |
| 4.5 - 6.0 | 5% | 0% |
| 6.5 - 8.0 | 10% | 20% |
| 8.5 - 9.5 | 25% | 30% |
| 10+ | 50% | 50% |
-
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
	* Curse Amount shown in inventory
	* New Jammed Enemy visuals
* Pickups
	* Buffed rates and added indicator
* Banned the following equipment from being used on equipment drones:
	* Bomb, Portable Barrel Equipment, Molotov
* Buffed overall stats:
	* Bomb, Roll Bomb, Number2
* Probably the last update

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
