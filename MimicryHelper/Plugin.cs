using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;

// TODO: - interface in the PluginUI
// concrete here
// inject into PluginUI 
// concrete takes in PluginInterface, shouldn't need CommandManager since it's just to bring up the menu
// show 3 buttons in the UI
// define enum for roles
// interface is for a func that lets us target a character whose current job matches button role

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

    public sealed unsafe class Plugin : IDalamudPlugin
    {
        public string Name => "Mimicry Helper";

        private const string commandName = "/mimic";

        //private Configuration Configuration { get; init; }
        private PluginUI PluginUi { get; init; }

#if DEBUG
        private delegate IntPtr UseActionDelegate(ActionManager* actionManager, ActionType actionType, uint actionID, long targetID, uint a4, uint a5, uint a6, void* a7);

        private readonly Hook<UseActionDelegate>? useActionHook;
#endif

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            Services.Initialize(pluginInterface);

            //this.Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            //this.Configuration.Initialize(Services.PluginInterface);

            // you might normally want to embed resources and load them from the manifest stream
            this.PluginUi = new PluginUI();// this.Configuration);

            Services.Commands.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Bring up the main menu with buttons for Tank, Healer, and DPS roles."
            });

            Services.PluginInterface.UiBuilder.Draw += DrawUI;
            Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;


#if DEBUG
            try
            {
                useActionHook = new Hook<UseActionDelegate>((IntPtr)ActionManager.fpUseAction, UseActionDetour);
                useActionHook.Enable();

                PluginLog.Log("Success");
            }
            catch (Exception e)
            {
                PluginLog.Log("Error:\n" + e);
            }
#endif
        }

        public void Dispose()
        {
#if DEBUG
            useActionHook?.Dispose();
#endif
            this.PluginUi.Dispose();
            Services.Commands.RemoveHandler(commandName);
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

#if DEBUG
        private IntPtr UseActionDetour(ActionManager* actionManager, ActionType actionType, uint actionID, long targetID, uint a4, uint a5, uint a6, void* a7)
        {
            Services.Chat.Print($"actionType = {actionType}, actionID = {actionID}, targetID = {targetID}, a4 = {a4}, a5 = {a5}, a6 = {a6}, a7 = {new IntPtr(a7):X}");

            return useActionHook!.Original(actionManager, actionType, actionID, targetID, a4, a5, a6, a7);
        }
#endif
    }
}
