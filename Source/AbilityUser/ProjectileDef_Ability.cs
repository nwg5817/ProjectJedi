using Verse;


namespace AbilityUser
{
    public class ProjectileDef_Ability : ThingDef
    {
        public bool IsBeamProjectile = false;

        public bool IsMentalStateGiver = false;
        public MentalStateDef InducesMentalState;

        public bool IsBuffGiver = false;
        public HediffDef BuffDef;

        public bool IsHealer = false;
        public int HealCapacity = 3;
        public float HealFailChance = 0.3f;

    }
}