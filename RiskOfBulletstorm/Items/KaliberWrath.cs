
using EliteSpawningOverhaul;
using R2API;
using RoR2;
using TILER2;
using UnityEngine;

namespace RiskOfBulletstorm //TODO modify weight of card depending on curse level.
{
    public class KaliberWrath : Equipment_V2<KaliberWrath>
    {
        public override string displayName => "Wrath of Kaliber";
        public const string PickupText = "Become an aspect of defiance";
        public string DescText = $"+200% damage dealt.";
        public string loreText = $"Several quotes from what seems to be a form of holy scripture were found around the Gungeon. They reference the Gungeon's past, and made prophecies about its future.\nDue to the references to Kaliber that can be found in the texts, this book is most likely hers, however, it is incomplete.\nGunesis 1:1 In the beginning, the Gungeon was formless and void, and bullets moved over the face of the deep.\nRevolvations 2:1 These are the words of Kaliber, she who grips the seven sidearms in her hands and walks among the six loaded chambers. When the Gun is drawn to the heart of the pacifist, and when the chambers are empty and the shells are spent, the Last of the Jammed will ascend to seal The Breach forever.";
        
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => PickupText;

        protected override string GetDescString(string langID = null) => DescText;

        protected override string GetLoreString(string langID = null) => loreText;

        public static EliteAffixCard JammedEliteCard { get; set; }
        public static EliteIndex JammedEliteIndex;
        public static BuffIndex JammedBuffIndex;

        public override void SetupAttributes()
        {
            base.SetupAttributes();
            equipmentDef.canDrop = false;
            equipmentDef.enigmaCompatible = false;

            var jammedEliteDef = new CustomElite(
            new EliteDef
            {
                name = "Jammed",
                modifierToken = "RISKOFBULLETSTORM_ELITE_MODIFIER_JAMMED",
                color = new Color32(150, 10, 10, 255),
                eliteEquipmentIndex = equipmentDef.equipmentIndex
            }, 1);
            JammedEliteIndex = EliteAPI.Add(jammedEliteDef);
            LanguageAPI.Add(jammedEliteDef.EliteDef.modifierToken, "Jammed {0}");

            var jammedBuffDef = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                iconPath = "",
                eliteIndex = JammedEliteIndex,
                canStack = false
            });
            JammedBuffIndex = BuffAPI.Add(jammedBuffDef);
            equipmentDef.passiveBuff = JammedBuffIndex;

            JammedEliteCard = new EliteAffixCard
            {
                spawnWeight = 0.8f,
                costMultiplier = 15.0f,
                damageBoostCoeff = 2.0f,
                healthBoostCoeff = 3.0f,
                eliteOnlyScaling = 0.5f,
                eliteType = JammedEliteIndex,
            };
            EsoLib.Cards.Add(JammedEliteCard);
        }
        public override void Install()
        {
            base.Install();


        }
        public override void Uninstall()
        {
            base.Uninstall();

        }
        protected override bool PerformEquipmentAction(RoR2.EquipmentSlot slot)
        {
            return false;
        }
        public class JammedBuffChecker : MonoBehaviour
        {
            public RoR2.TemporaryOverlay Overlay;
            public RoR2.CharacterBody Body;

            public void FixedUpdate()
            {
                if (!Body.HasBuff(JammedBuffIndex))
                {
                    UnityEngine.Object.Destroy(Overlay);
                    UnityEngine.Object.Destroy(this);
                }
            }
        }
    }
}
