namespace Slumber;

public class Scene1 : Scene, IScene
{
    public FollowCamera Camera { get; set; }

    Area Area1;
    RegionShape2D Rect1;
    Area Area2;
    RegionShape2D Rect2;

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
        Camera = new FollowCamera(1f);
        Camera.LerpFactor = 1f;
    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        Camera.Follow(KinematicEntity.EntityList.OfType<Player>().FirstOrDefault().KinematicBase.Collider.Rect);

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Engine.SceneManager.ReloadCurrentScene();
        }

        Console.WriteLine($"Area Intersecting: {Area1.AreaEntered()}");
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

}
