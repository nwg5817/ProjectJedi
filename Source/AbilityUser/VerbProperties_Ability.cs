using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AbilityUser
{
    public class VerbProperties_Ability : VerbProperties
    {
        public bool isViolent = true;

        public bool DrawProjectileOnTarget = true;

        public AbilityDef abilityDef;

        public bool AlwaysHits = true;

        public bool HarmsCaster;
        public float CasterDamage = 0f;

        public float AlignmentFactor = 1f;

        public float SecondsToRecharge = 10.0f;
        
        public AbilityTargetCategory AbilityTargetCategory = AbilityTargetCategory.TargetThing;
        public Type AoETargetClass;

        public bool ReplacesStandardAttack;

        public List<StatModifier> statModifiers;
        
        public bool IsDoubleProjectile = false;
        public ThingDef doubleProjectile;

    }
}
