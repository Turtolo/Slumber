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
    public class Main : Core 
    {
        bool bloomEnabled = true;
        
        private GumService GumUI;

        public Main() : base("Platformer", 640, 360, false, "Assets/Fonts/Font")
        {
            Window.AllowUserResizing = true;
            Window.IsBorderless = false;

            var displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            Graphics.PreferredBackBufferWidth = displayMode.Width;
            Graphics.PreferredBackBufferHeight = displayMode.Height;


            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.ApplyChanges();

            Window.Position = new Point(0, 0);
        }


        protected override void Initialize()
        {
            GumUI = GumHelper.GumInitialize(this, "GumProject/GumProject.gumx");

            base.Initialize();

            UpdateRenderTargetTransform();
            UpdateGumCamera();

            var binds = new Dictionary<string, List<InputAction>>
            {
                {"MoveLeft", [new InputAction(Keys.Left), new InputAction(Buttons.DPadLeft)]},
                {"MoveRight", [new InputAction(Keys.Right), new InputAction(Buttons.DPadRight)]},
                {"Jump", [new InputAction(Keys.Z), new InputAction(Buttons.A)]},
                {"Attack", [new InputAction(Keys.X), new InputAction(Buttons.Y), new InputAction(MouseButton.Left)]},
                {"Pause", [new InputAction(Keys.Escape), new InputAction(Buttons.Start)]},
                {"Back", [new InputAction(Keys.X), new InputAction(Buttons.B)]}
                
            };

            Input.InitializeBinds(binds);


            SceneManager.AddScene(new MainMenu());
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.Keyboard.WasKeyJustPressed(Keys.F1))
                bloomEnabled = !bloomEnabled;

            SceneManager.UpdateCurrentScene(gameTime);

            GumManager.UpdateAll(gameTime);
            GumUI.Update(this, gameTime);


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SetRenderTarget();
            GraphicsDevice.Clear(Color.DarkSlateGray);
            SceneManager.DrawCurrentScene(SpriteBatch);

            base.Draw(gameTime);

            GumUI.Draw();
        }

    }
}
