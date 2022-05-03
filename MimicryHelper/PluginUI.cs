using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using System;
using System.Numerics;

namespace MimicryHelper
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    unsafe class PluginUI : IDisposable
    {
        private Configuration configuration;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return this.settingsVisible; }
            set { this.settingsVisible = value; }
        }

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
            DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 330), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 330), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("My Amazing Window", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (ImGui.Button("Mimic Tank"))
                {
                    //SettingsVisible = true;



                    //Services.Chat.Print($"{(Services.Objects[0] as PlayerCharacter)?.ClassJob.GameData?.Role.ToString()  ?? "Empty"}");
                    // TANK = 1
                    // HEALER = 4
                    // LIMITED DPS = 3
                    // DPS = 2
                    // Craft = 0
                    const uint AethericMimicryActionID = 18322;

                    foreach (GameObject gameObject in Services.Objects)
                    {
                        if ((gameObject as PlayerCharacter)?.ClassJob.GameData?.Role == 1)
                        {
                            Services.Chat.Print($"Attempting to mimick {gameObject.Name}...");
                            ActionManager.Instance()->UseAction(ActionType.Spell, AethericMimicryActionID, gameObject.ObjectId);
                            break;
                        }
                    }
                }
            }
            ImGui.End();
        }

        public void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(232, 75), ImGuiCond.Always);
            if (ImGui.Begin("Mimicry Helper Configuration", ref this.settingsVisible,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                // make configuration here (reference git history or SamplePlugin for original example)

                // can save immediately on change, if you don't want to provide a "Save and Close" button
                //this.configuration.Save();
            }
            ImGui.End();
        }
    }
}
