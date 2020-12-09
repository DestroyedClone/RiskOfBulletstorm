
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Events;

namespace RiskOfBulletstorm.Items
{
    public class CharmHorn : Equipment_V2<CharmHorn>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius to charm enemies? (Default: 20m)", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Radius { get; private set; } = 20f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of charmed enemies? (Default: 10 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Duration { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds? (Default: 85.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 85.00f;

        public override string displayName => "Charm Horn";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "The Call Of Duty\nWhen blown, this horn will call those nearby to aid you.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Blows the horn to <style=cIsUtility>charm</style> enemies within <style=cIsUtility>{CharmHorn_Radius} meters</style> for {CharmHorn_Duration} seconds.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "There are strange inconsistencies in the behavior of the Gundead. Originally thought to be heartless killing machines, they have been known to capture certain invaders for unknown purposes. Furthermore, evidence of a crude religion has been discovered. Perhaps, one day, they could be reasoned with?";

        public static GameObject CharmWardPrefab { get; private set; }
        //public static readonly BuffIndex CharmBuff = GungeonBuffController.Charm;
        //public static readonly BuffIndex AngerBuff = GungeonBuffController.Anger;

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            CharmWardPrefab = warbannerPrefab.InstantiateClone("Bulletstorm_CharmHornWard");

            BuffWard buffWard = CharmWardPrefab.GetComponent<BuffWard>();
            buffWard.expires = true;
            buffWard.expireDuration = 0.6f;
            buffWard.buffDuration = CharmHorn_Duration;
            buffWard.invertTeamFilter = true;

            //CharmWard charmWard = CharmWardPrefab.AddComponent<CharmWard>();
            //charmWard.expires = true;
            //charmWard.expireDuration = 3f;
            //charmWard.floorWard = false;

            //charmWard.invertTeamFilter = true;
            /*buffWard.Networkradius = CharmHorn_Radius;
            charmWard.animateRadius = buffWard.animateRadius;
            charmWard.calculatedRadius = buffWard.calculatedRadius;
            charmWard.interval = buffWard.interval;
            charmWard.needsRemovalTime = buffWard.needsRemovalTime;
            charmWard.onRemoval = buffWard.onRemoval;
            charmWard.radiusCoefficientCurve = buffWard.radiusCoefficientCurve;
            charmWard.rangeIndicator = buffWard.rangeIndicator;
            charmWard.rangeIndicatorScaleVelocity = buffWard.rangeIndicatorScaleVelocity;
            charmWard.removalTime = buffWard.removalTime;
            charmWard.stopwatch = buffWard.stopwatch;
            charmWard.teamFilter = buffWard.teamFilter;*/

            //UnityEngine.Object.Destroy(buffWard);

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
            if (CharmWardPrefab) PrefabAPI.RegisterNetworkPrefab(CharmWardPrefab);
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        public override void SetupLate()
        {
            base.SetupLate();


        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;
            float multiplier = 1.0f;

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body))
                multiplier += 0.5f;
            CharmNearby(body, CharmHorn_Radius * multiplier);
            return true;
        }

        public void CharmNearby(CharacterBody body, float radius)
        {
            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(CharmWardPrefab, body.transform.position, Quaternion.identity);
                gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                BuffWard buffWard = gameObject.GetComponent<BuffWard>();
                buffWard.buffType = GungeonBuffController.Charm;
                buffWard.GetComponent<BuffWard>().Networkradius *= radius;
                NetworkServer.Spawn(gameObject);
            }
        }

        /*public class CharmWard : BuffWard
        {
            // Token: 0x06000839 RID: 2105 RVA: 0x00020118 File Offset: 0x0001E318
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "overrides method")]
            private void BuffTeam(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                foreach (TeamComponent teamComponent in recipients)
                {
                    if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                    {
                        CharacterBody component = teamComponent.GetComponent<CharacterBody>();
                        if (component)
                        {
                            component.AddBuff(buffType);
                        }
                    }
                }
            }
        }*/
    }
}
