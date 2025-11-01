using Il2CppSteamworks;
using MelonLoader;
using ReplantedOnline.Network.Online;

namespace ReplantedOnline;

internal class ReplantedOnlineMod : MelonMod
{
    internal static readonly HarmonyLib.Harmony _Harmony = new("com.d1gq.onlinemod");

    public override void OnUpdate()
    {
        if (!loaded) return;

        NetworkDispatcher.Update();
    }

    private bool loaded;
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "Frontend")
        {
            if (loaded) return;
            loaded = true;

            if (!SteamClient.initialized)
            {
                SteamClient.Init(3654560);
            }
            NetLobby.Initialize();
        }
    }
}
