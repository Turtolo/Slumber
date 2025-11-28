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

        new Sprite2D(new NodeConfig
        {
            Shape = new RectangleShape2D(10, 10, 10, 10),
            Name = "Sprite2D",
            Parent = this,
            Values = new Sprite2D.ValuesData
            (
                Texture: new MTexture("Assets/Animations/Player/PlayerModel3Atlas"),
                Scale: Vector2.One,
                Rotation: 0f,
                Modulate: Color.White
            )
        });

    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        Camera.Follow(NodeManager.AllInstances.OfType<Player>().FirstOrDefault());
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);        
    }
}
