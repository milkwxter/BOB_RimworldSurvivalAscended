using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BOB_ArkMod
{
    public class CompAbilityEffect_OnlyTargetFriendlies : CompAbilityEffect
    {
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            if (parent.pawn != null)
            {
                return !parent.pawn.HostileTo(target.Thing);
            }
            return false;
        }

        public override bool Valid(GlobalTargetInfo target, bool throwMessages = false)
        {
            if (parent.pawn != null)
            {
                return !parent.pawn.HostileTo(target.Thing);
            }
            return false;
        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            if (parent.pawn != null)
            {
                return !parent.pawn.HostileTo(target.Thing);
            }
            return false;
        }
    }
}
