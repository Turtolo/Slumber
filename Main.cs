using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine;
using ConstructEngine.UI;
using ConstructEngine.Util;
using ConstructEngine.Input;
using MonoGameGum;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.ComponentModel;

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

            Input.Binds.Add("MoveLeft", [new InputAction(Keys.Left), new InputAction(Buttons.LeftStick)]);
            Input.Binds.Add("MoveRight", [new InputAction(Keys.Right), new InputAction(Buttons.RightStick)]);
            Input.Binds.Add("Jump", [new InputAction(Keys.Z), new InputAction(Buttons.A)]);
            Input.Binds.Add("Attack", [new InputAction(Keys.X), new InputAction(Buttons.Y)]);


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

            GumUI.Update(this, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SetRenderTarget();
            GraphicsDevice.Clear(Color.DarkSlateGray);
            SceneManager.DrawCurrentScene(SpriteBatch);


            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp);
            SpriteBatch.Draw(RenderTarget, new Rectangle(offsetX, offsetY, finalWidth, finalHeight), Color.White);

            SpriteBatch.End();

            GumUI.Draw();
        }

    }
}
