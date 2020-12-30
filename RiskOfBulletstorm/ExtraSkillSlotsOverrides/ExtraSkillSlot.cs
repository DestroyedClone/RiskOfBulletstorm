using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    //Using struct instead of Enum for implicit conversion
    public struct ExtraSkillSlot
    {
        public int Value { get; }

        //Basic skill slots (this is not necessary but why not)
        public static readonly ExtraSkillSlot None = new ExtraSkillSlot((int)SkillSlot.None);
        public static readonly ExtraSkillSlot Primary = new ExtraSkillSlot((int)SkillSlot.Primary);
        public static readonly ExtraSkillSlot Secondary = new ExtraSkillSlot((int)SkillSlot.Secondary);
        public static readonly ExtraSkillSlot Utility = new ExtraSkillSlot((int)SkillSlot.Utility);
        public static readonly ExtraSkillSlot Special = new ExtraSkillSlot((int)SkillSlot.Special);

        public static readonly ExtraSkillSlot ExtraFirst = new ExtraSkillSlot(11);
        public static readonly ExtraSkillSlot ExtraSecond = new ExtraSkillSlot(12);
        public static readonly ExtraSkillSlot ExtraThird = new ExtraSkillSlot(13);
        public static readonly ExtraSkillSlot ExtraFourth = new ExtraSkillSlot(14);

        private ExtraSkillSlot(int value)
        {
            Value = value;
        }

        public static implicit operator SkillSlot(ExtraSkillSlot extraSkillSlot)
        {
            return (SkillSlot)extraSkillSlot.Value;
        }

        public static implicit operator ExtraSkillSlot(SkillSlot skillSlot)
        {
            return new ExtraSkillSlot((int)skillSlot);
        }
    }
}
