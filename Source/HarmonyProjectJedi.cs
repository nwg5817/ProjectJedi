using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using System.Reflection;
using UnityEngine;

namespace ProjectJedi
{
    [StaticConstructorOnStartup]
    static class HarmonyProjectJedi
    {
        static HarmonyProjectJedi()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.projectjedi");

            harmony.Patch(typeof(Pawn).GetMethod("GetGizmos"), null, new HarmonyMethod(typeof(HarmonyProjectJedi).GetMethod("GetGizmosPrefix")));
            harmony.Patch(typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"), null, new HarmonyMethod(typeof(HarmonyProjectJedi).GetMethod("DrawEquipmentAimingPostFix")));
        }

        public static void DrawEquipmentAimingPostFix(PawnRenderer __instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            float num = aimAngle - 90f;
            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            num %= 360f;

            Pawn pawn = (Pawn)AccessTools.Field(typeof(PawnRenderer), "pawn").GetValue(__instance);

            Pawn_EquipmentTracker pawn_EquipmentTracker = pawn.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                    if (compActivatableEffect != null)
                    {
                        //Log.Message("4");
                        if (compActivatableEffect.Graphic != null)
                        {
                            if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Activated)
                            {
                                Material matSingle = compActivatableEffect.Graphic.MatSingle;
                                Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Gizmo> gizmoGetter(CompActivatableEffect compActivatableEffect)
        {
            //Log.Message("5");
            if (compActivatableEffect.GizmosOnEquip)
            {
                //Log.Message("6");
                //Iterate EquippedGizmos
                IEnumerator<Gizmo> enumerator = compActivatableEffect.EquippedGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    //Log.Message("7");
                    Gizmo current = enumerator.Current;
                    yield return current;
                }
            }
        }

        public static void GetGizmosPrefix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            //Log.Message("1");
            Pawn_EquipmentTracker pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                //Log.Message("2");
                ThingWithComps thingWithComps = (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                if (thingWithComps != null)
                {
                    //Log.Message("3");
                    CompActivatableEffect compActivatableEffect = thingWithComps.GetComp<CompActivatableEffect>();
                    if (compActivatableEffect != null)
                    {
                        //Log.Message("4");
                        if (__instance != null)
                        {
                            if (__instance.Faction == Faction.OfPlayer)
                            {
                                __result = __result.Concat<Gizmo>(gizmoGetter(compActivatableEffect));
                            }
                            else
                            {
                                if (compActivatableEffect.CurrentState == CompActivatableEffect.State.Deactivated)
                                {
                                    compActivatableEffect.Activate();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
