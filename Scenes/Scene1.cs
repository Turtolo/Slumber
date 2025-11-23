using System.Collections.Generic;

namespace Slumber;

public class Scene1 : Scene, IScene
{
    public RoomCamera Camera { get; set; }
    public Area2D Area1;

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

        Area1 = new(new NodeConfig
        {
            Shape = new RectangleShape2D(400, 160, 100, 20),
            Name = "Scene1TestArea",
            Root = this
        });

    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        Camera.Follow(Node.AllInstances.OfType<Player>().FirstOrDefault());

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.R))
            Engine.SceneManager.ReloadCurrentScene();

        
        if (Area1.BodyEntered(out KinematicBody2D body))
        {
            body.QueeFree();
        }

    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);        
    }
}
