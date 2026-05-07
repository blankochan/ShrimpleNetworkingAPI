using MelonLoader;

[assembly: MelonInfo(typeof(ShrimpleNetworkingAPI.Main), ShrimpleNetworkingAPI.BuildInfo.Name, ShrimpleNetworkingAPI.BuildInfo.Version, ShrimpleNetworkingAPI.BuildInfo.Author, ShrimpleNetworkingAPI.BuildInfo.SourceURL)]
[assembly: MelonColor(255, 255, 170, 238)]

namespace ShrimpleNetworkingAPI;

public class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        Registration.TryRegister($"{BuildInfo.Author}.{BuildInfo.Name}", BuildInfo.Version, requiredForJoin: true);
        LoggerInstance.Msg("ShrimpleNetworkingAPI Loaded");
    }
}
