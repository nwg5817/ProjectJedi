using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    public class VerbProperties_PawnAbility : VerbProperties
    {
        public bool DrawProjectileOnTarget = true;

        public bool AlwaysHits = true;

        public bool HarmsCaster;
        public float CasterDamage = 0f;

        public float AlignmentFactor = 1f;

        public int TicksToRecharge = 600;
        
        public PawnAbilityTargetCategory PawnAbilityTargetCategory = PawnAbilityTargetCategory.TargetThing;
        public Type AoETargetClass;

        public bool ReplacesStandardAttack;

        public List<StatModifier> statModifiers;

    }
}
