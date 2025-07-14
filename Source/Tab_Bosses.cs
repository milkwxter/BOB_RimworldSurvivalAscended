using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOB_ArkMod
{
    using RimWorld;
    using Verse;
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Assertions;
    using Verse.Noise;

    namespace BOB_ArkMod
    {
        public static class SpecimenBossTabDrawer
        {
            public static List<BossEntry> bossList = new List<BossEntry>
            {
                new BossEntry("Turkey", "Unlock narcoberries"),
                new BossEntry("Megasloth", "Unlock mutations"),
                new BossEntry("Thrumbo", "Gain thrumbo mutation")
            };

            public static void DrawBossTab(Rect rect, HediffComp_SpecimenImplant comp)
            {
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(rect);

                foreach (var boss in bossList)
                {
                    Rect rowRect = listing.GetRect(100f);
                    DrawBossRow(rowRect, boss, comp);
                }

                listing.End();
            }

            private static void DrawBossRow(Rect rect, BossEntry boss, HediffComp_SpecimenImplant comp)
            {
                // draw the box outline
                Widgets.DrawBoxSolidWithOutline(rect, Widgets.WindowBGFillColor, Color.grey);

                // easy variables
                float centerY = rect.y + (rect.height - 64f) / 2f;

                // do the icon box
                Rect iconRect = new Rect(rect.x + 10f, centerY, 64f, 64f);
                GUI.DrawTexture(iconRect, ContentFinder<Texture2D>.Get("UI/Widgets/DesButBG"));
                Texture2D icon = ThingDef.Named(boss.defName).uiIcon;
                if (icon != null)
                    Widgets.DrawTextureFitted(iconRect, icon, 1f);

                // do the boss name
                Rect nameRect = new Rect(rect.x + 84f, rect.y + 10f, 200f, 30f);
                Text.Font = GameFont.Medium;
                string label = DefDatabase<PawnKindDef>.GetNamedSilentFail(boss.defName)?.label ?? "ERROR: CAN'T FIND DEF NAME!";
                Widgets.Label(nameRect, boss.defName);

                // do the boss reward
                Rect rewardRect = new Rect(rect.x + 84f, rect.y + 45f, 300f, 20f);
                Text.Font = GameFont.Small;
                Widgets.Label(rewardRect, $"Reward: {boss.rewardDesc}");

                // do the defeated status
                Rect defeatRect = new Rect(rect.x + 84f, rect.y + 60f, 300f, 20f);
                bool defeated = comp.HasBossBeenDefeated(boss.defName);
                Widgets.Label(defeatRect, $"Defeated status: {defeated}");

                // do the HUNT button
                Rect huntButtonRect = new Rect(rect.width - 74f, centerY, 64f, 64f);
                if (Widgets.ButtonText(huntButtonRect, "Spawn"))
                {
                    Messages.Message($"Spawning {boss.defName}...", MessageTypeDefOf.PositiveEvent);

                    // temp spawn logic
                    Pawn specimenPawn = comp.Pawn;
                    Pawn bossPawn = PawnGenerator.GeneratePawn(PawnKindDef.Named(boss.defName));
                    if (!bossPawn.AllComps.Any(c => c is Comp_ArkBossMarker))
                        bossPawn.AllComps.Add(new Comp_ArkBossMarker());
                    GenSpawn.Spawn(bossPawn, specimenPawn.Position, specimenPawn.Map);
                }
            }
        }

        public class BossEntry
        {
            public string defName;
            public string rewardDesc;

            public BossEntry(string defName, string rewardDesc)
            {
                this.defName = defName;
                this.rewardDesc = rewardDesc;
            }
        }
    }

}
