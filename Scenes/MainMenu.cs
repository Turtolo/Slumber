
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine.Util;
using ConstructEngine.UI;
using MonoGameGum;
using Slumber.Screens;
using System;
using ConstructEngine;
using ConstructEngine.Graphics;
using Gum.Forms.Controls;
using Slumber.Components.ConstructControls;
using ConstructEngine.Input;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;


namespace Slumber;


public class MainMenu : Scene, Scene.IScene
{
    TitleScreen titleScreen;
    
    public MainMenu()
    {
        
        
    }

    public void Initialize()
    {
        GumHelper.Wipe();

        titleScreen = new TitleScreen();

        GumHelper.AddScreenToRoot(titleScreen);

        ParallaxBackground.AddBackground(new("Assets/Backgrounds/streetsbg", 0.1f,  ParallaxBackground.RepeatX, new Vector2(0,0)));

    }

    public void Load()
    {

        
    }

    public void Update(GameTime gameTime)
    {



        if (Core.Input.Keyboard.WasKeyJustPressed(Microsoft.Xna.Framework.Input.Keys.X))
        {
            if (titleScreen.Settings.IsVisible)
            {
                titleScreen.Settings.IsVisible = false;
                titleScreen.Main.Visible = true;
                titleScreen.StartButton.IsFocused = true;
            }
            if (titleScreen.Controls.IsVisible)
            {
                titleScreen.Controls.IsVisible = false;
                titleScreen.Settings.IsVisible = true;
                titleScreen.Settings.ControlButton.IsFocused = true;
            }
        }

        titleScreen.Controls.Update();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        ParallaxBackground.DrawParallaxBackgrounds(spriteBatch, Core.GraphicsDevice, SamplerState.LinearWrap);
    }

}
