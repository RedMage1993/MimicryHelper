using Dalamud.Game.Gui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Game.Command;

namespace MimicryHelper
{
    public sealed class Services
    {
        public static void Initialize(DalamudPluginInterface pluginInterface) => pluginInterface.Create<Services>();

        [PluginService]
        [RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static ICommandManager Commands { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static IObjectTable Objects { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static IChatGui Chat { get; private set; } = null!;
    }
}
