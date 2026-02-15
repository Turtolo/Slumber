using System.IO;
using System.Security.Cryptography;
using RenderingLibrary.Graphics;

namespace Slumber;

public class Scene1 : Stage
{
    public Scene1 () {}

    public Song bgMusic;
    public SoundEffect bgSFX;

    Random random;

    public override void OnEnter()
    {
        base.OnEnter();

        random = new Random();

        var camera = new RoomCamera(new RoomCameraConfig
        {
            TargetNode = Engine.Node.GetFirstNodeByT<Player>()
        });

        camera.LocalPosition = new Vector2(Engine.Node.GetFirstNodeByT<Player>().GlobalPosition.X, Engine.Node.GetFirstNodeByT<Player>().GlobalPosition.Y - 60);

        var layer1 = new ParallaxLayer(new ParallaxLayerConfig
        {
            Texture = new MTexture("Assets/Backgrounds/HeightsBGNoMain"),
            LoopAxis = LoopAxis.X,
            LocalPosition = new Vector2(0, 250),
            Depth = -2
        });

        var layer2 = new ParallaxLayer(new ParallaxLayerConfig
        {
            Texture = new MTexture("Assets/Backgrounds/HeightsBG"),
            MotionScale = Vector2.Zero,
            LoopAxis = LoopAxis.X,
            LocalPosition = new Vector2(0, 300),
            Depth = -1
        });
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void PhysicsUpdate(float deltaTime)
    {
        base.PhysicsUpdate(deltaTime);
    }

    public override void ProcessUpdate(float deltaTime)
    {
        base.ProcessUpdate(deltaTime);
    }

    public override void SubmitCall()
    {
        base.SubmitCall();
        //foreach (var c in Engine.Node.GetNodesByT<CollisionShape2D>()) 
            //c.Shape.Draw();        
    }
}
