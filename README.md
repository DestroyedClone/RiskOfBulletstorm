
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
 * (Green) Military Training: Improves firing
 * (Red) Number 2: Boosts stats while alone
 * (Green) Live Ammo: Immunity to contact damage, hitbox to utility
 * (Green) Battery Bullets: Electrifies bullets
***
1. BetterUI, ItemStatsMod Support
2. Multiplayer Testing

***
# Items
## Pickups
Every 50 kills, the game will have a 20% chance to *spawn a random weighted pickup*.
 - **Armor**: Destroys itself to block one hit of damage dealing more than 20% health or fatal, and fires a Blank. (TODO: SFX)
 - **Blank**: Press T to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use. (TODO: SFX, Visual effect)
 - **Key**: Opens a locked chest for free. Consumed on use.
 - **Spread Ammo**: Spawns a bandolier pack that restores *all* players' cooldowns and restores one equipment charge to all equipment.

## White
 - **Cartographer's Ring**: Upon starting the next stage, 20% chance (+10% chance per stack) of automatically revealing all chests.
 - **Mustache**: Heal for +10% (+10% per stack) health upon purchasing something.

## Green
 - **Disarming Personality**: *Hyperbolically* reduces chest prices by 10% (-5% per stack) up to 60%.
 - **Enraging Photo**: Gain a temporary +100% damage boost upon taking 11% of your health in damage for 1 (+0.25 per stack) seconds.
 - **Roll Bomb**: After using your Utility, drop 1 bomb(s) (+1 per stack) for 80% damage.
- **Scope**: Reduces bullet spread by -10% (-5% per stack). 
	- Projectile Whitelist: Sawmerangs, REX's syringes, and Preon Accumulators. (config) Disposable Missile Launcher.
	- You can disable the whitelist to affect all projectiles, but note this may break some functionality with other projectiles.

## Red
 - **Unity**: +0.1 (+0.01 per stack) base damage per unique item in inventory
 - **Number 2**: Boosts your base damage and movement speed by 6 for every dead survivor. Currently doesn't stack. Acts as if one survivor is dead if in singleplayer.

## Lunar
 - **Metronome**: Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150%. Gain 50 extra stacks per pickup. Lose 25 stacks upon using a different skill.
 - **Ring of Miserly Protection**: Grants +100%(+50% per stack) increased maximum health, ...but one shatters upon using a shrine (does not include combat).

## Boss
 - **Beetle**: Spawns 1 beetle that have a 5% (+5% per stack) chance to dig up a pickup upon completing a room.
 - **Master Round**: Granted upon clearing a teleporter boss, as long as no player took more than 3 hits each. Adds 150 max health per stack.

# Equipment
## Normal
*Beating Embryo from ThinkInvis's Classic Items has support, listed as BEmbryo:*

 - **Bomb**: Throws a bomb for 100% damage. CD: 14s
	 - *BEmbryo: Throws an extra bomb.*
 - **Friendship Cookie**: Revives all players. If in singleplayer, gives an Infusion instead. Consumed on use. 
	 - *BEmbryo: Fills the barrier of revives players. Singleplayer: Gives one more Infusion.*
- **Meatbun**: Heals the player for 33% health. Grants +45% damage until the player takes more than 5% damage. Buff stacks up to 5. CD: 90s
	 - *BEmbryo: Heals again for an additional 33% health.*
- **Medkit**: Fully heals the player and maxes out barrier. CD: 145s
	 - *BEmbryo: Activates twice.*
- **Molotov**: Throw to cover an area in flames for 10% of your damage every 1/60th of a second for 8 seconds. 
	 - *BEmbryo: Molotov deals 10% more damage.*
 - **Orange**: 80% chance to spawn (or 20% rarer). Consuming permanently reduces equipment recharge rate by 10%, increases health by 10%, fully heals the player, but is removed from the inventory.
	 - *BEmbryo: Activates twice.*
 - **Portable Barrel Device**: Places a barrel. 
	 - *BEmbryo: Places *two* barrels.*
 - **Ration**: Heals for 40% health. Automatically used upon taking fatal damage. Consumed on use.
	 - *BEmbryo: Heals twice.*
 - **Trusty Lockpicks**: 50% chance to unlock a chest. If it fails, doubles its price and prevents further picklocking or using Keys.
	 - *BEmbryo: Adds 30% of your base chance to unlock. By default, raises your unlock chance to 65%.*

# Special
 - **Curse** - Certain items have apply curse. The chance of encountering a Jammed enemy increases as you gain more Curse. This is independent of elite affixes.
 - **[Jammed]**: +100% damage, +20% attack speed, +20% movement speed.

# Current Issues:
* (?) Trusty Lockpicks unlocks don't trigger Mustache.
* (Medium) No way to get curse

# Changelog
1.1.0 - Content Update:
 -  *Added---* 
	 - Pickups: Armor, Blank, Key, and Spread Ammo, 	
	 - Whites: Cartographer's Ring,
	 - Greens: Disarming Personality, Enraging Photo, Roll Bomb
	 - Reds: Unity, Number 2
	 - Lunars: Ring of Miserly Protection
	 - Boss: Master Round
	 - Equipment: Friendsip Cookie, Meatbun, Medkit, Molotov, Orange, Portable ~~Table~~ Barrel Device, Ration, Trusty Lockpicks
 - *Mustache---* Added config for interaction with Blood Shrines.
 - *Scope---* now affects REX syringes, Sawmerang, Preon Accumulator, and (config) Disposable Missile Launcher. Added experimental config for applying it to all projectiles (may break some projectiles).
 - *Metronome---* Added config for stack loss.
 - *Bomb---* Added assets, increased cooldown
 - *EliteSpawningOverhaul)---* Temporarily removed dependency. Deciding whether to keep or not.

1.0.1 - Reverted dependency of R2API. Adjusted item descriptions.

1.0.0 - Released!

## Credits
Rico - Equipment Removal
Rein - Nonspecific, DirectorAPI
KomradeSpectre, Chen - Lots, and TILER2 Help
KomradeSpectre - Walking me through the resource process
OKIGotIt, Ghor - Spice
rob - help with hooks
