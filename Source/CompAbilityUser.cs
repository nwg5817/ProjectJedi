using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace ProjectJedi
{
    /*
    "This class is primarily formed from code made by Cpt. Ohu for his Warhammer 40k mod.
     Credit goes where credit is due.
     Bless you, Ohu."
                                    -Jecrell
    */

    [StaticConstructorOnStartup]
    public class PawnComponent_AbilityUser : CompUseEffect
    {

        public int TicksToCastMax = 100;
        public int TicksToCast = 0;

        static PawnComponent_AbilityUser()
        {

        }


        public Pawn forceUser
        {
            get
            {
                return this.parent as Pawn;
            }
        }

        public Need_ForcePool forcePool
        {
            get
            {
                Need_ForcePool s = forceUser.needs.TryGetNeed<Need_ForcePool>();
                if (s != null)
                {
                    return s;
                }
                return null;
            }
        }

        public CompProperties_ForceUser Props
        {
            get
            {
                return (CompProperties_ForceUser)props;
            }
        }


        public List<PawnAbility> Powers = new List<PawnAbility>();

        public List<PawnAbility> temporaryWeaponPowers = new List<PawnAbility>();

        public List<PawnAbility> temporaryApparelPowers = new List<PawnAbility>();

        public List<PawnAbility> allPowers = new List<PawnAbility>();

        public Dictionary<PawnAbility, Verb_CastPawnAbility> pawnAbilities = new Dictionary<PawnAbility, Verb_CastPawnAbility>();

        public List<Verb_CastPawnAbility> AbilityVerbs = new List<Verb_CastPawnAbility>();


        public IEnumerable<Command_PawnAbility> GetPawnAbilityVerbs()
        {

            List<Verb_CastPawnAbility> temp = new List<Verb_CastPawnAbility>();
            temp.AddRange(this.AbilityVerbs);
            for (int i = 0; i < allPowers.Count; i++)
            {
                int j = i;
                Verb_CastPawnAbility newverb = temp[j];
                newverb.caster = this.forceUser;
                newverb.verbProps = temp[j].verbProps;

                Command_PawnAbility command_CastPower = new Command_PawnAbility(this);
                command_CastPower.verb = newverb;
                command_CastPower.defaultLabel = allPowers[j].def.LabelCap;
                command_CastPower.defaultDesc = allPowers[j].def.description;
                command_CastPower.targetingParams = TargetingParameters.ForAttackAny();
                if (newverb.warpverbprops.PawnAbilityTargetCategory == PawnAbilityTargetCategory.TargetSelf || newverb.warpverbprops.PawnAbilityTargetCategory == PawnAbilityTargetCategory.TargetAoE)
                {
                    command_CastPower.targetingParams = TargetingParameters.ForSelf(this.forceUser);
                }
                command_CastPower.icon = allPowers[j].def.uiIcon;
                string str;
                if (FloatMenuUtility.GetAttackAction(this.forceUser, LocalTargetInfo.Invalid, out str) == null)
                {
                    command_CastPower.Disable(str.CapitalizeFirst() + ".");
                }
                command_CastPower.action = delegate (Thing target)
                {
                    Action attackAction = PawnComponent_AbilityUser.TryCastPowerAction(abilityUser, target, this, newverb, allPowers[j].def as AbilityDef);
                    if (attackAction != null)
                    {
                        attackAction();
                    }
                };
                if (newverb.caster.Faction != Faction.OfPlayer)
                {
                    command_CastPower.Disable("CannotOrderNonControlled".Translate());
                }
                if (newverb.CasterIsPawn)
                {
                    if (newverb.CasterPawn.story.DisabledWorkTags.Contains(WorkTags.Violent))
                    {
                        command_CastPower.Disable("IsIncapableOfViolence".Translate(new object[]
                        {
                            newverb.CasterPawn.NameStringShort
                        }));
                    }
                    else if (!newverb.CasterPawn.drafter.Drafted)
                    {
                        command_CastPower.Disable("IsNotDrafted".Translate(new object[]
                        {
                            newverb.CasterPawn.NameStringShort
                        }));
                    }
                    else if (!this.IsActive)
                    {
                        command_CastPower.Disable("PawnAbilityRecharging".Translate(new object[]
                            {
                                newverb.CasterPawn.NameStringShort
                            }));
                    }
                }
                yield return command_CastPower;
            }
            yield break;
        }


        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            IEnumerator<Gizmo> enumerator = base.CompGetGizmosExtra().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Gizmo current = enumerator.Current;
                yield return current;
            }

            if (this.parent.Faction == Faction.OfPlayer)
            {

            }

        }
    }

    public class CompProperties_AbilityUser : CompProperties
    {
        public CompProperties_AbilityUser()
        {
            compClass = typeof(PawnComponent_AbilityUser);
        }
    }
}
