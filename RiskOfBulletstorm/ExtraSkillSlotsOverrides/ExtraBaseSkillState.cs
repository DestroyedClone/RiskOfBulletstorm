using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Networking;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    internal class ExtraBaseSkillState
    {
        private static readonly Dictionary<BaseSkillState, ExtraBaseSkillState> instances = new Dictionary<BaseSkillState, ExtraBaseSkillState>();

        public ExtraInputBankTest extraInputBankTest { get; private set; }

        internal static ExtraBaseSkillState Add(BaseSkillState baseSkillState)
        {
            return instances[baseSkillState] = new ExtraBaseSkillState
            {
                extraInputBankTest = baseSkillState.outer.GetComponent<ExtraInputBankTest>()
            };
        }

        internal static void Remove(BaseSkillState baseSkillState)
        {
            instances.Remove(baseSkillState);
        }

        internal static ExtraBaseSkillState Get(BaseSkillState baseSkillState)
        {
            if (instances.TryGetValue(baseSkillState, out var extraBaseSkillState))
            {
                return extraBaseSkillState;
            }
            return null;
        }

        public static void IsKeyDownAuthorityILHook(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchNewobj<ArgumentOutOfRangeException>(),
                x => x.MatchThrow());
            c.Index++;
            c.Previous.OpCode = OpCodes.Nop;
            c.Previous.Operand = null;

            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<BaseSkillState, bool>>(self =>
            {
                var extraBaseSkillState = Get(self);

                var extraInputBank = extraBaseSkillState.extraInputBankTest;


                if (!extraInputBank)
                {
                    return false;
                }

                throw new ArgumentOutOfRangeException();
            });
            c.Emit(OpCodes.Ret);
        }
    }
}
