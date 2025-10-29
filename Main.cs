using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine;
using ConstructEngine.Gum;
using ConstructEngine.Util;
using MonoGameGum;
using ConstructEngine.Physics;
using ConstructEngine.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slumber
{  
    public class Main : Core
    {   
        private GumService GumUI;

        public Main() : base("Platformer", 640, 360, false)
        {
            Window.AllowUserResizing = true;

            Window.IsBorderless = true;

            //IsMouseVisible = false;

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

            //ToggleFullscreen();

        }

        protected override void LoadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            SceneManager.UpdateCurrentScene(gameTime);

            GumUI.Update(this, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SetRenderTarget();


            if (!SceneManager.IsStackEmpty())
            {
                GraphicsDevice.Clear(Color.DarkSlateGray);
                SceneManager.GetCurrentScene().Draw(SpriteBatch);
            }



            base.Draw(gameTime);


            GumUI.Draw();

        }
    }
}
