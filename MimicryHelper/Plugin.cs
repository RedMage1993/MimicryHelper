using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager;

namespace MimicryHelper
{
    public sealed unsafe class Plugin : IDalamudPlugin
    {
        public static string Name => "Mimicry Helper";

        private const string commandName = "/mimic";

        private Configuration Configuration { get; init; }
        private Gogo MimicryMaster { get; init; }
        private PluginUI PluginUi { get; init; }



#if DEBUG
        private delegate IntPtr UseActionDelegate(ActionManager* actionManager, ActionType actionType, uint actionID, ulong targetID, uint a4, UseActionMode a5, uint a6, bool* a7, bool a8);

        private readonly Hook<UseActionDelegate>? useActionHook;
#endif

        public Plugin(IDalamudPluginInterface pluginInterface)
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
                useActionHook = Services.GameInteropProvider.HookFromAddress<UseActionDelegate>((IntPtr)ActionManager.MemberFunctionPointers.UseAction, UseActionDetour);
                useActionHook?.Enable();

                Services.PluginLog.Debug("Success");
            }
            catch (Exception e)
            {
                Services.PluginLog.Debug("Error:\n" + e);
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
            var roleMap = new Dictionary<string, IMimicryRole>()
            {
                { "t", new TankMimicryRole() },
                { "d", new DpsMimicryRole() },
                { "h", new HealerMimicryRole() }
            };

            if (roleMap.TryGetValue(args, out IMimicryRole? value))
            {
                MimicryMaster.MimicRole(value);  
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

        //private void DrawConfigUI()
        //{
        //    this.PluginUi.SettingsVisible = true;
        //}

#if DEBUG
        private IntPtr UseActionDetour(ActionManager* actionManager, ActionType actionType, uint actionID, ulong targetID, uint a4, UseActionMode a5, uint a6, bool* a7, bool a8)
        {
            if (Services.Objects.SearchById((uint)targetID) is IPlayerCharacter playerCharacter && playerCharacter.ClassJob.IsValid)
            {
                Services.Chat.Print($"actionType = {actionType}, actionID = {actionID}, targetID = {targetID}, mode = {a5}, role = {playerCharacter.ClassJob.Value.Role.ToString() ?? "?"}");
            }

            return useActionHook!.Original(actionManager, actionType, actionID, targetID, a4, a5, a6, a7, a8);
        }
#endif
    }
}
