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
        
    }
    public override void Load()
    {
        GumHelper.Wipe();

        Camera = new RoomCamera(1f); 
    }

    public override void Unload() {  }

    public override void Update(GameTime gameTime)
    {
        Camera.Follow(KinematicEntity.EntityList.OfType<Player>().FirstOrDefault());
    }
    public override void Draw(SpriteBatch spriteBatch) {  }

}
