using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace AbilityUser
{
    public class JobDriver_CastAbilitySelf : JobDriver
    {
        private CompAbilityUser compAbilityUser
        {
            get
            {
                return this.pawn.TryGetComp<CompAbilityUser>();
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {

            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
            yield return Toils_Combat.CastVerb(TargetIndex.A, true);
            compAbilityUser.IsActive = true;

            this.AddFinishAction(() =>
            {
                if (compAbilityUser.IsActive)
                {
                    //PsykerUtility.PsykerShockEvents(compAbilityUser, compAbilityUser.curPower.PowerLevel);
                }
                compAbilityUser.IsActive = false;
                compAbilityUser.ShotFired = true;
            });
        }
    }
}
