using Dalamud.Game.Command;
using Dalamud.Plugin;
using System.Collections.Generic;

namespace MimicryHelper
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Mimicry Helper";

        private const string commandName = "/mimic";

        private Configuration Configuration { get; init; }
        private MimicryMaster MimicryMaster { get; init; }
        private PluginUI PluginUi { get; init; }

        

#if DEBUG
        private delegate IntPtr UseActionDelegate(ActionManager* actionManager, ActionType actionType, uint actionID, long targetID, uint a4, uint a5, uint a6, void* a7);

        private readonly Hook<UseActionDelegate>? useActionHook;
#endif

        public Plugin(DalamudPluginInterface pluginInterface)
        {
            Services.Initialize(pluginInterface);

            this.Configuration = Services.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(Services.PluginInterface);

            this.MimicryMaster = new Gogo();

            // you might normally want to embed resources and load them from the manifest stream
            this.PluginUi = new PluginUI(this.Configuration, MimicryMaster);

            Services.Commands.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Bring up the main menu with buttons for Tank, Healer, and DPS roles. You can specify the first letter of the role as an argument (e.g. /mimic t)."
            });

            Services.PluginInterface.UiBuilder.Draw += DrawUI;
            //Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;


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
            var roleMap = new Dictionary<string, MimicryRole>()
            {
                { "t", MimicryRole.Tank },
                { "h", MimicryRole.Healer },
                { "d", MimicryRole.Dps }
            };

            if (roleMap.ContainsKey(args))
            {
                MimicryMaster.MimicRole(roleMap[args]);  
            }
            else
            {
                // in response to the slash command, just display our main ui
                this.PluginUi.Visible = !this.PluginUi.Visible;
            }
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
