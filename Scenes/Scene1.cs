
using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ConstructEngine.Graphics;
using ConstructEngine.Util;
using System.Linq;
using Slumber.Entities;
using ConstructEngine.Components;
using ConstructEngine.UI;
using Slumber.Screens;
using ConstructEngine.Directory;
using ConstructEngine.Area;
using ConstructEngine.Helpers;
using ConstructEngine.Objects;

namespace Slumber;

public class Scene1 : Scene, Scene.IScene
{
    public RoomCamera _camera { get; set; }
    private ContentManager contentManager;

    public Scene1()
    {
        this.contentManager = Engine.Content;
        
    }

    public void Initialize()
    {
        GumHelper.RemoveScreenOfType<TitleScreen>();
    }
    public void Load()
    {
        OgmoParser.FromFile("Data/Scene1.json", "Assets/Tileset/SlumberTilesetAtlas", "0 0 512 512");

        _camera = new RoomCamera(1f); 
        
        ParallaxBackground.AddBackground(new("Assets/Backgrounds/Main", 0.1f,  ParallaxBackground.RepeatYX, _camera));

        ParallaxBackground.AddBackgrounds([
            "Assets/Backgrounds/Clouds5",
            "Assets/Backgrounds/Clouds4",
            "Assets/Backgrounds/Clouds3",
            "Assets/Backgrounds/Clouds2",
            "Assets/Backgrounds/Clouds1"
        ], 0.2f, _camera);



    }

    public void Unload() {  }

    public void Update(GameTime gameTime)
    {
  
        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.R))
        {
            Engine.SceneManager.ReloadCurrentScene();
        }

        ConstructObject.UpdateObjects(gameTime);

        _camera.Follow(KinematicEntity.EntityList.OfType<Player>().FirstOrDefault());

    }



    public void Draw(SpriteBatch spriteBatch)
    {
        ParallaxBackground.DrawParallaxBackgrounds(spriteBatch, Engine.GraphicsDevice, SamplerState.LinearWrap);

        spriteBatch.Begin(
            SpriteSortMode.BackToFront,
            samplerState: SamplerState.PointClamp,
            transformMatrix: _camera.Transform
        );

        ConstructObject.DrawObjects(spriteBatch);
    
        Tilemap.DrawTilemaps(spriteBatch);
                
        spriteBatch.End();
    }

}
