using RimWorld;
using Verse;

namespace BOB_ArkMod
{
    public class Hediff_BleedingBoost : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            if (pawn != null && !pawn.Dead)
            {
                Hediff existing = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.BloodLoss);

                if (existing != null)
                {
                    existing.Severity += 0.05f;
                }
                else
                {
                    Hediff bloodLoss = HediffMaker.MakeHediff(HediffDefOf.BloodLoss, pawn);
                    bloodLoss.Severity = 0.05f;
                    pawn.health.AddHediff(bloodLoss);
                }
            }
        }

    }
}