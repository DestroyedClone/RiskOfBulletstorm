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

namespace RiskOfBulletstorm.Items
{
    public class GungeonBuffController : Item_V2<GungeonBuffController>
    {
        public override string displayName => "GungeonBuffController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";
        public static BuffIndex Burn { get; private set; }
        public static BuffIndex Poison { get; private set; }
        public static BuffIndex Curse { get; private set; }
        public static BuffIndex Stealth { get; private set; }
        public static BuffIndex Petrification { get; private set; }
        public static BuffIndex Anger { get; private set; } //
        public static BuffIndex Buffed { get; private set; }
        public static BuffIndex BurnEnemy { get; private set; }
        public static BuffIndex PoisonEnemy { get; private set; }
        public static BuffIndex Charm { get; private set; }
        public static BuffIndex Encheesed { get; private set; }
        public static BuffIndex Fear { get; private set; }
        public static BuffIndex Jammed { get; private set; } //
        public static BuffIndex Slow { get; private set; }
        public static BuffIndex Freeze { get; private set; }
        public static BuffIndex Stun { get; private set; }
        public static BuffIndex Weakened { get; private set; }
        public static BuffIndex Tangled { get; private set; }
        public static BuffIndex Encircled { get; private set; }
        public static BuffIndex Glittered { get; private set; }
        public static BuffIndex Bloody { get; private set; }

        private static readonly float HeartValue = 50f;

        private ItemIndex SpiceTally;

        private float[] SpiceBonusesAdditive;
        private float[,] SpiceBonuses;
        private float[] SpiceBonusesConstantMaxed;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        //https://enterthegungeon.gamepedia.com/Status_Effects
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var angerBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.red,
                canStack = false,
                isDebuff = false,
                name = "Enraged",
            });
            Anger = BuffAPI.Add(angerBuff);

            var jammedBuff = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                canStack = false,
                isDebuff = false,
                iconPath = "",
                //eliteIndex = JammedEliteIndex,
            });
            Jammed = BuffAPI.Add(jammedBuff);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupLate()
        {
            base.SetupLate();
            //so we dont crash
            SpiceTally = Spice.SpiceTally;
            SpiceBonusesAdditive = Spice.SpiceBonusesAdditive;
            SpiceBonuses = Spice.SpiceBonuses;
            SpiceBonusesConstantMaxed = Spice.SpiceBonusesConstantMaxed;

        }
        public override void Install()
        {
            base.Install();
            GetStatCoefficients += AddRewards;
            GetStatCoefficients += AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= AddRewards;
            GetStatCoefficients -= AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }

        private void AddSpiceRewards(CharacterBody sender, StatHookEventArgs args)
        {
            var SpiceTallyCount = sender.inventory.GetItemCount(SpiceTally);
            switch (SpiceTallyCount)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    //health, attack speed, shot accuracy, enemy bullet speed, damage
                    args.baseHealthAdd += HeartValue * SpiceBonuses[SpiceTallyCount, 0];
                    args.attackSpeedMultAdd += SpiceBonuses[SpiceTallyCount, 1];
                    //accuracy 
                    //enemy bullet speed
                    //damage
                    break;
                default:
                    var baseHealthAdd = HeartValue * SpiceBonusesAdditive[0] * (SpiceTallyCount - 4);

                    var healthComponent = sender.healthComponent;
                    if (healthComponent)
                    {
                        var oldMaxHealth = healthComponent.fullHealth;
                        var resultant = oldMaxHealth + args.baseHealthAdd + baseHealthAdd;
                        Debug.Log(resultant);
                        if (resultant < 1)
                        {
                            Debug.Log("value was too low");
                            baseHealthAdd = oldMaxHealth - 1;
                        }
                    }
                    var attackSpeedAdd = SpiceBonusesConstantMaxed[1];
                    args.baseHealthAdd += baseHealthAdd;
                    //health, attack speed, shot accuracy, enemy bullet speed, damage
                    args.attackSpeedMultAdd += attackSpeedAdd;
                    //accuracy
                    //enemy
                    //damage
                    break;
            }
        }
        private void AddRewards(CharacterBody sender, StatHookEventArgs args)
        {
            /*
            [0] Health: Handled Here
            [1] Attack Speed: Handled Here
            [2] Accuracy: BulletstormExtraStatsController
            [3] Enemy Bullet Speed: BulletstormExtraStatsController
            [4] Damage: HealthComponent_TakeDamage
            */
            if (sender.HasBuff(Anger)) { args.damageMultAdd += EnragingPhoto.instance.EnragingPhoto_DmgBoost; }
            if (sender.HasBuff(Jammed))
            {
                args.damageMultAdd += Items.CurseController.instance.Curse_DamageBoost;
                args.attackSpeedMultAdd += 0.2f;
                args.moveSpeedMultAdd += 0.2f;
                //Contact damage is handled by the component
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Used Values")]
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                CharacterBody body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var SpiceTallyCount = inventory.GetItemCount(SpiceTally);
                        var DamageMult = 0f;
                        var SpiceMult = 0f;
                        switch (SpiceTallyCount)
                        {
                            case 0: //
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                SpiceMult = SpiceBonuses[SpiceTallyCount, 4];
                                break;
                            default: //also 5
                                SpiceMult = SpiceBonuses[4, 4] + SpiceBonusesAdditive[4] * (SpiceTallyCount - 4);
                                break;
                        }
                        DamageMult = SpiceMult;
                        damageInfo.damage *= 1 + DamageMult;
                    }
                }
            }
            orig(self, damageInfo);
        }

        public static BlastAttack JammedContactDamage = new BlastAttack
        {
            losType = BlastAttack.LoSType.NearestHit,
            damageColorIndex = DamageColorIndex.Default,
            damageType = DamageType.Generic,
            procCoefficient = 0f,
            procChainMask = default,
            baseForce = 0,
            falloffModel = BlastAttack.FalloffModel.None,

        };

        public static DamageInfo JammedContactDamageInfo = new DamageInfo
        {
            damage = 0, //gets set by component
            crit = false,
            force = new Vector3(0,0,0),
            procChainMask = default,
            procCoefficient = 0,
            damageType = DamageType.Generic,
            damageColorIndex = DamageColorIndex.DeathMark
        };

        public class IsJammed : MonoBehaviour
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                Debug.Log("JammedOnEnable: 0");
                if (!characterBody.HasBuff(Jammed))
                {
                    characterBody.AddBuff(Jammed);
                }
                Debug.Log("JammedOnEnable: 1");
                contactDamageInfo.inflictor = ContactDamageGameObject; //how
                Debug.Log("JammedOnEnable: 2");
                contactDamageInfo.attacker = gameObject; //who
                Debug.Log("JammedOnEnable: 3");
            } //setup

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnTriggerEnter(Collider other)
            {
                GameObject gameObject = other.gameObject;
                HealthComponent healthComponent = gameObject.GetComponent<HealthComponent>();
                if (healthComponent)
                {
                    if (!healthComponents.Contains(healthComponent))
                    {
                        contactDamageInfo.position = gameObject.transform.position;
                        contactDamageInfo.damage = characterBody.damage * 3f;
                        healthComponent.TakeDamage(contactDamageInfo);

                        healthComponents.Add(healthComponent);
                        ContactDamageCooldown = ContactDamageCooldownFull;
                    }
                } else
                {
                    Destroy(this); //no point in running if there's no healthcomponent
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void FixedUpdate()
            {
                ContactDamageCooldown -= Time.fixedDeltaTime;
                if (ContactDamageCooldown < 0)
                {
                    healthComponents.Clear();
                }
            } //clears list at end of cooldown

            public static readonly float ContactDamageCooldownFull = 2f;
            public float ContactDamageCooldown = ContactDamageCooldownFull;
            public CharacterBody characterBody;
            public DamageInfo contactDamageInfo = JammedContactDamageInfo;
            public static GameObject ContactDamageGameObject;
            public float damageCoefficient = 3f;
            public List<HealthComponent> healthComponents;
        }
    }
}
