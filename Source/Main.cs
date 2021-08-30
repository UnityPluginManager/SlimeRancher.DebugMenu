
using System.Reflection;
using SRML;
using SRML.SR;

namespace SrDebugDirector
{
    public class Main : ModEntryPoint
    {
        public static Assembly execAssembly = Assembly.GetExecutingAssembly();

        public override void PreLoad()
        {
            HarmonyInstance.PatchAll(execAssembly);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x0000208C File Offset: 0x0000028C
        public override void Load()
        {
            SRCallbacks.OnSaveGameLoaded += context => SRSingleton<SceneContext>.Instance.Player.AddComponent<SrDebugDirector>();
        }
		
    }
}