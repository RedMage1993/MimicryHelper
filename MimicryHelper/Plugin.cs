using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Game.ClientState;
using System.Reflection;

// TODO: - interface in the PluginUI
// concrete here
// inject into PluginUI 
// concrete takes in PluginInterface, shouldn't need CommandManager since it's just to bring up the menu
// show 3 buttons in the UI
// define enum for roles
// interface is for a func that lets us target a character whose current job matches button role

namespace MimicryHelper
{
    public sealed class Dalamud
    {
        public static void Initialize(DalamudPluginInterface pluginInterface) => pluginInterface.Create<Dalamud>();

        [PluginService]
        [RequiredVersion("1.0")]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        [RequiredVersion("1.0")]
        public static CommandManager Commands { get; private set; } = null!;
    }

    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Sample Plugin";

        private const string commandName = "/pmycommand";

        private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            Dalamud.Initialize(pluginInterface);

            this.Configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(Dalamud.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(Dalamud.PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");
            var goatImage = Dalamud.PluginInterface.UiBuilder.LoadImage(imagePath);
            this.PluginUi = new PluginUI(this.Configuration, goatImage);

            Dalamud.Commands.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            Dalamud.PluginInterface.UiBuilder.Draw += DrawUI;
            Dalamud.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        public void Dispose()
        {
            this.PluginUi.Dispose();
            Dalamud.Commands.RemoveHandler(commandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            this.PluginUi.Visible = true;
        }

        private void DrawUI()
        {
            this.PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            this.PluginUi.SettingsVisible = true;
        }
    }
}
