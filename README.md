# Risk of Bulletstorm

A mod for Risk of Rain 2. Built with BepInEx and R2API and TILER2

Plans to add a bunch of custom items, modified from Enter the Gungeon.

# Dependencies

ThinkIvis - TILER2
Jarlyk - EliteSpawningOverhaul

# Items
No assets currently.

#[Items]
##[White]

##[Green]
+1 Bullets: +25% damage (+5% per stack)

[Green] Enraging Photo: Gain a temporary +100% damage boost (+50% per stack) upon taking 12% of your health in damage for 3 seconds.
=> TODO: Health Requirement Loss
=> TODO: Set Tier to Green
=> BetterUI Compat

[unimplemented] Green Guon Stone: Upon taking damage has a chance to heal you instead. 

Roll Bomb: After using your Utility, drop 1 bomb(s) (+1 per stack) for 80% damage.

##[Red]

##[Lunar]
[unimplemented] Bloodied Scarf: Replaces your utility with Huntress Blink

[unimplemented] Metronome: Gain a damage bonus for every enemy you kill, clears when you use a different skill.
=> \+2% damage bonus for each enemy killed, maxes at +150%(+50%). The bonus is reset if the player uses a different skill.

#[Special Items]
[White] Armor: Blocks one hit of damage, then get destroyed.
=> TODO: Add health requirement

[White] Blank: Press a key to activate, destroying all projectiles, stunning and knocking nearby enemies back.


#[Equipment]
[Standard] Bomb: Throws a bomb for 80% damage.

[Standard] Proximity Mine: Drops a mine for 100% damage.

[Standard] Ticket: Spawns a Clay Templar with +100% health and +50% damage. 
=> Scaling doesn't work


[Lunar]


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