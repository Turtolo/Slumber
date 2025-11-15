using ConstructEngine;
using Microsoft.Xna.Framework;

namespace Slumber;

public class Main : Engine
{
    public Main() : base(new EngineConfig
    {
        Title = "Platformer",
        VirtualWidth = 640,
        VirtualHeight = 360,
        Fullscreen = false,
        FontPath = "Assets/Fonts/Font",
        GumProject = "GumProject/GumProject.gumx",
        IntegerScaling = true
    }) {}

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
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SceneManager.DrawCurrentScene(SpriteBatch);
        
        base.Draw(gameTime);
    }
}
