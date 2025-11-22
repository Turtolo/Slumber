namespace Slumber;

public class Scene1 : Scene, IScene
{
    public RoomCamera Camera { get; set; }

    public Scene1 ():  base(new SceneConfig
    {
        DataPath = "Data/Scene1.json",
        TilemapTexturePath = "Assets/Tileset/SlumberTilesetAtlas",
        TilemapRegion = "0 0 512 512"
    }) {  }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Load()
    {
        base.Load();
        
        GumHelper.Wipe();
        Camera = new RoomCamera(1f);
        Camera.LerpFactor = 1f;

    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        Camera.Follow(KinematicEntity.EntityList.OfType<Player>().FirstOrDefault());

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.R))
            Engine.SceneManager.ReloadCurrentScene();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

}
