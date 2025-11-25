namespace Slumber
{
    public class Main : Engine 
    {
        public Main() : base(new EngineConfig
        {
            Title = "Slumber",
            RenderWidth = 640,
            RenderHeight = 360,
            Fullscreen = false,
            IntegerScaling = true,
            DebugMode = true,
            AllowUserResizing = true,
            Maximised = true,
            IsBorderless = true,
            IsFixedTimeStep = false,
            SynchronizeWithVerticalRetrace = true,
            FontPath = "Assets/Fonts/Font",
            GumProject = "GumProject/GumProject.gumx",
            MainCharacterType = typeof(Player)
        }) {  }

        protected override void Initialize()
        {
            base.Initialize();
            SceneManager.AddScene(new MainMenu());

            DebugOverlay.AddInfo("PlayerPosition", () => $"Player Pos: {2}");
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
