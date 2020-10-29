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
3a. Equipment: ThinkInvis Beating Embryo

#[Items]

##[Special Items]
(WIP) [White] Armor: Blocks one hit of damage, then get destroyed.

(WIP) [White] Blank: Press a key to activate, destroying all projectiles, stunning and knocking nearby enemies back. Consumed on use.

##[White]

##[Green]
+1 Bullets: +10% base damage (+1% per stack)

Enraging Photo: Gain a temporary +100% damage boost  upon taking 33% of your health in damage for 3 (+0.25 per stack) seconds.
=> TODO: Health Requirement Loss

Macho Brace: Gain +30% (+10% per stack) damage bonus upon using your utility.
=> TODO: Add Stacking Bonus

[unimplemented] Green Guon Stone: Upon taking 33% damage, has a 20% chance to heal you instead. Rises to 50% if fatal.
=> Add Health Check

Roll Bomb: After using your Utility, drop 1 bomb(s) (+1 per stack) for 80% damage.

##[Red]

##[Lunar]
[unimplemented] Bloodied Scarf: Replaces your utility with Huntress Blink

[unimplemented] Metronome: Gain a damage bonus for every enemy you kill, clears when you use a different skill.
=> \+2% damage bonus for each enemy killed, maxes at +150%(+50%). The bonus is reset if the player uses a different skill.

#[Equipment]
##[Normal]
Bomb: Throws a bomb for 100% damage. CD: 8

Proximity Mine: Drops a mine for 100% damage. CD: 18

Smoke Bomb: Cloak for 6 seconds. CD: 36

Ticket: Spawns a Clay Templar. CD: 120

[Unimplemented] Trusty Lockpicks: 50% chance to unlock chest. If it fails, the chest is destroyed.


##[Lunar]
[Unimplemented] Drill: Unlock a chest and summon a combat-shrine worth of enemies depending on the chest.
	Normal: 1 
	Green: 2
	Red: 4
	


#[Curse]
[unimplemented] Picking up RiskOfBulletstorm items that defy Kaliber adds +0.25 curse. Each subsequent stack of a single item will add +0.1.
Curse increases the chance of an enemy becoming Jammed.

#[Elites]
Kaliber's Wrath - Jammed {0}
Jammed Elites deal 400% ([unimplemented]+20% per stage) damage and have 400% health.
=> Currently only affects spawned Elites.


## Credits
Nearly all the code either directly references or is directly copied from and modified. Including this readme:

KomradeSpectre - AetheriumMod https://github.com/KomradeSpectre/AetheriumMod

ThinkInvis - ClassicItems https://github.com/ThinkInvis/RoR2-ClassicItems

Chen - Chen's ClassicItems https://github.com/cheeeeeeeeeen/RoR2-ClassicItems

RyanPallensen - FloodWarning 