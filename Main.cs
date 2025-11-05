using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine;
using ConstructEngine.Gum;
using ConstructEngine.Util;
using MonoGameGum;
using Microsoft.Xna.Framework.Input;

namespace Slumber
{
    public class Main : Core
    {

        bool bloomEnabled = true;

        private GumService GumUI;

        public Main() : base("Platformer", 640, 360, false, "Assets/Fonts/Font")
        {
            Window.AllowUserResizing = true;
            Window.IsBorderless = true;

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
