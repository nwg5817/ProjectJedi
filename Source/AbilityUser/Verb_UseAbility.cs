using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;


namespace AbilityUser
{
    public class Verb_UseAbility : Verb_LaunchProjectile
    {
        public VerbProperties_Ability useAbilityProps
        {
            get
            {
                return (VerbProperties_Ability)verbProps;
            }
        }

        public ProjectileDef_Ability abilityProjectileDef
        {
            get
            {
                return this.useAbilityProps.projectileDef as ProjectileDef_Ability;
            }
        }

        public List<LocalTargetInfo> TargetsAoE = new List<LocalTargetInfo>();

        //public Need_ForcePool soul
        //{
        //    get
        //    {
        //        return this.CasterPawn.needs.TryGetNeed<Need_Soul>();
        //    }
        //    set
        //    {

        //    }
        //}


        public CompAbilityUser abilityUserComp
        {
            get
            {
                return this.CasterPawn.TryGetComp<CompAbilityUser>();
            }
        }

        private void UpdateTargets()
        {
            this.TargetsAoE.Clear();
            if (this.useAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetAoE)
            {
                if (useAbilityProps.AoETargetClass == null)
                {
                    Log.Error("Tried to Cast AoE-Psyker Power without defining a target class");
                }

                List<Thing> targets = new List<Thing>();
                if (abilityProjectileDef != null && abilityProjectileDef.IsHealer)
                {
                    this.TargetsAoE.Add(new LocalTargetInfo(this.currentTarget.Cell));
                    targets = this.caster.Map.listerThings.AllThings.Where(x => (x.Position.InHorDistOf(caster.Position, this.useAbilityProps.range)) && (x.GetType() == this.useAbilityProps.AoETargetClass) && !x.Faction.HostileTo(Faction.OfPlayer)).ToList<Thing>();
                }
                else if ((this.useAbilityProps.AoETargetClass == typeof(Plant)) || (this.useAbilityProps.AoETargetClass == typeof(Building)))
                {
                    targets.Clear();
                    targets = this.caster.Map.listerThings.AllThings.Where(x => (x.Position.InHorDistOf(caster.Position, this.useAbilityProps.range)) && (x.GetType() == this.useAbilityProps.AoETargetClass)).ToList<Thing>();
                    foreach (Thing targ in targets)
                    {
                        LocalTargetInfo tinfo = new LocalTargetInfo(targ);
                        TargetsAoE.Add(tinfo);
                    }
                    return;
                }
                else
                {
                    targets.Clear();
                    targets = this.caster.Map.listerThings.AllThings.Where(x => (x.Position.InHorDistOf(caster.Position, this.useAbilityProps.range)) && (x.GetType() == this.useAbilityProps.AoETargetClass) && x.Faction.HostileTo(Faction.OfPlayer)).ToList<Thing>();
                }

                foreach (Thing targ in targets)
                {
                    TargetInfo tinfo = new TargetInfo(targ);
                    if (this.useAbilityProps.targetParams.CanTarget(tinfo))
                    {
                        TargetsAoE.Add(new LocalTargetInfo(targ));
                    }
                }
            }
            else
            {
                this.TargetsAoE.Clear();
                this.TargetsAoE.Add(this.currentTarget);
            }
        }

        public virtual void PostCastShot()
        {

        }

        protected override bool TryCastShot()
        {
            //     Log.Message("TryCastShot");
            this.TargetsAoE.Clear();
            UpdateTargets();
            int burstshots = this.ShotsPerBurst;
            if (this.useAbilityProps.AbilityTargetCategory != AbilityTargetCategory.TargetAoE && this.TargetsAoE.Count > 1)
            {
                this.TargetsAoE.RemoveRange(0, TargetsAoE.Count - 1);
            }
            //         Log.Message("Targeting: " + TargetsAoE.Count.ToString());
            for (int i = 0; i < TargetsAoE.Count; i++)
            {
                //         Log.Message(TargetsAoE[i].Thing.Label);
                for (int j = 0; j < burstshots; j++)
                {
                    bool? attempt = TryLaunchProjectile(this.verbProps.projectileDef, TargetsAoE[i]);
                    bool? attempt2 = true;
                    if (this.useAbilityProps.IsDoubleProjectile)
                    {
                        attempt2 = TryLaunchProjectile(this.useAbilityProps.doubleProjectile, TargetsAoE[i]);
                    }
                    if (attempt != null)
                    {
                        if (attempt == true && attempt2 == true)
                            return true;
                        if (attempt == false && attempt2 == true)
                            return false;
                        if (attempt == false && attempt2 == false)
                            return false;
                    }
                }

                abilityUserComp.TicksToCast = (int)this.useAbilityProps.SecondsToRecharge * GenTicks.TicksPerRealSecond;
                abilityUserComp.TicksToCastMax = (int)this.useAbilityProps.SecondsToRecharge * GenTicks.TicksPerRealSecond;
            }
            this.burstShotsLeft = 0;

            //Hook for modding
            PostCastShot();

            return true;
        }

        protected bool? TryLaunchProjectile(ThingDef projectileDef, LocalTargetInfo launchTarget)
        {
            ShootLine shootLine;
            bool flag = base.TryFindShootLineFromTo(this.caster.Position, launchTarget, out shootLine);
            if (this.verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }
            Vector3 drawPos = this.caster.DrawPos;
            Projectile projectile = (Projectile)GenSpawn.Spawn(projectileDef, shootLine.Source, this.caster.Map);
            projectile.FreeIntercept = (this.canFreeInterceptNow && !projectile.def.projectile.flyOverhead);
            ShotReport shotReport = ShotReport.HitReportFor(this.caster, this, launchTarget);
            if (!this.useAbilityProps.AlwaysHits)
            {
                if (Rand.Value > shotReport.ChanceToNotGoWild_IgnoringPosture)
                {
                    if (DebugViewSettings.drawShooting)
                    {
                        MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToWild", -1f);
                    }
                    shootLine.ChangeDestToMissWild();
                    if (launchTarget.HasThing)
                    {
                        projectile.ThingToNeverIntercept = launchTarget.Thing;
                    }
                    if (!projectile.def.projectile.flyOverhead)
                    {
                        projectile.InterceptWalls = true;
                    }
                    //              Log.Message("LaunchingIntoWild");
                    projectile.Launch(this.caster, drawPos, shootLine.Dest, this.ownerEquipment);
                    return true;
                }
                if (Rand.Value > shotReport.ChanceToNotHitCover)
                {
                    if (DebugViewSettings.drawShooting)
                    {
                        MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToCover", -1f);
                    }
                    if (launchTarget.Thing != null && launchTarget.Thing.def.category == ThingCategory.Pawn)
                    {
                        Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                        if (!projectile.def.projectile.flyOverhead)
                        {
                            projectile.InterceptWalls = true;
                        }
                        //            Log.Message("LaunchingINtoCover");
                        projectile.Launch(this.caster, drawPos, randomCoverToMissInto, this.ownerEquipment);
                        return true;
                    }
                }
            }
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(this.caster.DrawPos, this.caster.Map, "ToHit", -1f);
            }
            if (!projectile.def.projectile.flyOverhead)
            {
                projectile.InterceptWalls = (!launchTarget.HasThing || launchTarget.Thing.def.Fillage == FillCategory.Full);
            }
            if (launchTarget.Thing != null)
            {
                //                Log.Message("Release Shot at: " + launchTarget.Thing.Label);

                if (this.useAbilityProps.DrawProjectileOnTarget)
                {
                    Projectile_Ability wprojectile = projectile as Projectile_Ability;
                    if (wprojectile != null)
                    {
                        //                      Log.Message("Launched Warpprojectile");
                        wprojectile.selectedTarget = launchTarget.Thing;
                        wprojectile.Caster = this.CasterPawn;
                        wprojectile.Launch(this.caster, drawPos, launchTarget);
                    }
                }
                else
                {
                    //              Log.Message("Launched Projectile");
                    projectile.Launch(this.caster, drawPos, launchTarget);
                }
            }
            else
            {
                if (this.useAbilityProps.DrawProjectileOnTarget)
                {
                    Projectile_Ability wprojectile = projectile as Projectile_Ability;
                    wprojectile.targetVec = shootLine.Dest.ToVector3();
                    wprojectile.Launch(this.caster, drawPos, launchTarget);
                }
                //                   Log.Message("LaunchingWild");
                projectile.Launch(this.caster, drawPos, shootLine.Dest);
            }
            return null;

        }

        protected override int ShotsPerBurst
        {
            get
            {
                return this.verbProps.burstShotCount;
            }
        }

        public override void WarmupComplete()
        {
            this.burstShotsLeft = this.ShotsPerBurst;
            this.state = VerbState.Bursting;
            this.TryCastNextBurstShot();
        }
    }
}
