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
        public static void Initialize(IDalamudPluginInterface pluginInterface) => pluginInterface.Create<Services>();

        [PluginService]
        public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        public static ICommandManager Commands { get; private set; } = null!;

        [PluginService]
        public static IObjectTable Objects { get; private set; } = null!;

        [PluginService]
        public static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        public static IChatGui Chat { get; private set; } = null!;

        [PluginService]
        public static IPluginLog PluginLog { get; private set; } = null!;

        [PluginService]
        public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    }
}
