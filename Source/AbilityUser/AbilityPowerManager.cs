using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace AbilityUser
{
    public class AbilityPowerManager : IExposable
    {
        public void AddPawnAbility(AbilityDef psydef)
        {
            //Log.Message("Add Pawn Ability Called");
            if (!this.CompAbilityUser.Powers.Any(x => x.def.defName == psydef.defName))
            {
                //Log.Message("Added Ability " + psydef.label);
                this.CompAbilityUser.Powers.Add(new PawnAbility(this.CompAbilityUser.abilityUser, psydef));
            }

            this.CompAbilityUser.UpdateAbilities();
        }

        //public int PowerSlotsIota = 4;
        //public int PowerSlotsZeta = 3;
        //public int PowerSlotsEpsilon = 2;
        //public int PowerSlotsDelta = 1;

        public AbilityPowerManager(CompAbilityUser CompAbilityUser)
        {
            this.CompAbilityUser = CompAbilityUser;
        }

        public void Initialize()
        {
            //this.PowerLevelSlots = new Dictionary<PawnAbilityLevel, int>();
            //this.PowerLevelSlots.Add(PawnAbilityLevel.Iota, PowerSlotsIota);
            //this.PowerLevelSlots.Add(PawnAbilityLevel.Zeta, PowerSlotsZeta);
            //this.PowerLevelSlots.Add(PawnAbilityLevel.Epsilon, PowerSlotsEpsilon);
            //this.PowerLevelSlots.Add(PawnAbilityLevel.Delta, PowerSlotsDelta);
        }

        private CompAbilityUser CompAbilityUser;

        //public Dictionary<PawnAbilityLevel, int> PowerLevelSlots = new Dictionary<PawnAbilityLevel, int>();

        public void AbilityPowerManagerTick()
        {
        }

        //public bool CheckAvailablePowerSlots(PawnAbilityLevel leveltocheck)
        //{
        //    int powers = CompAbilityUser.Powers.FindAll(x => x.powerdef.PowerLevel == leveltocheck).Count;
        //    int availableslots = (from entry in PowerLevelSlots where entry.Key == leveltocheck select entry.Value).FirstOrDefault();
        //    if (powers <= availableslots)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public void ExposeData()
        {
            //Scribe_Collections.LookDictionary<PawnAbilityLevel, int>(ref this.PowerLevelSlots, "PowerLevelSlots", LookMode.Value, LookMode.Value);
            Scribe_Values.LookValue<CompAbilityUser>(ref this.CompAbilityUser, "CompAbilityUser", null);
        }

        public List<PawnAbility> powersint = new List<PawnAbility>();

        public List<PawnAbility> Powers = new List<PawnAbility>();

    }
}
