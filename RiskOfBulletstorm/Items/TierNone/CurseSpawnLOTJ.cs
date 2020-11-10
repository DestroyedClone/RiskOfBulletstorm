/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using EntityStates.ScavMonster;
using EntityStates.Engi.EngiWeapon;
//using Aetherium.Utils;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Net;
using static RiskOfBulletstorm.Shared.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class CurseSpawnLOTJ : Item_V2<CurseSpawnLOTJ>
    {
        public override string displayName => "CurseSpawnLOTJ";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        //public static GameObject ItemBodyModelPrefab;
        public static GameObject characterPrefab;

        public static SkillDef primaryReplacement;
        public static SkillDef secondaryReplacement;
        public static SkillDef utilityReplacement;
        public static SkillDef specialReplacement;

        public override void SetupBehavior()
        {


            base.SetupBehavior();
        }
        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody"), "LordJammedBody", true);

            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = "LORDJAMMED_NAME"; // name token
            bodyComponent.subtitleNameToken = "LORDJAMMED_SUBTITLE"; // subtitle token- used for umbras
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.Mechanical;
            //bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 10000;
            bodyComponent.levelMaxHealth = 10000;
            bodyComponent.baseRegen = 1000f;
            bodyComponent.levelRegen = 1000f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            //bodyComponent.baseMoveSpeed = 12;
            bodyComponent.levelMoveSpeed = 0;
            //bodyComponent.baseAcceleration = 80;
            //bodyComponent.baseJumpPower = 0;
            bodyComponent.levelJumpPower = 0;
            //bodyComponent.baseDamage = 15;
            //bodyComponent.levelDamage = 1.5f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 0;
            //bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 10000; // 0.0099 damage multiplier
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 0;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = true;
            healthComponent.godMode = true;
        }

        public override void SetupAttributes()
        {
            base.SetupAttributes();
            LanguageAPI.Add("LORDJAMMED_NAME", "Lord of the Jammed");
            LanguageAPI.Add("LORDJAMMED_SUBTITLE", "Ceaseless");
        }
        public override void SetupConfig() { base.SetupConfig(); }
        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CalculateCurse;
            On.RoR2.CharacterBody.FixedUpdate += SummonLOTJ;
            On.RoR2.CharacterBody.FixedUpdate += LOTJRetarget;
            //On.RoR2.MapZone.TeleportBody += DisableOOBCheck;
            On.RoR2.MapZone.TryZoneStart += LOTJFall;
            On.RoR2.HealthComponent.Suicide += PreventSuicide;
            //Stage.onStageStartGlobal += Stage_onStageStartGlobal;

        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CalculateCurse;
            On.RoR2.CharacterBody.FixedUpdate -= SummonLOTJ;
            On.RoR2.CharacterBody.FixedUpdate -= LOTJRetarget;
            //On.RoR2.MapZone.TeleportBody -= DisableOOBCheck;
            On.RoR2.MapZone.TryZoneStart -= LOTJFall;
            On.RoR2.HealthComponent.Suicide -= PreventSuicide;
            //Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
        }

        private void CalculateCurse(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) 
        {
            var CurseInventoryCount = self.inventory.GetItemCount(Curse.instance.catalogIndex);
            int MaxCurseInvCount = self.inventory.GetItemCount(catalogIndex);
            var CurseMax = Curse.instance.CurseMax;
            if (CurseInventoryCount < CurseMax && MaxCurseInvCount == 1)
            {
                self.inventory.RemoveItem(catalogIndex);
                orig(self);
                return;
            }
            //var body = self.gameObject;

            orig(self);
        }

        private void SummonLOTJ(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            int InventoryCount = GetPlayersItemCount(catalogIndex);

            var LOTJComponent = self.master.GetComponent<LordOfTheJammedComponent>();
            if (!LOTJComponent) { LOTJComponent = self.masterObject.AddComponent<LordOfTheJammedComponent>(); }

            //PREVENT NONPLAYER
            var SummonerBodyMaster = self.master;
            if (SummonerBodyMaster) //Check if we're a minion or not. If we are, we don't summon LOTJ.
            {
                if (!self.isPlayerControlled)
                {
                    orig(self);
                    return;
                }
            }

            //string spawnCard = "SpawnCards/CharacterSpawnCards/cscBrother";
            string spawnCard = "SpawnCards/CharacterSpawnCards/cscJellyfish";

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest((SpawnCard)Resources.Load(spawnCard), new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 100f,
                maxDistance = 101f,
                spawnOnTarget = self.transform
            }, RoR2Application.rng);

            GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
            if (InventoryCount > 0)
            {
                if (gameObject)
                {
                    //SKILL REPLACEMENTS    
                    {
                        primaryReplacement = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("JellyBarrage"));
                        secondaryReplacement = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("SprintShootShards"));
                        utilityReplacement = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("SprintShootShards"));
                        specialReplacement = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("SprintShootShards"));

                        var skillComponent = self.GetComponent<SkillLocator>();
                        if (skillComponent)
                        {
                            skillComponent.primary.SetSkillOverride(self, primaryReplacement, GenericSkill.SkillOverridePriority.Replacement);
                            skillComponent.primary.SetSkillOverride(self, secondaryReplacement, GenericSkill.SkillOverridePriority.Replacement);
                            skillComponent.primary.SetSkillOverride(self, utilityReplacement, GenericSkill.SkillOverridePriority.Replacement);
                            skillComponent.primary.SetSkillOverride(self, specialReplacement, GenericSkill.SkillOverridePriority.Replacement);
                        }
                    }


                    CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
                    //AIOwnership component1 = gameObject.GetComponent<AIOwnership>();
                    //BaseAI component2 = gameObject.GetComponent<BaseAI>();

                    if (component)
                    {
                        //RoR2.Chat.AddMessage($"Character Master Found: {component}");
                        CharacterBody body = component.GetBody();

                        CharacterModel model = body.GetComponent<CharacterModel>();
                        if (model && component.inventory.GetItemCount(ItemIndex.Ghost) == 0)
                        {
                            Debug.Log("BrotherBodyName = "+model.body.ToString());

                        }

                        HealthComponent healthComponent = body.healthComponent;
                        healthComponent.dontShowHealthbar = true;
                        healthComponent.godMode = true;

                        body.baseNameToken = "LORDJAMMED_NAME"; // name token
                        body.subtitleNameToken = "LORDJAMMED_SUBTITLE"; // subtitle token- used for umbras
                        body.baseRegen = 10000;

                        body.teamComponent.teamIndex = TeamIndex.Monster;
                        component.teamIndex = TeamIndex.Monster;

                        component.inventory.GiveItem(ItemIndex.InvadingDoppelganger);
                        component.inventory.GiveItem(ItemIndex.Ghost);

                        //GET BODY
                        CharacterBody component4 = component.GetBody();
                        if (component4)
                        {
                            //RoR2.Chat.AddMessage($"CharacterBody Found: {component4}");
                            var gameObj = component4.gameObject;
                            gameObj.AddComponent<LordOfTheJammedComponent>();

                            LOTJComponent.LastLOTJSpawned = component4;
                            //DEATH REWARDS
                            DeathRewards component5 = component4.GetComponent<DeathRewards>();
                            if (component5)
                            {
                                //RoR2.Chat.AddMessage($"DeathRewards Found: {component5}");
                                component5.goldReward = 0;
                                component5.expReward = 0;
                                component5.logUnlockableName = null;
                                component5.spawnValue = 0;
                            }
                        }
                    }
                }
            }
            orig(self);
        }

        private void LOTJRetarget(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            var characterMaster = self.master;
            if (characterMaster)
            {
                var baseAIComponent = characterMaster.GetComponent<BaseAI>();
                var retargetComponent = self.GetComponent<LOTJRetargetComponent>();
                if (baseAIComponent && retargetComponent)
                {
                    retargetComponent.RetargetTimer -= Time.fixedDeltaTime;
                    if (retargetComponent.RetargetTimer <= 0)
                    {
                        if (!baseAIComponent.currentEnemy.hasLoS)
                        {
                            GameObject CursedPlayer = GetFirstPlayerWithItem(catalogIndex);

                            baseAIComponent.currentEnemy.Reset();
                            retargetComponent.RetargetTimer = 5f;
                            if (CursedPlayer != null)
                            {
                                baseAIComponent.customTarget.gameObject = CursedPlayer;
                                baseAIComponent.currentEnemy.gameObject = CursedPlayer;
                            }
                        }
                    }
                }
            }
            orig(self);
        }

        private static void DisableOOBCheck(On.RoR2.MapZone.orig_TeleportBody orig, MapZone self, CharacterBody characterBody) //debugtoolkit
        {
            //if (!characterBody.isPlayerControlled)
                orig(self, characterBody);
        }

        private void LOTJFall(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other) //if somehow the OOB Check didn't trigger
        {
            var body = other.GetComponent<CharacterBody>();
            if (body)
            {
                var LOTJRetargeter = body.GetComponent<LOTJRetargetComponent>();
                if (LOTJRetargeter)
                {
                    var teamComponent = body.teamComponent;
                    teamComponent.teamIndex = TeamIndex.Player;  //Set the team of it to player to avoid it dying when it falls into a hellzone.
                    orig(self, other);                           //Run the effect of whatever zone it is in on it. Since it is of the Player team, it obviously gets teleported back into the zone.
                    teamComponent.teamIndex = TeamIndex.Monster; //Now make it hostile again. Thanks Obama.
                    return;
                }
            }
            orig(self, other);
        }

        private void PreventSuicide(On.RoR2.HealthComponent.orig_Suicide orig, HealthComponent self, GameObject killerOverride, GameObject inflictorOverride, DamageType damageType)
        {
            var body = self.GetComponent<CharacterBody>();
            if (body)
            {
                var LOTJRetargeter = body.GetComponent<LOTJRetargetComponent>();
                if (LOTJRetargeter)
                {
                    Debug.Log("LOTJ: Prevented suicide!");
                    return;
                }
            }
            orig(self, killerOverride, inflictorOverride, damageType);
        }

        public class LordOfTheJammedComponent : MonoBehaviour
        {
            public CharacterBody LastLOTJSpawned;
        }
        public class LOTJRetargetComponent : MonoBehaviour
        {
            public float RetargetTimer;
        }
    }
}
*/