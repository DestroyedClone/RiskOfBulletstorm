//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using R2API.Networking;

namespace RiskOfBulletstorm.Items
{
    public class Scouter : Item_V2<Scouter>
    {
        public override string displayName => "Scouter";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Quality Assured\nShows combat data. Increases accuracy and damage by a small amount.";

        protected override string GetDescString(string langid = null) => $"Shows enemy health up to 1 digit per stack.";

        protected override string GetLoreString(string langID = null) => "This scouter, worn with use, provides detailed data on enemies encountered within the Gungeon. The name \"Ritvik\" is inscribed on the rim.";

        public override void SetupBehavior()
        {

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
            //On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.CharacterBody.FixedUpdate -= CharacterBody_FixedUpdate;
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
        }
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            var attacker = damageInfo.attacker;
            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            var InventoryCount = body.inventory.GetItemCount(catalogIndex);

            if (!attacker) Chat.AddMessage("Scouter: "+damageInfo.attacker.ToString()+"=attacker not found");
            if (damageInfo.rejected) Chat.AddMessage("Scouter: " + damageInfo.rejected.ToString() + "=rejected not false");
            if (attacker && !damageInfo.rejected)
            {
                if (InventoryCount > 0)
                {
                    string cutText(float value)
                    {
                        var text = value.ToString();
                        int maxText = Math.Min(text.Length, InventoryCount);
                        var allowedText = text.Substring(Math.Max(0, text.Length - maxText));
                        string blockedText = new String('?', text.Length - maxText);
                        return blockedText + allowedText;
                    }
                    CharacterBody component = victim.gameObject.GetComponent<CharacterBody>();
                    var EnemyName = component.name;
                    var EnemyHealthMax = component.maxHealth;
                    var EnemyHealth = component.healthComponent.health;
                    var EnemyShieldMax = component.maxShield;
                    var EnemyShield = component.healthComponent.shield;
                    var DamageType = damageInfo.damageType.ToString();
                    var Damage = damageInfo.damage;
                    if (InventoryCount < 2) { DamageType = "???"; }
                    switch (InventoryCount)
                    {
                        case 0:
                            break;
                    }
                    var ScouterMsg = "===" + EnemyName.ToString().ToUpper() + "===" +
                        "\n FleshHP: " + cutText(EnemyHealth) + " / " + cutText(EnemyHealthMax) +
                        "\n ShieldHP:" + cutText(EnemyShield) + " / " + cutText(EnemyShieldMax) +
                        "\n Damage Received" + cutText(Damage) + "(" + DamageType + ")" +
                        "===SCOUTER===";

                    Chat.AddMessage(ScouterMsg);
                } else
                {
                    Chat.AddMessage("Scouter: InventoryCount=" + InventoryCount.ToString());
                }

            } else
            {
                Chat.AddMessage("Scouter: "+damageInfo.attacker.ToString() + " did not equal " + self.gameObject.ToString());
            }
            orig(self, damageInfo, victim);
        }
    }
}
