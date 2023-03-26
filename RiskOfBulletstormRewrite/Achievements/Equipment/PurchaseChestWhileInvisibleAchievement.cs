using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;

namespace RiskOfBulletstormRewrite.Achievements.Equipment
{
    [RegisterAchievement("RISKOFBULLETSTORM_PURCHASECHESTWHILEINVISIBLE", "RiskOfBulletstorm.Equipment.DRILL", null, null)]
    public class PurchaseChestWhileInvisibleAchievement : RoBBaseAchievement
    {
        public override string BaseToken => "PURCHASECHESTWHILEINVISIBLE";

        public override void OnGranted()
        {
            base.OnGranted();
        }

        public override void OnInstall()
        {
            base.OnInstall();
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (activator.TryGetComponent(out CharacterBody interactorBody)
                && interactorBody == localUser.cachedBody
                && interactorBody.modelLocator
                && interactorBody.modelLocator.modelTransform
                && interactorBody.modelLocator.modelTransform.TryGetComponent(out CharacterModel characterModel)
                && characterModel.invisibilityCount > 0)
            {
                base.Grant();
            }
        }

        public override void OnUninstall()
        {
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
            base.OnUninstall();
        }
    }
}
