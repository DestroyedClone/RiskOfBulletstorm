using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Enemies.WeirdEgg
{
    internal class WeirdEggBody : MonsterBase<WeirdEggBody>
    {
        public override string MonsterName => "WeirdEgg";

        public override string MonsterLangTokenName => "WEIRDEGG_NAME";

        public override string MonsterLangTokenSubtitle => "WEIRDEGG_SUBTITLE";

        public override GameObject CreateBodyPrefab()
        {
            return base.CreateBodyPrefab();
        }
    }
}
