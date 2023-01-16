using System;
using BepInEx.Configuration;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class MonstersController : ControllerBase<MonstersController>
	{
        public static BodyIndex potIndex;
        public static ConfigEntry<float> cfgSpawnWispFromPotChance;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            Hooks();
        }


        public override void CreateConfig(ConfigFile config)
        {
            var category = "Monsters";
            cfgSpawnWispFromPotChance = config.Bind(category, "Chance to Spawn Wisp From Pot", 100f, "The chance to spawn a wisp when breaking a pot. 5 = 5% chance");

        }
        public override void Hooks()
        {
            base.Hooks();
            if (cfgSpawnWispFromPotChance.Value > 0)
                GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeathGlobal;
        }

        private void OnCharacterDeathGlobal(DamageReport obj)
        {
            if (NetworkServer.active)
            {
                if (obj?.victimBody?.baseNameToken == "POT2_BODY_NAME"
                && Util.CheckRoll(cfgSpawnWispFromPotChance.Value))
                {
                    var wispMaster = new MasterSummon
                    {
                        position = obj.victimBody.corePosition,
                        ignoreTeamMemberLimit = true,
                        masterPrefab = GlobalEventManager.CommonAssets.wispSoulMasterPrefabMasterComponent.gameObject,
                        summonerBodyObject = obj.victim.gameObject
                    }.Perform();
                    /*if (wispMaster 
                    && wispMaster.TryGetComponent<BaseAI>(out BaseAI baseAI)
                    && obj.attackerBody)
                    {
                    }*/
                }
            }
        }
    }
}