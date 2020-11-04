# Risk of Bulletstorm

A mod for Risk of Rain 2. Built with BepInEx and R2API and TILER2

Plans to add a bunch of custom items, modified from Enter the Gungeon.

# Dependencies

ThinkIvis - TILER2
Jarlyk - EliteSpawningOverhaul

# Items
TODO:
1. Assets (Icons, Models, DisplayRules, Sound Effects)
2. Soft BetterUI Compatibility
3. Soft Dependencies for other mods
3a. Equipment: ThinkInvis's Classic Items: Beating Embryo
3b. Mimic-ites: Hailstorm: Mimics
4. Multiplayer Testing

# Items

## Special Items

[White] Armor: Blocks one hit of damage if it dealt more than 25% health or would have killed you, then gets destroyed.

[White] Blank: Press a key to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use.

## White

## Green

+1 Bullets: +10% base damage (+1% per stack)

[unimplemented] Battle Standard: 

Enraging Photo: Gain a temporary +100% damage boost upon taking 33% of your health in damage for 3 (+0.25 per stack) seconds.
=> TODO: Health Requirement Loss

Green Guon Stone: Upon taking 33% damage, has a 10% chance to heal you instead. Rises to 25% if fatal.
Heals for 50 health + 25 per stack.
=> TODO: Add Health Check

Macho Brace: Gain +30% (+10% per stack) damage bonus upon using your utility.

Ring of Chest Friendship: Increases chance of chest spawning by 10% (+1% per stack)

Roll Bomb: After using your Utility, drop 1 bomb(s) (+1 per stack) for 80% damage.

## Red

[unimplemented] Ring of Triggers: Using an equipment fires your primary attack in all directions for 3 (+1 per stack) seconds

Ring of Fire Resistance: Prevents being inflicted with fire damage.
Replaced with a random red if you already have one.

Unity: +0.1 (0.05 per stack) damage per unique item in inventory

## Boss

(WIP) Dog: [unimplemented]Spawns 1 (+1 per stack) dog(s) that have a 5% chance to dig up a pickup upon completing a room.
			Spawns a pettable beetle. (untested)

[unimplemented] Master Round: Granted upon clearing a teleporter boss without taking damage. Adds +33% max health.


## Lunar

[unimplemented] Teaching of the Dodge Roll: Replaces the player's Utility with "Dodge Roll". Removed from item pool upon possession.
Picking one up with one equipped will roll a random lunar.
Unknown Compatibility with Strides of Heresy.

"Dodge Roll" Ability: Roll forward. Immunity to damage upon rolling

[unimplemented] Bloodied Scarf: Replaces your utility with Huntress Blink

[unimplemented] Metronome: Gain a damage bonus for every enemy you kill, clears when you use a different skill.
=> +2% damage bonus for each enemy killed, maxes at +150%(+50%). The bonus is reset if the player uses a different skill.

Ring of Miserly Protection: Grants +50%(+50% per stack) increased maximum health, ...but all stacks disappear upon purchasing.

Spice: Increases various stats, but increases the rate at which Spice spawns and increases Curse.
=> TODO: Add Stat bonuses. 

# Equipment

## Normal

Bomb: Throws a bomb for 100% damage. CD: 8

Portable Turret: Spawns a EngiTurret. CD: 75

Proximity Mine: Drops a mine for 100% damage. CD: 18

Smoke Bomb: Cloak for 6 seconds. CD: 36

Ticket: Spawns a Clay Templar. CD: 120

[Unimplemented] Trusty Lockpicks: 50% chance to unlock chest. If it fails, the chest is permanently locked.


## Lunar
[Unimplemented] Drill: Unlock a chest and summon a combat-shrine worth of enemies depending on the chest.
	Normal: 1 
	Green: 2
	Red: 4
	


# Curse
[unimplemented] Picking up RiskOfBulletstorm items that defy Kaliber adds +0.25 curse. Each subsequent stack of a single item will add +0.1.
Curse increases the chance of an enemy becoming Jammed.
At 10 Curse (configurable), the Lord of the Jammed will spawn. Currently spawns an invincible BrotherGlass that's invulnerable.'

# Elites
Kaliber's Wrath - Jammed {0}
Jammed Elites deal 400% ([unimplemented]+20% per stage) damage and have 400% health.
=> Currently only affects spawned Elites.


## Credits
# Repos

Crediting all the repos that I've peered at'

KomradeSpectre - AetheriumMod https://github.com/KomradeSpectre/AetheriumMod

ThinkInvis - ClassicItems https://github.com/ThinkInvis/RoR2-ClassicItems

Chen - Chen's ClassicItems https://github.com/cheeeeeeeeeen/RoR2-ClassicItems

RyanPallensen - FloodWarning 

Harb - HarbCrate

ontrigger - ItemStatsMod https://github.com/ontrigger/ItemStatsMod/blob/4c015d5f54b5f7df788236b7c8546f7cc72743fe/ItemStats/src/Hooks.cs (UI)

# Non-standard

Crediting people I've actually talked to'

Rein - Nonspecific

KomradeSpectre, Chen - Lots, and TILER2 Help

Spice - OKIGotIt, Ghor

rob - help with hooks

##Credits - Other
https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
https://answers.unity.com/questions/285785/how-to-randomly-pick-a-string-from-an-array.html
https://stackoverflow.com/questions/2893297/iterate-multi-dimensional-array-with-nested-foreach-statement
https://answers.unity.com/questions/285785/how-to-randomly-pick-a-string-from-an-array.html