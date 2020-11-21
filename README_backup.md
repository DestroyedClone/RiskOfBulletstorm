# Risk of Bulletstorm

A mod for Risk of Rain 2. Built with BepInEx and R2API and TILER2

Plans to add a bunch of custom items, modified from Enter the Gungeon.

# Dependencies

ThinkIvis - TILER2
Jarlyk - EliteSpawningOverhaul

# Items
TODO:
1. Assets (DisplayRules, Sound Effects)
2. Soft BetterUI Compatibility
3. Soft Dependencies for other mods
3a. Equipment: ThinkInvis's Classic Items: Beating Embryo
3b. Mimic-ites: Hailstorm: Mimics
4. Multiplayer Testing

# Items

## Special Items

[unimplemented] [Boss] Old Crest : Identical to Armor, takes priority. Using it on a specific interactable will do something...

## White


[nonfunctional] Irradiated Lead: 5% chance per stack to Poison.

## Green

+1 Bullets: +25% damage dealt (+5% per stack)

[unimplemented] Battle Standard: Increases damage of non-player allies. Does not apply to turrets.

[unimplemented] Ghost Bullets: Projectiles and Bullets pierce 1 (+1 per stack) enemies but reduces subsequent damage by 50% per enemy.

Green Guon Stone: Upon taking 33% damage, has a 10% chance to heal you instead. Rises to 25% if fatal.
Heals for 50 health + 25 per stack.
=> TODO: Add Health Check

Macho Brace: Gain +30% (+10% per stack) damage bonus upon using your utility. Damage bonus is used up when attacking.
=> TODO: Add check for CombatSkill, add check for equipment use

Ring of Chest Friendship: Increases credits of interactables by +10% (+2% per stack)

[nonfunctional] Rocket-Powered Bullets: Increases projectile speed by 50% (+5% per stack).

Scouter: Shows enemy health and damage in chat on hit to a pinged enemy. Only shows number of digits depending on InventoryCount. DamageType only shows up after 2 Scouters.

## Red

[unimplemented] Ring of Triggers: Using an equipment fires your primary attack in all directions for 3 (+1 per stack) seconds

Ring of Fire Resistance: Prevents being inflicted with fire damage.
Replaced with a random red if you already have one.

## Lunar

[unimplemented] Teaching of the Dodge Roll: Replaces the player's Utility with "Dodge Roll". Removed from item pool upon possession.
Picking one up with one equipped will roll a random lunar.
If you already have a Strides of Heresy, it will refund you Lunar Coins equal to your item stack, then give the item.

[unimplemented] "Dodge Roll" Ability: Roll forward. Immunity to damage upon rolling

[unimplemented] Bloodied Scarf: Replaces your utility with Huntress Blink

Spice: Increases various stats, but increases the rate at which Spice spawns and increases Curse.
For now, it increases base damage by +1 per stack.
=> TODO: Add Stat bonuses. 

# Equipment

## Normal

Proximity Mine: Drops a mine for 100% damage. CD: 18

Smoke Bomb: Cloak for 6 seconds. CD: 36


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
[unimplemented] Kaliber's Wrath - Jammed {0}
Jammed Elites deal 400% ([unimplemented]+20% per stage) damage and have 400% health.
=> Currently only affects spawned Elites.


## Credits
# Repos

Crediting all the repos that I've peered at'

KomradeSpectre - AetheriumMod https://github.com/KomradeSpectre/AetheriumMod

ThinkInvis - ClassicItems https://github.com/ThinkInvis/RoR2-ClassicItems

Chen - Chen's ClassicItems https://github.com/cheeeeeeeeeen/RoR2-ClassicItems

Rico - AffixGen https://github.com/RicoValdezio/RoR2-AffixGen/blob/master/Equipment/AffixEquipBehaviour.cs (Equipment Removal)

# Non-standard

Crediting people I've actually talked to'

Rein - Nonspecific, DirectorAPI

KomradeSpectre, Chen - Lots, and TILER2 Help

KomradeSpectre - Walking me through the resource process

Spice - OKIGotIt, Ghor

rob - help with hooks

##Credits - Other
https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
https://answers.unity.com/questions/285785/how-to-randomly-pick-a-string-from-an-array.html
https://stackoverflow.com/questions/2893297/iterate-multi-dimensional-array-with-nested-foreach-statement
https://stackoverflow.com/questions/6413572/how-do-i-get-the-last-four-characters-from-a-string-in-c
https://stackoverflow.com/questions/411752/best-way-to-repeat-a-character-in-c-sharp
