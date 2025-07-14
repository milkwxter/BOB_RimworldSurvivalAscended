using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BOB_ArkMod
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("Kill")]
    public static class Patch_Pawn_Kill_SpecimenTracker
    {
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            // early return if victim is null or belongs to the player faction
            if (__instance == null || __instance.Faction == Faction.OfPlayer) return;

            // early return if the victim does not have the ARK boss comp
            if (__instance.TryGetComp<Comp_ArkBossMarker>() == null) return;

            // create list that stores everyone who should get credit for the fight
            HashSet<Pawn> credited = new HashSet<Pawn>();

            Log.Message("Pawn just died!");

            // go through ALL battles in the battlelog that concern the victim
            var relevantBattles = Find.BattleLog.Battles.Where(b => b.Entries.Any(e => e.GetConcerns().Contains(__instance)));
            foreach (Battle battle in relevantBattles)
            {
                Log.Message("Checking battle!");

                // go through each entry in said battle
                foreach (LogEntry entry in battle.Entries)
                {
                    string entryText = entry.ToGameStringFromPOV(__instance);
                    Log.Message($"Entry: {entryText}");

                    // get both the instigator and the victim
                    List<Thing> entryThingsConcerned = entry.GetConcerns().ToList();

                    Pawn actualVictim = null;
                    Pawn actualInstigator = null;

                    if (entry is BattleLogEntry_RangedImpact)
                    {
                        if (entryThingsConcerned.Count < 3) continue;

                        Thing instigator = entryThingsConcerned[0];
                        Thing victim = entryThingsConcerned[1];
                        Thing originalTargetPawn = entryThingsConcerned[2];

                        if (!(instigator is Pawn)) continue;
                        if (!(victim is Pawn)) continue;
                        if (!(originalTargetPawn is Pawn)) continue;

                        Log.Message(" - Ranged impact hit the victim");

                        if (victim == originalTargetPawn)
                        {
                            actualVictim = (Pawn)victim;
                            actualInstigator = (Pawn)instigator;
                        }
                    }
                    if (entry is BattleLogEntry_MeleeCombat)
                    {
                        if (entryThingsConcerned.Count < 2) continue;

                        Thing instigator = entryThingsConcerned[0];
                        Thing victim = entryThingsConcerned[1];

                        if (!(instigator is Pawn)) continue;
                        if (!(victim is Pawn)) continue;

                        Log.Message(" - Melee combat hit the victim");

                        actualVictim = (Pawn)victim;
                        actualInstigator = (Pawn)instigator;
                    }

                    // make sure the victim is actually the guy who just died
                    if (actualVictim == __instance)
                    {
                        // add him to the credits list
                        credited.Add(actualInstigator);
                    }
                }
            }

            // give credit to everyone who helped!
            foreach (Pawn pawn in credited)
            {
                // get the specimen implant comp if it exists
                var comp = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("BOB_SpecimenImplant"))?.TryGetComp<HediffComp_SpecimenImplant>();

                // if he has the component
                if (comp != null && !comp.HasBossBeenDefeated(__instance.kindDef.defName))
                {
                    comp.MarkBossAsDefeated(__instance.kindDef.defName);
                    Messages.Message($"{pawn.LabelShortCap} credited for defeating {__instance.LabelCap}.", MessageTypeDefOf.PositiveEvent);
                }
                else
                {
                    Messages.Message($"{pawn.LabelShortCap} credited for defeating {__instance.LabelCap}. But it did nothing!", MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }
}