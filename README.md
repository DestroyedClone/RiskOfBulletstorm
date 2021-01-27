# Risk of Bulletstorm
A mod for Risk of Rain 2 that plans to add custom content, modified from Enter The Gungeon. Currently untested for multiplayer. I've added a few things here and there for networking, but any *actual* effects in multiplayer are unintended effects.

Feel free to message me in the Risk of Rain 2 Modding discord for any broken junk or suggestions.
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
* Some issues with item displays for scav and mithrix

# Planned
1. BetterUI, ItemStatsMod Support
2. Multiplayer Support
3. Jarlyk's Hailstorm support (Curse increases mimic chance)

***
# Items
## Pickups
Every 30 kills *(+50% per stage)*, the game will have a 30% chance to *spawn a random weighted pickup*. "Forgive Me, Please" will not trigger this. If it does, tell me because it's not supposed to.
 - **Armor**: *[10% chance]* Destroys itself to block one hit of damage dealing more than 20% health or fatal, and fires a Blank. (TODO: SFX)
 - **Blank**: *[25% chance]* Press T to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use. (TODO: SFX, New Visual effect)
 - **Spread Ammo**: *[30% chance]* Pick up to restore *all* allies' cooldowns and restores one equipment charge to all equipment.

## White
 - **Cartographer's Ring**: Upon starting the next stage, 20% chance (+10% chance per stack) of automatically revealing all chests. Counts based off all players' item counts. Stacking beyond has no effect.
 - **Mustache**: Heal for +10% (+10% per stack) health upon purchase. If sacrifice is enabled, you will heal upon pickup of an item or equipment.

## Green
 - **Disarming Personality**: *Hyperbolically* reduces chest prices by 10% (-5% per stack) up to 60%.
 - **Enraging Photo**: Gain a temporary +100% damage boost upon taking 11% of your health in damage for 1 (+0.25 per stack) seconds.
 - **Roll Bomb**: After using your Utility, drop 1 bomb(s) (+1 per stack) for 100% damage.
- **Scope**: Reduces spread by -10% (-5% per stack). 
	- There is a projectile whitelist to prevent breaking.
	- You can disable the whitelist to affect all projectiles, but note this may break some functionality with other projectiles. Feel free to recommend me other projectiles to add to the whitelist, including any modded projectiles.

## Red
 - **Backpack**: Grants 1 extra equipment slot per stack.
	 - Hold [Left Shift] + [NumberRow keys] to switch between slots.
	 - Press [Left Shift] + [=] to list all equipment.
	 - Press [F] or [G] to cycle left/right through equipment.
	 - Adds onto MULT's additional slot. (MULT w/ 1 backpack = 3 slots, though you can't use retool to switch to the modded ones)
 - **Number 2**: Increases your base stats by 4 (+2 per stack) for every dead player.
	 - *Singleplayer: Starts with one player dead. Stacking halved.*
	 - Adds to: baseAttackSpeed, baseDamage, baseHealth, baseMoveSpeed, baseRegen, armor, and crit.
 - **Unity**: +0.1 (+0.01 per stack) base damage per unique item in inventory.

## Lunar
 - **Metronome**: Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150% (75 kills). Gain 50 extra stacks per pickup. Lose 25 stacks upon using a different skill. Shows a buff for tracking.
 - **Ring of Miserly Protection**: Grants +125%(+75% per stack) increased maximum health ...but one shatters upon using a shrine (does not include combat).

## Boss
 - **Master Round**: Granted upon clearing a teleporter boss, as long as no player took more than 3 hits (scales with stages cleared) each. Adds 150 max health per stack.

# Equipment
## Normal

 - **Bomb**: Throws a bomb for 100% damage. CD: 14s
 - **Charm Horn**: Blowing this horn [Charms] all enemies within a radius of 20 meters for 10 seconds.
 - **Friendship Cookie**: Revives all players and gives 3 seconds of immunity. If in singleplayer, gives an Infusion instead. Consumed on use. 
 - **Magazine Rack**: Place to create a zone of no cooldowns within a radius of 3.5m for 2.5 seconds. CD: 90s
- **Meatbun**: Heals the player for 33% health. Grants +10% damage until the player takes more than 5% damage. Buff stacks up to 5 times. CD: 90s
- **Medkit**: Heals the player for 75% max health and restores 50% barrier. CD: 100s
- **Molotov**: Throw to cover an area in flames for 10% damage per second for 8 seconds. CD: 55s
 - **Orange**: -20% chance to spawn. Consuming permanently reduces equipment recharge rate by 10%, increases health by 10%, fully heals the player, but is removed from the inventory.
 - **Portable Barrel Device**: Places a barrel, but doesn't give any money or experience for picking it up.
 - **Ration**: Heals for 40% health. Automatically used upon taking fatal damage. Consumed on use.
## Lunar
 - **Spice**: Increases and decreases stats per consumed Spice.  It's been slightly adjusted from Gungeon's values. Bonuses:
 - 
| Spice Consumed | Health Multiplier | Attack Speed | Accuracy | Enemy Bullet Speed | Damage | Curse
|--|--|--|--|--|--|--|
| 1 | +25% | +20% |  |  |  | +0.5
| 2 | +25% | +20% | +25% |  |  | +1
| 3 | -25% |  |  | -10% |  | +1
| 4 | -25% |  |  | -5% | +20% | +1 
| 5+ | -5% |  | -10% |  | +15% | +1
 - 
 - Consuming beyond 5 will continue to add the same as the bonuses from 5. I hardcapped the health multiplier minimum at -99%.

## Special
 - **Curse** - Certain items apply curse. The chance of encountering a Jammed enemy increases as you gain more Curse. This is independent of elite affixes.
## Buffs
 - **[Enraged]**: +100% damage.

---AI Specific:---
 - **[Charmed]**: Switches to the opposite team and fights the former team. Can be damaged by anybody, including other charmed enemies, but will attempt to avoid the Charmer's team.
 - **[Jammed]**: +100% damage, +100% crit chance, +20% attack speed, +20% movement speed. 

# Changelog
1.1.0 - Content Update:
 -  *Added---* 
	 - Pickups: Armor, Blank, and Spread Ammo
	 - Whites: Cartographer's Ring
	 - Greens: Disarming Personality, Enraging Photo, Roll Bomb
	 - Reds: Backpack, Number 2, Unity
	 - Lunars: Ring of Miserly Protection
	 - Boss: Master Round
	 - Equipment: Charm Horn, Friendship Cookie, Magazine Rack, Meatbun, Medkit, Molotov, Orange, Portable ~~Table~~ Barrel Device, Ration, Trusty Lockpicks
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
