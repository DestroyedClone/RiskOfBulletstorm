# Risk of Bulletstorm
A mod for Risk of Rain 2 that plans to add custom content, modified from Enter The Gungeon. Currently Multiplayer untested.

There's a config, but I'm not sure if it works. Feel free to message me in the Risk of Rain 2 Modding discord for any broken junk or suggestions.
***
# Dependencies
BepInEx and R2API
ThinkIvis - TILER2
Jarlyk - EliteSpawningOverhaul

# Compatibility with other mods
1. Classic-Items : Beating Embryo Support
2. Bandit Reloaded : Item Displays
3. HAND : Item Displays

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
Shells?
Half + Full Heart: Restores 35%/60% health on pickup. Can fill barrier.
Armor: Destroys itself to block one hit of damage dealing more than 25% health or exceeding your health, and fires a Blank. (TODO: SFX)
Blank: Press a key to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use. (TODO: SFX, Visual effect)
Key: Opens a locked chest for free. Consumed on use.
Cell Key: ??? No?
Ray Key: ??? Maybe?
Ammo: Bandolier clone (X)
Spread Ammo: Bandolier clone that applies to all players

## White
Mustache: Heal for +10% (+10% per stack) health upon purchasing something.
Cartographer's Ring: Upon starting the next stage, 25% chance (+10% chance per stack) of automatically scanning the stage.

## Green
Scope: Reduces bullet spread by -10% (-5% per stack).
Enraging Photo: Gain a temporary +100% damage boost upon taking 33% of your health in damage for 1 (+0.25 per stack) seconds.
Disarming Personality: Reduces chest prices by -15% (-5% per stack)
Roll Bomb: After using your Utility, drop 1 bomb(s) (+1 per stack) for 80% damage.

## Red
Unity: +0.1 (+0.05 per stack) base damage per unique item in inventory
Number 2: Boosts your damage when you're the only survivor/team:player alive.'

## Lunar
Metronome: Gain a 2% damage bonus for every enemy you kill with the same skill, up to 150%. Gain 50 extra stacks per pickup. Lose 25 stacks upon using a different skill.
Ring of Miserly Protection: Grants +50%(+50% per stack) increased maximum health, ...but one shatters upon purchase.

## Boss
Beetle: Spawns 1 beetle that have a 5% (+5% per stack) chance to dig up a pickup upon completing a room.
Master Round: Granted upon clearing a teleporter boss without ANYONE taking damage. Adds 150 max health per stack.

# Equipment
## Normal
Bomb: Throws a bomb for 100% damage. CD: 10s
Meatbun: Heals the player for 33% health. Grants double damage until the player takes more than 10% damage. CD: 2 * 45s = (90s)
Medkit: Fully heals the player and maxes out barrier. Cooldown 3x as long as foreign fruit
Orange: 50% less likely to be found in chests. Consuming permanently reduces recharge rate by 10% (hyperbolic), adds an infusion, fully heals the player, but is removed from the inventory.

Molotov: fire throw aoe mushrum
Supply Drop: Drops an equipment beacon
Friendship Cookie: Revives all survivors
Coolant Leak: Covers an area with water, dousing enemies in water.

# Elites
(Default: Disabled) Jammed Elites: +200% damage, +100% crit chance

## Credits
Rico - Equipment Removal
Rein - Nonspecific, DirectorAPI
KomradeSpectre, Chen - Lots, and TILER2 Help
KomradeSpectre - Walking me through the resource process
OKIGotIt, Ghor - Spice
rob - help with hooks

## Credits - Other
https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
https://answers.unity.com/questions/285785/how-to-randomly-pick-a-string-from-an-array.html
https://stackoverflow.com/questions/2893297/iterate-multi-dimensional-array-with-nested-foreach-statement
https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c
https://stackoverflow.com/questions/411752/best-way-to-repeat-a-character-in-c-sharp
