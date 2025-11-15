using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine;
using ConstructEngine.UI;
using ConstructEngine.Util;
using ConstructEngine.Input;
using MonoGameGum;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using ConstructEngine.Helpers;
using System;

namespace Slumber
{
    public class Main : Engine 
    {
        public Main() : base(new EngineConfig
        {
            Title = "Slumber",
            VirtualWidth = 640,
            VirtualHeight = 360,
            Fullscreen = false,
            IntegerScaling = true,
            AllowUserResizing = true,
            IsBorderless = true,
            IsFixedTimeStep = false,
            SynchronizeWithVerticalRetrace = true,
            FontPath = "Assets/Fonts/Font",
            GumProject = "GumProject/GumProject.gumx",

        }) {  }

        protected override void Initialize()
        {
            base.Initialize();
            SceneManager.AddScene(new MainMenu());
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
