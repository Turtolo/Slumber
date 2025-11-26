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
            FontPath = "Assets/Fonts/Font",
            GumProject = "GumProject/GumProject.gumx",
            MainCharacterType = typeof(Player)
        }) {  }

        protected override void Initialize()
        {
            base.Initialize();
            SceneManager.AddScene(new MainMenu());

            DebugOverlay.AddInfo("PlayerLocation", () =>
            {
                var p = NodeManager.GetNodeByName("Player") as Player;
                
                return p == null ? "Player: Null" : $"Player Location: {p.Location}";
            }, Color.Yellow);
           
            DebugOverlay.AddInfo("PlayerState", () =>
            {
                var p = NodeManager.GetNodeByName("Player") as Player;
                
                return p == null ? "Player: Null" : $"Player State: {p.StateController.CurrentState}";
            }, Color.Yellow);
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
