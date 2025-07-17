using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace BOB_ArkMod
{
    public enum SpecimenTab
    {
        Bosses,
        Mutations,
        Engrams
    }

    public class Dialog_SpecimenInterface : Window
    {
        private SpecimenTab selectedTab = SpecimenTab.Bosses;
        private HediffComp_SpecimenImplant compSpecimen;
        private Pawn pawn;
        private List<TabRecord> tabs;

        public Dialog_SpecimenInterface(HediffComp_SpecimenImplant comp)
        {
            compSpecimen = comp;
            pawn = comp.parent.pawn;
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            tabs = new List<TabRecord>
            {
                new TabRecord("Bosses", () => selectedTab = SpecimenTab.Bosses, () => selectedTab == SpecimenTab.Bosses),
                new TabRecord("Mutations", () => selectedTab = SpecimenTab.Mutations, () => selectedTab == SpecimenTab.Mutations),
                new TabRecord("Engrams", () => selectedTab = SpecimenTab.Engrams, () => selectedTab == SpecimenTab.Engrams)
            };
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(inRect.TopPartPixels(60f), "Specimen Interface: " + pawn.Name);

            Rect tabsRect = new Rect(inRect.x, inRect.y + 65f, inRect.width, 40f);
            TabDrawer.DrawTabs(tabsRect, tabs);

            Rect contentRect = new Rect(inRect.x + 10f, inRect.y + 90f, inRect.width - 20f, inRect.height - 100f);
            DrawSelectedTab(contentRect);
        }

        private void DrawSelectedTab(Rect rect)
        {
            switch (selectedTab)
            {
                case SpecimenTab.Bosses:
                    DrawBossTab(rect);
                    break;
                case SpecimenTab.Mutations:
                    DrawMutationTab(rect);
                    break;
                case SpecimenTab.Engrams:
                    DrawEngramTab(rect);
                    break;
            }
        }

        private void DrawBossTab(Rect rect)
        {
            SpecimenBossTabDrawer.DrawBossTab(rect, compSpecimen);
        }

        private void DrawMutationTab(Rect rect)
        {
            // build the connections here
            SpecimenMutationTabDrawer.BuildMutationConnections();
            // draw the tab
            SpecimenMutationTabDrawer.DrawMutationTab(rect, compSpecimen);
        }

        private void DrawEngramTab(Rect rect)
        {
            Widgets.Label(rect, "Engrams UI");
        }
    }
}
