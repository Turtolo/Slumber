
namespace Slumber;

public class MainMenu : Scene, IScene
{
    TitleScreen titleScreen;
    MTexture texture;
    
    public MainMenu() : base(new SceneConfig {})
    {
        
    }

    public override void Initialize()
    {
        base.Initialize();

        GumHelper.Wipe();

        titleScreen = new TitleScreen();

        GumHelper.AddScreenToRoot(titleScreen);

        
        texture = new("Assets/Backgrounds/streetsbg");
    
    }
    public override void Load()
    {
        base.Load();
        
        
    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Engine.Input.IsActionJustPressed("Back"))
        {
            if (titleScreen.Settings.Controls.IsVisible)
            {
                titleScreen.Settings.Controls.IsVisible = false;
                titleScreen.Settings.Main.IsVisible = true;
                titleScreen.Settings.ControlButton.IsFocused = true;
                return;
            }

            if (titleScreen.Settings.Main.IsVisible)
            {
                titleScreen.Settings.IsVisible = false;
                titleScreen.Main.Visible = true;
                titleScreen.StartButton.IsFocused = true;
                return;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Engine.DrawManager.DrawLooping(
            texture,
            Vector2.Zero,
            Vector2.Zero,
            DrawLayer.Background
        );
    }


}
