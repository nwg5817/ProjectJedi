using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace ProjectJedi
{
    public class AbilityDef : ThingDef
    {

        public int RechargeTicks;

        public int CastTime = 0;

        public string IconGraphicPath;

        public VerbProperties_PawnAbility MainVerb;
    }
}
