namespace Slumber;

public class Scene1 : Scene, IScene
{
    public FollowCamera Camera { get; set; }

    Area Area1;
    Area Area2;

    


    public Scene1 ():  base(new SceneConfig
    {
        DataPath = "Data/Scene1.json",
        TilemapTexturePath = "Assets/Tileset/SlumberTilesetAtlas",
        TilemapRegion = "0 0 512 512"
    }) {  }

    public override void Initialize()
    {
        base.Initialize();

        IRegionShape2D shape = new RectangleShape2D(10, 10, 10, 10);
    }
    public override void Load()
    {
        base.Load();
        GumHelper.Wipe();
        Camera = new FollowCamera(1f);
        Camera.LerpFactor = 1f;

        Area1 = new(new RectangleShape2D(10, 10, 10, 10));
        Area2 = new(new RectangleShape2D(10, 10, 10, 10));

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

        Area1.RegionShape.X += 10;

        Area1.AreaEntered(out Area other);
        Console.WriteLine($"Area1 Position: {Area1.RegionShape.Location}");
        Console.WriteLine($"Area1 Intersector: {other}");
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

}
