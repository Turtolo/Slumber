using System;
using ConstructEngine;
using ConstructEngine.Components;
using ConstructEngine.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Slumber.Components.ConstructControls;
using Slumber.Screens;

namespace Slumber.Logic;

public class Pausemenu : BackseatComponent
{
    private PauseMenu Menu;

    public Pausemenu()
    {
        Menu = new PauseMenu();
        GumHelper.AddScreenToRoot(Menu);
    }
    
    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.SceneManager.QueeFreezeCurrentScene();
            Menu.Root.Visible = true;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.X))
        {
            if (Menu.Main.Visible)
            {
                Core.SceneManager.UnFreezeCurrentScene();
                Menu.Root.Visible = false;
            }
            if (Menu.Settings.IsVisible)
            {
                Menu.Settings.IsVisible = false;
                Menu.Main.Visible = true;
                Menu.ResumeButton.IsFocused = true;
            }
            if (Menu.Controls.IsVisible)
            {
                Menu.Controls.IsVisible = false;
                Menu.Settings.IsVisible = true;
                Menu.Settings.ControlButton.IsFocused = true;
            }
        }
        
    }
}
