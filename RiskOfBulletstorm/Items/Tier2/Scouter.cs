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
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.CharacterBody.FixedUpdate -= CharacterBody_FixedUpdate;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker == self.gameObject)
            {
                var InventoryCount = GetCount(self.body);
                if (InventoryCount > 0)
                {
                    CharacterBody component = damageInfo.attacker.gameObject.GetComponent<CharacterBody>();
                    var EnemyName = component.name;
                    var EnemyHealthMax = component.maxHealth;
                    var EnemyHealth = component.healthComponent.health;
                    var EnemyShieldMax = component.maxShield;
                    var EnemyShield = component.healthComponent.shield;
                    var ScouterMsg = "==="+ EnemyName.ToString().ToUpper()+"===" +
                        "\n FleshHP: " + EnemyHealth.ToString() + " / " + EnemyHealthMax.ToString() +
                        "\n ShieldHP:" + EnemyShield.ToString() + " / " + EnemyShieldMax.ToString() +
                        "\n Damage Received" + damageInfo.damage.ToString() + "(" + damageInfo.damageType + ")" +
                        "===SCOUTER===";

                    Chat.AddMessage(ScouterMsg);
                }
            }
            orig(self, damageInfo);
        }
    }
}
