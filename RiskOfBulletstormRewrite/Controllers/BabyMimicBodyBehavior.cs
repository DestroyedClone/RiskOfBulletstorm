using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RoR2;
using RoR2.Items;
using RiskOfBulletstormRewrite;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class BabyMimicBodyBehavior : BaseItemBodyBehavior
	{
		private DeployableMinionSpawner redBuddySpawner;

		private DeployableMinionSpawner greenBuddySpawner;

		[BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = false)]
		private static ItemDef GetItemDef()
		{
			return Items.BabyGoodMimic.instance.ItemDef;
		}

		private void FixedUpdate()
		{
			if (redBuddySpawner == null && base.isActiveAndEnabled)
			{
				CreateSpawners();
			}
		}
		private void OnDisable()
		{
			DestroySpawners();
		}

		private void CreateSpawners()
		{
			RoboBallBuddyBodyBehavior.<> c__DisplayClass5_0 CS$<> 8__locals1;
			CS$<> 8__locals1.<> 4__= 
			CS$<> 8__locals1.rng = new Xoroshiro128Plus(Run.instance.seed ^ (ulong)((long)base.GetInstanceID()));
			CreateSpawner(ref redBuddySpawner, DeployableSlot.RoboBallRedBuddy, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallRedBuddy"), ref CS$<> 8__locals1);
			CreateSpawner(ref greenBuddySpawner, DeployableSlot.RoboBallGreenBuddy, LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallGreenBuddy"), ref CS$<> 8__locals1);
		}

		private void DestroySpawners()
		{
			DeployableMinionSpawner deployableMinionSpawner = redBuddySpawner;
			if (deployableMinionSpawner != null)
			{
				deployableMinionSpawner.Dispose();
			}
			redBuddySpawner = null;
			DeployableMinionSpawner deployableMinionSpawner2 = greenBuddySpawner;
			if (deployableMinionSpawner2 != null)
			{
				deployableMinionSpawner2.Dispose();
			}
			greenBuddySpawner = null;
		}

		private void OnMinionSpawnedServer(SpawnCard.SpawnResult spawnResult)
		{
			GameObject spawnedInstance = spawnResult.spawnedInstance;
			if (spawnedInstance)
			{
				CharacterMaster component = spawnedInstance.GetComponent<CharacterMaster>();
				if (component)
				{
					Inventory inventory = base.body.inventory;
					Inventory inventory2 = component.inventory;
					if (inventory)
					{
						RoboBallBuddyBodyBehavior.InventorySync inventorySync = spawnedInstance.AddComponent<RoboBallBuddyBodyBehavior.InventorySync>();
						inventorySync.srcInventory = inventory;
						inventorySync.destInventory = inventory2;
					}
				}
			}
		}

		[CompilerGenerated]
		private void CreateSpawner(ref DeployableMinionSpawner buddySpawner, DeployableSlot deployableSlot, SpawnCard spawnCard, ref RoboBallBuddyBodyBehavior.<>c__DisplayClass5_0 A_4)
		{
			buddySpawner = new DeployableMinionSpawner(base.body.master, deployableSlot, A_4.rng)
			{
			respawnInterval = 30f,
				spawnCard = spawnCard
			};
		buddySpawner.onMinionSpawnedServer += OnMinionSpawnedServer;
		}
}
}
