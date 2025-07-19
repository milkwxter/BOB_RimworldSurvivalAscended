using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using System.Security.Cryptography;

namespace BOB_ArkMod
{
    public static class SpecimenBossTabDrawer
    {
        public static List<BossEntry> bossList = new List<BossEntry>
        {
            new BossEntry("BOB_Yutyrannus", "Unlock mutations"),
            new BossEntry("BOB_YutyrannusAlpha", "Unlock alpha mode"),
            new BossEntry("Thrumbo", "Gain new mutation")
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
            ThingDef def = ThingDef.Named(boss.defName);
            Texture2D icon = def.uiIcon;
            if (icon != null)
            {
                Color iconColor = Color.white;

                if (def.comps != null && def.comps.Any(c => c.compClass == typeof(BOB_ArkMod.Comp_ArkAlphaMarker)))
                {
                    iconColor = new Color(0.8f, 0.2f, 0.2f); // Alpha Red
                }

                GUI.color = iconColor;
                Widgets.DrawTextureFitted(iconRect, icon, 1f);
                GUI.color = Color.white;
            }

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
            if (Widgets.ButtonText(huntButtonRect, "Hunt!"))
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
