using HarmonyLib;
using RimWorld;
using Verse;

namespace BOB_ArkMod
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("Kill")]
    public static class Patch_Pawn_Kill_SpecimenTracker
    {
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            // early return if pawn is null or belongs to the player faction
            if (__instance == null || __instance.Faction == Faction.OfPlayer) return;

            // get the defName of the dead pawn
            string killedDefName = __instance.kindDef.defName;

            // find killer
            Thing killerThing = dinfo.Value.Instigator;
            if (killerThing is Pawn && killerThing != null)
            {
                Pawn killerPawn = killerThing as Pawn;

                // does he have the specimen implant
                HediffComp_SpecimenImplant comp = killerPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("BOB_SpecimenImplant"))?.TryGetComp<HediffComp_SpecimenImplant>();
                if (comp == null) return;

                // if he has it, mark boss as defeated if it doesnt already exist
                if (comp.HasBossBeenDefeated(killedDefName)) return;
                comp.MarkBossAsDefeated(killedDefName);
                Messages.Message($"{killerPawn.LabelShortCap} defeated {__instance.LabelCap}!", MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}
