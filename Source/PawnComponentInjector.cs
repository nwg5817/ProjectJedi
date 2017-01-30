using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ProjectJedi
{ 
    [StaticConstructorOnStartup]
    public class PawnComponentInjectorBehavior : MonoBehaviour
    {
        static PawnComponentInjectorBehavior()
        {
            GameObject initializer = new UnityEngine.GameObject("PawnAbilityCompInjector");
            initializer.AddComponent<PawnComponentInjectorBehavior>();
            UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)initializer);
        }

        protected float reinjectTime = 0;
        int lastTicks;

        public void FixedUpdate()
        {
            try
            {
                if (Find.TickManager != null)
                {
                    if (Find.TickManager.TicksGame > lastTicks + 10)
                    {
                        lastTicks = Find.TickManager.TicksGame;
                        reinjectTime -= Time.fixedDeltaTime;
                        if (reinjectTime <= 0)
                        {
                            reinjectTime = 0;
                            if (Find.Maps != null)
                            {
                                Find.Maps.ForEach(delegate (Map map)
                                {
                                    List<Pawn> pawns = map.mapPawns.AllPawnsSpawned.Where((Pawn p) => p.story != null).ToList();
                                    pawns.Where((Pawn p) => p.Name != null && p.TryGetComp<PawnComponent_AbilityUser>() == null &&
                                            p.story.traits.HasTrait(TraitDef.Named("PJ_ForceUser"))).ToList().ForEach(
                                        delegate (Pawn p)
                                        {
                                            PawnComponent_AbilityUser pca = new PawnComponent_AbilityUser();
                                            pca.parent = p;
                                            p.AllComps.Add(pca);
                                        });
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

        }
    }
}