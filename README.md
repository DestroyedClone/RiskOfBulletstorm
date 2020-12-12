

# Risk of Bulletstorm
A mod for Risk of Rain 2 that plans to add custom content, modified from Enter The Gungeon. Currently Multiplayer Untested.

Feel free to message me in the Risk of Rain 2 Modding discord for any broken junk or suggestions.
***
# Dependencies
 - BepInEx and R2API
 - ThinkInvis - TILER2

# Compatibility with other mods
 1. **Classic-Items** : Beating Embryo Support
 2. **Bandit Reloaded** : Item Displays
 3. **HAND** : Item Displays

# Planned
 * (Boss) Dog: Chance to find pickups
***
1. BetterUI, ItemStatsMod Support
2. Multiplayer Testing

***
# Items
## Pickups
Every 30 kills *(+50% per stage)*, the game will have a 30% chance to *spawn a random weighted pickup*. "Forgive Me, Please" will not trigger this.
 - **Armor**: *[10% chance]* Destroys itself to block one hit of damage dealing more than 20% health or fatal, and fires a Blank. (TODO: SFX)
 - **Blank**: *[25% chance]* Press T to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use. (TODO: SFX, Visual effect)
 - **Key**: *[15% chance]* Opens a locked chest for free. Consumed on use.
 - **Spread Ammo**: *[30% chance]* Pick up to restore *all* players' cooldowns and restores one equipment charge to all equipment.

## White
 - **Battery Bullets**: 1% chance *(+1% per stack, up to 15%)* for your attacks to stun enemies. Affected by proc coefficients.
 - **Cartographer's Ring**: Upon starting the next stage, 20% chance (+10% chance per stack) of automatically revealing all chests.
 - **Mustache**: Heal for +10% (+10% per stack) health upon purchasing something.

## Green
 - **Disarming Personality**: *Hyperbolically* reduces chest prices by 10% (-5% per stack) up to 60%.
 - **Enraging Photo**: Gain a temporary +100% damage boost upon taking 11% of your health in damage for 1 (+0.25 per stack) seconds.
 - **Live Ammo**: Using your Utility creates a 7m explosion for 100% *(+50%  per stack)* damage that propels you by 100% *(+50% per stack)*
	 - *Note: I couldn't do contact damage adjustments at the time, so I opted to add this custom effect. Will likely be changed or removed.*
 - **Roll Bomb**: After using your Utility, drop 1 bomb(s) (+1 per stack) for 100% damage.
- **Scope**: Reduces bullet spread by -10% (-5% per stack). 
	- There is a projectile whitelist to prevent breaking.
	- You can disable the whitelist to affect all projectiles, but note this may break some functionality with other projectiles. Feel free to recommend me other projectiles to add to the whitelist, including any modded projectiles.

## Red
 - **Number 2**: Boosts your base damage and movement speed by 6 for every dead survivor. Currently doesn't stack.
	 - *Singleplayer: Effect acts like one player is dead.*
 - **Orbital Bullets**: [Experimental] Projectiles that hit a wall will instead orbit around the player. Max of 5 projectiles, +1 per stack.
 - **Ring of Triggers**: [Experimental] Using your equipment will fire projectiles/bullets/melee attacks from your primary skill in a circle for 3 seconds (+1 second per stack). 
 - **Unity**: +0.1 (+0.01 per stack) base damage per unique item in inventory.

## Lunar
 - **Metronome**: Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150%. Gain 50 extra stacks per pickup. Lose 25 stacks upon using a different skill. Shows a buff for tracking.
 - **Ring of Miserly Protection**: Grants +100%(+50% per stack) increased maximum health ...but one shatters upon using a shrine (does not include combat).

## Boss
 - **Master Round**: Granted upon clearing a teleporter boss, as long as no player took more than 3 hits (scales with stages cleared) each. Adds 150 max health per stack.

# Equipment
## Normal
*[Currently Broken, equipment works fine otherwise] Beating Embryo from ThinkInvis's Classic Items has support, listed as BEmbryo. If you don't have it you can just ignore the line.*

 - **Bomb**: Throws a bomb for 100% damage. CD: 14s
	 - *BEmbryo: Throws an extra bomb.*
 - **Charm Horn**: Blowing this horn [Charms] all enemies within a radius of 20 meters for 10 seconds.
	 - *BEmbryo: Increases radius by 50%*
 - **Friendship Cookie**: Revives all players and gives 3 seconds of immunity. If in singleplayer, gives an Infusion instead. Consumed on use. 
	 - *BEmbryo: Extends immunity duration to 9 seconds. Singleplayer: Gives one more Infusion.*
 - **Magazine Rack**: Place to create a zone of no cooldowns within a radius of 3.5m for 2.5 seconds. CD: 90s
	 - *BEmbryo: Range increases by 150% and duration lasts 20% longer.* 
- **Meatbun**: Heals the player for 33% health. Grants +10% damage until the player takes more than 5% damage. Buff stacks up to 5 times. CD: 90s
	 - *BEmbryo: Heals again for an additional 33% health.*
- **Medkit**: Heals the player for 75% max health and restores 50% barrier. CD: 100s
	 - *BEmbryo: Activates twice.*
- **Molotov**: Throw to cover an area in flames for 10% damage per second for 8 seconds. 
	 - *BEmbryo: Molotov deals 2x more damage.*
 - **Orange**: -20% chance to spawn. Consuming permanently reduces equipment recharge rate by 10%, increases health by 10%, fully heals the player, but is removed from the inventory.
	 - *BEmbryo: Activates twice.*
 - **Portable Barrel Device**: Places a barrel, but doesn't give any money or experience for picking it up.
	 - *BEmbryo: Places two barrels.*
 - **Ration**: Heals for 40% health. Automatically used upon taking fatal damage. Consumed on use.
	 - *BEmbryo: Heals twice.*
 - **Trusty Lockpicks**: 50% chance to unlock a chest. If it fails, doubles its price and prevents further picklocking or using Keys.
	 - *BEmbryo: Adds 30% of your base chance to unlock. By default, raises your unlock chance to 65%.*
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
- *BEmbryo:* No support at the moment.

## Special
 - **Curse** - Certain items apply curse. The chance of encountering a Jammed enemy increases as you gain more Curse. This is independent of elite affixes.
## Buffs
 - **[Enraged]**: +100% damage.

---AI Specific:---
 - **[Charmed]**: Switches to the opposite team and fights the former team. Can be damaged by both sides.
 - **[Jammed]**: +100% damage, +100% crit chance, +20% attack speed, +20% movement speed. 

# Current Issues:
* Master Round isn't given if the teleporter event was forcefully completed (such as with RORCheats)
* 

# Changelog
1.1.0 - Content Update:
 -  *Added---* 
	 - Pickups: Armor, Blank, Key, and Spread Ammo
	 - Whites: Cartographer's Ring, Battery Bullets
	 - Greens: Disarming Personality, Enraging Photo, Roll Bomb, Live Ammo
	 - Reds: Number 2, Orbital Bullets, Ring of Triggers, Unity
	 - Lunars: Ring of Miserly Protection
	 - Boss: Master Round
	 - Equipment: Charm Horn, Friendship Cookie, Magazine Rack, Meatbun, Medkit, Molotov, Orange, Portable ~~Table~~ Barrel Device, Ration, Trusty Lockpicks
	 - Lunar Equipment: Spice
 - *Mustache---* Added config for interaction with Blood Shrines.
 - *Scope---* now affects ALOT of projectiles but not all. Can affect all projectiles via config (strongly discouraged).
 - *Metronome---* Added config for stack loss. Added stack display via buff count.
 - *Bomb---* Added assets, increased cooldown
 - *EliteSpawningOverhaul---* Temporarily removed dependency due to changing how Jammed is applied. Deciding whether to keep or not.

1.0.1 - Reverted dependency of R2API. Adjusted item descriptions.

1.0.0 - Released!

## Credits
Rico - Equipment Removal
Rein - Nonspecific, DirectorAPI
KomradeSpectre, Chen - Lots, and TILER2 Help
OKIGotIt, Ghor - Spice
rob - help with hooks
