namespace Slumber;

public class Checkpoint : Node2D
{
  public AnimatedSprite2D Sprite { get; set; }

  public Area2D Area { get; set; }

  //public string Id { get; set; } 

  public bool Lit { get; set; }

  public override void EnterTree()
  {
    var atlas = AsepriteLoader.LoadAnimations(
        new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/BONFIRE"), new Rectangle(0, 0, 864, 48)),
        PathTools.Combine("Raw/Raw/BONFIRE.json")
    );

    Sprite = new AnimatedSprite2D().Set(n =>
    {
      n.Atlas = atlas;
      n.SetParent(this);
    });

    Area = new Area2D().Set(n =>
    {
      n.AddChild(new CollisionShape2D().Set(c =>
      {
        c.Position = new Vector2(-20, -15);
        c.Shape = new RectangleShape2D(40, 38);
      }));
      n.SetParent(this);
    });
  }

  public override void PhysicsUpdate(float delta)
  {

  }

  public override void Process(float delta)
  {
    if (Lit)
      Sprite.PlayAnimation("Lit");
    else
      Sprite.PlayAnimation("UnLit");

    if (Area.GetAnyBody() is Player p)
    {
      Light();
    }
  }
  
  public void Light()
  {
    Lit = true;
    Main.GameManager.Save(this);
  }

  public override void ExitTree()
  {

  }
}
