using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RoR2.Artifacts;
using System.Linq;
using RoR2.Networking;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using static RiskOfBulletstorm.BulletstormPlugin;
using RiskOfBulletstorm.Items;

namespace RiskOfBulletstorm.Shared.Buffs
{
    public static class CharmBuff
    {
        // TODO: IL Add friendly fire bypass
        public static bool Config_Charm_Boss = GungeonBuffController.instance.Config_Charm_Boss;
        public static BuffIndex Config_Charm_PlayerBuff = GungeonBuffController.instance.Config_Charm_PlayerBuff;
        public static BuffIndex charmIndex = GungeonBuffController.Charm;

        public static void Install()
        {
            // Spawn //
            On.RoR2.CharacterBody.Start += Charmed_AddComponent;
            // Buff //
            On.RoR2.CharacterBody.AddBuff += Charmed_EnableComponent;
            // AI //
            On.RoR2.CharacterAI.BaseAI.OnBodyDamaged += BaseAI_RetaliateSpecial;
            On.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox += BaseAI_CustomTargeting;
            // Visual //
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        public static void Uninstall()
        {
            // Spawn //
            On.RoR2.CharacterBody.Start -= Charmed_AddComponent;
            // Buff //
            On.RoR2.CharacterBody.AddBuff -= Charmed_EnableComponent;
            // AI //
            On.RoR2.CharacterAI.BaseAI.OnBodyDamaged -= BaseAI_RetaliateSpecial;
            On.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox -= BaseAI_CustomTargeting;
            // Visual //
            On.RoR2.CharacterModel.UpdateOverlays -= CharacterModel_UpdateOverlays;
        }

        // Spawn //
        private static void Charmed_AddComponent(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);

            if (!self.isPlayerControlled && self.masterObject)
            {
                var baseAI = self.masterObject.GetComponent<BaseAI>();
                if (baseAI)
                {
                    var isCharmed = self.gameObject.GetComponent<IsCharmed>();
                    if (!isCharmed) isCharmed = self.gameObject.AddComponent<IsCharmed>();

                    isCharmed.enabled = false;
                    isCharmed.characterBody = self;
                    isCharmed.teamComponent = self.teamComponent;
                    isCharmed.baseAI = baseAI;
                    isCharmed.oldTeamIndex = self.teamComponent.teamIndex;
                }
            }
        }

        // Buff //
        private static void Charmed_EnableComponent(On.RoR2.CharacterBody.orig_AddBuff orig, CharacterBody self, BuffIndex buffType)
        {
            IsCharmed isCharmed = null;
            if (buffType == charmIndex)
            {
                if (!Config_Charm_Boss && self.isBoss) //prevents adding the buff if it's a boss and the config is disabled
                    return;

                if (self.isPlayerControlled)
                    buffType = Config_Charm_PlayerBuff;
                else
                {
                    isCharmed = self.gameObject.GetComponent<IsCharmed>();
                }
            }
            orig(self, buffType);
            // If I don't put this after the orig call I think the component immediately disables because of the
            // fixedupdate check for no buff. Enable component -> component has no buff so it disables -> buff gets added but component is off
            if (isCharmed != null && isCharmed && !isCharmed.enabled) isCharmed.enabled = true; //quick and dirty test
        }
        // AI //
        private static void BaseAI_RetaliateSpecial(On.RoR2.CharacterAI.BaseAI.orig_OnBodyDamaged orig, BaseAI self, DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;

            var isCharmed = self.body.gameObject.GetComponent<IsCharmed>();
            if (isCharmed && isCharmed.enabled)
            {
                bool attackerIsCharmerTeam = damageReport.attackerTeamIndex == isCharmed.GetOppositeTeamIndex(isCharmed.GetOldTeam());
                if (attackerIsCharmerTeam)
                {
                    bool noTarget = (!self.currentEnemy.gameObject || self.enemyAttention <= 0f); // no current enemy or they're ready to retarget
                    bool attackerNotSelf = damageInfo.attacker != self.body.gameObject; // if their target isnt themselves
                    bool enemyIsNotCharmed = !damageInfo.attacker.GetComponent<IsCharmed>();
                    if (noTarget && attackerNotSelf && enemyIsNotCharmed)
                    {
                        return;
                    }
                }
            }
            var attacker = damageInfo.attacker;
            if (attacker) // prevents retaliating against a charmed enemy if they were on the prior team
            {
                var attackerIsCharmed = attacker.GetComponent<IsCharmed>();
                if (attackerIsCharmed && attackerIsCharmed.enabled && damageReport.attackerTeamIndex == attackerIsCharmed.GetOldTeam()) return;
            }
            orig(self, damageReport);
        }
        private static HurtBox BaseAI_CustomTargeting(On.RoR2.CharacterAI.BaseAI.orig_FindEnemyHurtBox orig, RoR2.CharacterAI.BaseAI self, float maxDistance, bool full360Vision, bool filterByLoS)
        {
            var gameObject = self.body.gameObject;
            if (gameObject)
            {
                var isCharmed = gameObject.GetComponent<IsCharmed>();
                if (isCharmed && isCharmed.enabled)
                {
                    self.enemySearch.viewer = self.body;
                    self.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
                    self.enemySearch.teamMaskFilter.RemoveTeam(isCharmed.GetOppositeTeamIndex(isCharmed.GetOldTeam()));
                    self.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
                    self.enemySearch.minDistanceFilter = 0;
                    self.enemySearch.maxDistanceFilter = maxDistance * 4f; //maxDistance
                    self.enemySearch.searchOrigin = self.bodyInputBank.aimOrigin;
                    self.enemySearch.searchDirection = self.bodyInputBank.aimDirection;
                    self.enemySearch.maxAngleFilter = 180f; // (full360Vision ? 180f : 90f)
                    self.enemySearch.filterByLoS = filterByLoS;
                    self.enemySearch.RefreshCandidates();
                    self.enemySearch.FilterOutGameObject(gameObject);
                    var list = self.enemySearch.GetResults().ToList();

                    foreach (HurtBox hurtBox in list)
                    {
                        if (!hurtBox.GetComponent<IsCharmed>())
                        {
                            return hurtBox; //Chooses the first non-charmed target
                        }
                    }
                    return list.FirstOrDefault(); //and falls back if it can't
                }
            }
            return orig(self, maxDistance, full360Vision, filterByLoS);
        }
        private static void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);

            if (self)
            {
                if (self.body && self.body.HasBuff(charmIndex))
                {
                    var isCharmed = self.body.GetComponent<IsCharmed>();
                    if (isCharmed && !isCharmed.Overlay)
                    {
                        TemporaryOverlay overlay = self.gameObject.AddComponent<TemporaryOverlay>();
                        overlay.duration = float.PositiveInfinity;
                        overlay.alphaCurve = AnimationCurve.Constant(0f, 5f, 0.54f);
                        overlay.animateShaderAlpha = true;
                        overlay.destroyComponentOnEnd = true;
                        overlay.originalMaterial = Resources.Load<Material>("@RiskOfBulletstorm:Assets/Textures/Materials/Overlays/Charmed.mat");
                        overlay.AddToCharacerModel(self);
                        isCharmed.Overlay = overlay;
                    } else return;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
        public class IsCharmed : MonoBehaviour
        {
            //public float duration = CharmHorn.instance.CharmHorn_Duration;
            public CharacterBody characterBody;
            public TeamComponent teamComponent;
            public TeamIndex oldTeamIndex;
            public BaseAI baseAI;
            public BuffIndex charmIndex = GungeonBuffController.Charm;
            public bool buffExpired = false;

            public TemporaryOverlay Overlay;

            void Start()
            {
                // If the current target was an enemy of the previous team
                if (baseAI.currentEnemy != null)
                {
                    var currentEnemyCharacterBody = baseAI.currentEnemy.characterBody;
                    if (currentEnemyCharacterBody && currentEnemyCharacterBody.teamComponent && currentEnemyCharacterBody.teamComponent.teamIndex == GetOppositeTeamIndex(oldTeamIndex))
                    {
                        ResetTarget();
                    }
                }
            }
            
            void FixedUpdate()
            {
                if (!characterBody.HasBuff(charmIndex))
                {
                    buffExpired = true;
                    Destroy(Overlay);
                }
                if (buffExpired)
                {
                    enabled = false;
                }
            }

            void OnDisable()
            {
                if (characterBody)
                {
                    if (characterBody.HasBuff(charmIndex))
                        characterBody.RemoveBuff(charmIndex);
                }
                buffExpired = false;
                ResetTarget();
            }

            public void ResetTarget()
            {
                if (baseAI && characterBody && characterBody.healthComponent && characterBody.healthComponent.alive)
                {
                    // No need to check for the enemy's current target, since it's guaranteed to be not the player.
                    baseAI.currentEnemy.Reset();
                    baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                }
            }

            public TeamIndex GetOppositeTeamIndex(TeamIndex teamIndex)
            {
                if (teamIndex == TeamIndex.Player) return TeamIndex.Monster;
                else if (teamIndex == TeamIndex.Monster) return TeamIndex.Player;
                else return teamIndex;
            }

            public TeamIndex GetOldTeam()
            {
                return oldTeamIndex;
            }
        }
    }
}
