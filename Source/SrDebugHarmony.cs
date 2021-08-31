using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;

namespace SrDebugDirector
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(PediaModel))]
    [HarmonyPatch("Init")]
    internal static class PediaModelInstance
    {
        // Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
        private static void Postfix(PediaModel __instance)
        {
            pediamodel = __instance;
        }

        public static PediaModel pediamodel;
    }
}
namespace SrDebugDirector
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(TutorialsModel))]
    [HarmonyPatch("Init")]
    internal static class TutorialsModelInstance
    {
        // Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
        private static void Postfix(TutorialsModel __instance)
        { 
            tutorialsmodel = __instance;
        }

        public static TutorialsModel tutorialsmodel;
    }
}
namespace SrDebugDirector
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(ProfileAchievesModel))]
    [HarmonyPatch("Init")]
    internal static class ProfileAchievesModelInstance
    {
        // Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
        private static void Postfix(ProfileAchievesModel __instance)
        {
            profileachievesmodel = __instance;

        }

        public static ProfileAchievesModel profileachievesmodel;



    }
}
namespace SrDebugDirector
{
    // Token: 0x02000004 RID: 4
    [HarmonyPatch(typeof(ProgressModel))]
    [HarmonyPatch("Init")]
    internal static class ProgressModelInstance
    {
        // Token: 0x06000007 RID: 7 RVA: 0x00002080 File Offset: 0x00000280
        private static void Postfix(ProgressModel __instance)
        {
            progressmodel = __instance;
        }
        
        public static ProgressModel progressmodel;




    }
}