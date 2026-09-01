using System.Linq;

namespace Slumber;

public class SceneChange : Area2D
{
  public string TargetSceneName { get; set; }
  public string TargetGateID { get; set; }

  public Sprite2D EnterText { get; set; }

  public bool Trigger;

  public override void EnterTree()
  {
    EnterText = new Sprite2D().Set(n =>
    {
      n.Texture = new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Interact"), new Rectangle(0, 0, 48, 16));
      n.Visible = false;
      n.SetParent(this);
    });
  }

  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {
      if (Trigger)
        EnterText.Visible = true;
      if (Trigger && !Core.Input.IsActionJustPressed("Interact"))
        return;

      p.QueueFree();
      Main.GameManager.Change(TargetSceneName, TargetGateID);
    }

    EnterText.Visible = false;

  }
}

