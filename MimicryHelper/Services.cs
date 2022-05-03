using Dalamud.Game.Gui;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
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
        public static CommandManager Commands { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static ObjectTable Objects { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static ChatGui Chat { get; private set; } = null!;
    }
}
