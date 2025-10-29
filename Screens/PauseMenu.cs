using ConstructEngine;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Slumber.Screens
{
    partial class PauseMenu
    {
        partial void CustomInitialize()
        {
            Root.Visible = false;

            ResumeButton.IsFocused = true;
            
            ResumeButton.Click += (_, _) =>
            {
                Core.SceneManager.ToggleSceneFreeze(false);
                Root.Visible = false;
            };

            QuitButton.Click += (_, _) =>
            {
                Core.SceneManager.AddScene(new MainMenu());
            };

            SettingsButton.Click += (_, _) =>
            {
                Settings.IsVisible = true;
                Main.Visible = false;
                Settings.MasterSlider.IsFocused = true;
            };

            Settings.BackButton.Click += (_, _) =>
            {
                Settings.IsVisible = false;
                Main.Visible = true;
                ResumeButton.IsFocused = true;
            };
        }
    }
}
