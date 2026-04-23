namespace Slumber
{
    public class Main : Engine
    {
        public Main() {}

        protected override void Initialize()
        {
            base.Initialize();

            ClassDB.Initialize(typeof(Main).Assembly);

            Tree.SetScene(Engine.Table.Create<DevStage>());    
            
            Input.AddBind("MoveLeft", new InputAction(Keys.A), new InputAction(Buttons.LeftThumbstickLeft), new InputAction(Buttons.DPadLeft));
            Input.AddBind("MoveRight", new InputAction(Keys.D), new InputAction(Buttons.LeftThumbstickRight), new InputAction(Buttons.DPadRight));
            Input.AddBind("MoveDown", new InputAction(Keys.S), new InputAction(Buttons.LeftThumbstickDown), new InputAction(Buttons.DPadDown));
            Input.AddBind("MoveUp",new InputAction(Keys.W), new InputAction(Buttons.LeftThumbstickUp), new InputAction(Buttons.DPadUp));

            Input.AddBind("Jump", new InputAction(Keys.Space), new InputAction(Buttons.A));

            Input.AddBind("Attack", new InputAction(Keys.K), new InputAction(Buttons.Y));

            Input.AddBind("Pause", new InputAction(Keys.Escape), new InputAction(Buttons.Start));
            Input.AddBind("Back", new InputAction(Keys.X), new InputAction(Buttons.B));

            Prefs.Graphics.Fullscreen = false;
 
            Prefs.Apply();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.CurrentGamePad.WasButtonJustPressed(Buttons.Back)) 
                Tree.ReloadCurrentScene();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        
        }
    }
}

