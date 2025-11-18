
namespace Slumber;


public class MainMenu : Scene, IScene
{
    TitleScreen titleScreen;

    ParallaxBackground Background;
    
    public MainMenu() : base(new SceneConfig {})
    {
        
    }

    public override void Initialize()
    {
        GumHelper.Wipe();

        titleScreen = new TitleScreen();

        GumHelper.AddScreenToRoot(titleScreen);

        Background = new ParallaxBackground(
            texture: Engine.Content.Load<Texture2D>("Assets/Backgrounds/streetsbg"),
            parallaxFactor: 0.1f,
            samplerState: ParallaxSamplers.RepeatX,
            position: new Vector2(0,0)
        );
    }   

    public override void Load() { }
    public override void Unload() { }

    public override void Update(GameTime gameTime)
    {
        if (Engine.Input.IsActionJustPressed("Back"))
        {
            if (titleScreen.Settings.Main.IsVisible)
            {
                titleScreen.Settings.IsVisible = false;
                titleScreen.Main.Visible = true;
                titleScreen.StartButton.IsFocused = true;
            }
            if (titleScreen.Settings.Controls.IsVisible)
            {
                titleScreen.Settings.Controls.IsVisible = false;
                titleScreen.Settings.Main.IsVisible = true;
                titleScreen.Settings.ControlButton.IsFocused = true;
            }
        }

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
       Background.Draw(spriteBatch, Engine.GraphicsDevice);
    }

}
