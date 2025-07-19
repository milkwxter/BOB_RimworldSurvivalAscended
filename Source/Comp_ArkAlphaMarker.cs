using RimWorld;
using Verse;

namespace BOB_ArkMod
{
    public class Comp_ArkAlphaMarker : ThingComp
    {
        private Effecter alphaEffecter;
        private int tickCounter = 0;

        public override void CompTick()
        {
            base.CompTick();

            Pawn pawn = parent as Pawn;

            // extra cleanup
            if (pawn == null || !pawn.Spawned || pawn.Dead)
            {
                alphaEffecter?.Cleanup();
                alphaEffecter = null;
                return;
            }

            // spawn effecter if it doesnt exist
            if (alphaEffecter == null)
            {
                EffecterDef alphaEffecterDef = DefDatabase<EffecterDef>.GetNamed("BOB_Alpha", true);
                alphaEffecter = alphaEffecterDef.Spawn();
            }

            // tick the effecter
            alphaEffecter.EffectTick(pawn, TargetInfo.Invalid);

            // do the alpha smoke once a second
            tickCounter++;
            if (tickCounter >= 60)
            {
                if (pawn != null && pawn.Spawned && !pawn.Dead)
                {
                    MoteThrown mote = (MoteThrown)ThingMaker.MakeThing(ThingDef.Named("BOB_Mote_AlphaSmoke"));
                    mote.Scale = Rand.Range(0.8f, 1.2f);
                    mote.rotationRate = Rand.Range(-10f, 10f);
                    mote.exactRotation = Rand.Range(0f, 360f);
                    mote.exactPosition = pawn.DrawPos;
                    mote.SetVelocity(Rand.Range(0f, 360f), Rand.Range(0.05f, 0.1f));
                    GenSpawn.Spawn(mote, pawn.Position, pawn.Map);
                }

                tickCounter = 0;
            }
        }

        public override void PostDeSpawn(Map map, DestroyMode mode)
        {
            base.PostDeSpawn(map, mode);
            alphaEffecter?.Cleanup();
            alphaEffecter = null;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            alphaEffecter?.Cleanup();
            alphaEffecter = null;
        }
    }

    public class CompProperties_ArkAlphaMarker : CompProperties
    {
        public CompProperties_ArkAlphaMarker()
        {
            this.compClass = typeof(Comp_ArkAlphaMarker);
        }
    }
}
