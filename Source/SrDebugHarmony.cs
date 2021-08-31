using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SrDebugDirector
{
    [HarmonyPatch(typeof(PediaModel))]
    [HarmonyPatch("Init")]
    internal static class PediaModelInstance
    {
        private static void Postfix(PediaModel __instance)
        {
            pediamodel = __instance;
        }

        public static PediaModel pediamodel;
    }
}
namespace SrDebugDirector
{
    [HarmonyPatch(typeof(TutorialsModel))]
    [HarmonyPatch("Init")]
    internal static class TutorialsModelInstance
    {
        private static void Postfix(TutorialsModel __instance)
        { 
            tutorialsmodel = __instance;
        }

        public static TutorialsModel tutorialsmodel;
    }
}
namespace SrDebugDirector
{
    [HarmonyPatch(typeof(ProfileAchievesModel))]
    [HarmonyPatch("Init")]
    internal static class ProfileAchievesModelInstance
    {
        private static void Postfix(ProfileAchievesModel __instance)
        {
            profileachievesmodel = __instance;

        }

        public static ProfileAchievesModel profileachievesmodel;
    }
}
namespace SrDebugDirector
{
    [HarmonyPatch(typeof(ProgressModel))]
    [HarmonyPatch("Init")]
    internal static class ProgressModelInstance
    {
        private static void Postfix(ProgressModel __instance)
        {
            progressmodel = __instance;
        }
        
        public static ProgressModel progressmodel;
    }
}
