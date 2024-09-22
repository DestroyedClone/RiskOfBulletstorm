using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class MonstersController : ControllerBase<MonstersController>
    {
        public static BodyIndex potBodyIndex;
        public static ConfigEntry<float> cfgSpawnWispFromPotChance;

        public override void Init(ConfigFile config)
        {
            base.Init(config);
            BodyCatalog.availability.CallWhenAvailable(() => { potBodyIndex = BodyCatalog.FindBodyIndex("POT2"); });
        }

        public override void CreateConfig(ConfigFile config)
        {
            var category = "Monsters";
            cfgSpawnWispFromPotChance = config.Bind(category, "Chance to Spawn Wisp From Pot", 0.5f, Modules.Assets.cfgChanceIntegerDesc);
        }

        public override void Hooks()
        {
            base.Hooks();
            if (cfgSpawnWispFromPotChance.Value > 0)
                GlobalEventManager.onCharacterDeathGlobal += RollForSpawnWispOnPotBreak;
        }

        private void RollForSpawnWispOnPotBreak(DamageReport obj)
        {
            if (!NetworkServer.active) return;
            if (!obj.victimBody) return;
            if (obj.victimBody.bodyIndex != potBodyIndex) return;
            if (!Util.CheckRoll(cfgSpawnWispFromPotChance.Value)) return;

            new MasterSummon
            {
                position = obj.victimBody.corePosition,
                ignoreTeamMemberLimit = true,
                masterPrefab = GlobalEventManager.CommonAssets.wispSoulMasterPrefabMasterComponent.gameObject,
                summonerBodyObject = obj.victim.gameObject
            }.Perform();
        }
    }
}