using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BOB_ArkMod
{
    public class HediffCompProperties_SpecimenImplant : HediffCompProperties
    {
        public HediffCompProperties_SpecimenImplant()
        {
            this.compClass = typeof(HediffComp_SpecimenImplant);
        }
    }

    public class HediffComp_SpecimenImplant : HediffComp
    {
        private HashSet<string> defeatedBosses = new HashSet<string>();

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (PawnIsEligible())
            {
                yield return new Command_Action
                {
                    defaultLabel = "Open specimen interface",
                    defaultDesc = "Access implant-linked system interface.",
                    icon = ContentFinder<Texture2D>.Get("Things/Items/m_specimen_implant", true),
                    iconDrawScale = 1.5f,
                    action = () =>
                    {
                        Find.WindowStack.Add(new Dialog_SpecimenInterface(this));
                    }
                };
            }
        }

        private bool PawnIsEligible()
        {
            return this.Pawn != null && this.Pawn.health.hediffSet.HasHediff(this.parent.def);
        }

        public bool HasBossBeenDefeated(string bossDefName)
        {
            return defeatedBosses.Contains(bossDefName);
        }

        public void MarkBossAsDefeated(string bossDefName)
        {
            defeatedBosses.Add(bossDefName);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Collections.Look(ref defeatedBosses, "defeatedBosses", LookMode.Value);
        }
    }
}
