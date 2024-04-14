* `1.2.2`
	* Disabled some items and equipment for quality
	* ! New Items
		* Uncommon
			* Alpha Bullets, Battle Standard, Coin Crown, 
		* Lunar
			* Ring of Miserly Protection
		* Void Uncommon
			* Leader Standard
		* Void Legendary
			* Prototype
	* ! New Equipment:
		* Normal
			* Orange
		* Lunar
			* Iron Coin, Trusty Lockpicks, Drill
	* ! Artifacts
		* Adaptive Armor, Pit Lord, Swift Post-Battle
	* ! Tweaks
		* Center Pickup Text Config setting
		* Added Stealing
		* No Auto Pickup
	* ! Enemies
		* Added chance to spawn a wisp from breaking chests

<details> <summary>Previous Changelog</summary>

* `1.2.1`
	* Compat:
		* BetterUI
		* ItemStats
	* New:
		* [Lunar Item] Ring of Miserly Protection: Grants increased health but destroys on Chance Shrines.
		* [Lunar Equipment] Drill: Opens chests but summons enemies depending on chest quality. Can't unlock failed Trusty Lockpick chests.
		* [Lunar Item] Bloodied Scarf: Replaces your Utility with a teleport.
		* [Lunar Item] Teachings of Dodge Roll: Replaces your Utility with a roll.
	* Change:
		* Artifact of the Pit Lord: Removed config, effect now always applies for everyone.
		* Added subcooldown timer of 2.5 seconds for iBomb Companion Device
* `1.2.0`
	* Switched from TILER2 Dependency to KomradeSpectre's ItemBase for more code freedom
	* Circumstances mean that there will be less initial items, but as I catch up they'll be re-added.
	* New:
		* [Common Item] Antibody: Upon `healing`, gain a 25% chance to heal for +33% more healing `(+11% more healing per stack)`.
		* [Uncommon Item] Ring of Chest Friendship: Increases the amount of `chest credit` by +50% `(+25% per stack)`.
		* [Equipment] iBomb Companion App: Upon use, within a radius of 75 meters, all explosives and explosive enemies will explode. CD:90
			* Pots and Fusion Cells are instantly killed.
			* Clay Templars and Clayman take 200% damage.
			* Jellyfish take 300% damage.
			* Wandering Vagrants take 100% damage.
			* Clay Dunestriders take 150% damage.
			* Explosive Projectiles are destroyed.
		* [Lunar Equipment] Trusty Lockpicks: 50% chance to unlock a chest, or double its price. CD: 60
		* [Artifact] Artifact of the Pit Lord: When enabled, players take no damage from falling off the stage.
			* Can be configured to include monsters too.
	* Changes:
		* Mustache: 
			* (old) 2 (+2 per stack) for 10 seconds
			* (new) 2 (+1 per stack) for 8 seconds
		* Scope:
			* (old) -10% (-5% per stack) spread
			* (new) -20% (-2% per stack) spread.
			* (new) Stacking beyond the max increases your damage by 1% per stack.
			* *By increasing the initial value and reducing the stack count, the cap increases from 19 to 41. Also the inclusion of a slight damage increase should make it less annoying if you're forced to pick it up when at max. The initial increase makes it more meaningful when you have one, while the stacking has been decreased for balance.*
		* Backpack: Unchanged, but also added.
		
</details>