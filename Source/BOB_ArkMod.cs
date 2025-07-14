using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BOB_ArkMod
{
    public class BOB_Ark_Mod : Mod
    {
        public BOB_Ark_Mod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("BOBArkMod.HarmonyPatcher");
            harmony.PatchAll();
        }
    }
}
