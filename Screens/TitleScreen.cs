using System;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using MonoGameGum;
using Gum.Forms.Controls;
using Slumber.Components.ConstructControls;
using ConstructEngine.UI;

namespace Slumber.Screens
{
    partial class TitleScreen
    {
        partial void CustomInitialize()
        {   
            StartButton.IsFocused = true;
            Main.Visible = true;
            Settings.IsVisible = false;

            if (File.Exists(SaveManager.FileSavePath)) StartButton.Text = "Resume";
            else StartButton.Text = "Start";

            StartButton.Click += (_, _) =>
            {
                if (File.Exists(SaveManager.FileSavePath)) SaveManager.LoadData();
                else Engine.SceneManager.AddScene(new Scene1());
            };

            DeleteSaveButton.Click += (_, _) =>
            {
                File.Delete(SaveManager.FileSavePath);
                if (File.Exists(SaveManager.FileSavePath)) StartButton.Text = "Resume";
                else StartButton.Text = "Start";
            };

            QuitButton.Click += (_, _) =>
            {
                Engine.Quit();
            };

            SettingsButton.Click += (_, _) =>
            {
                Settings.IsVisible = true;
                Main.Visible = false;
                Settings.ControlButton.IsFocused = true;
            };


            Settings.MasterSlider.ValueChanged += (_, _) =>
            {
                
            };


    

            

        }

    }
}
