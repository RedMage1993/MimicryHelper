
using ImGuiNET;
using System;
using System.Numerics;

namespace MimicryHelper
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        const float UIWidth = 350;
        const float UIHeight = 75;

        const float UIButtonWidth = 100;
        const float UIButtonHeight = 25;

        private Configuration configuration;

        private MimicryMaster mimicryMaster;

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
        public PluginUI(Configuration configuration, MimicryMaster mimicryMaster)
        {
            this.configuration = configuration;
            this.mimicryMaster = mimicryMaster;
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
            //DrawSettingsWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(UIWidth, UIHeight), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(UIWidth, UIHeight), new Vector2(UIWidth, UIHeight));

            if (ImGui.Begin("Mimicry Helper", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (ImGui.Button("Mimic Tank", new Vector2(UIButtonWidth, UIButtonHeight)))
                {
                    mimicryMaster.MimicRole(MimicryRole.Tank);
                }

                ImGui.SameLine();

                if (ImGui.Button("Mimic Heal", new Vector2(UIButtonWidth, UIButtonHeight)))
                {
                    mimicryMaster.MimicRole(MimicryRole.Healer);
                }

                ImGui.SameLine();

                if (ImGui.Button("Mimic DPS", new Vector2(UIButtonWidth, UIButtonHeight)))
                {
                    mimicryMaster.MimicRole(MimicryRole.Dps);
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
                this.configuration.Save();
            }
            ImGui.End();
        }
    }
}
