using System.Collections.Generic;
using Opal.Managers;
using Opal.Runtime;

namespace Slumber;

public class Scene : Anchor,
  IReady,
  IEnterTree, 
  IPhysicsUpdate, 
  IProcess, 
  ICall, 
  IExitTree
{
  public Dictionary<string, Vector2> SpawnPoints { get; set; } = new();

  public string EntranceGateID { get; set; }

  public Vector2 SpawnPosition { get; set; }

  public Scene()
  {
    _Ready();
    Ready();
  }
  
  public virtual void _Ready() { }
  public virtual void Ready() { }

  public virtual void _EnterTree() { }
  public virtual void EnterTree() { }

  public virtual void _Process(float delta) { }
  public virtual void Process(float delta) { }

  public virtual void _PhysicsUpdate(float delta) { }
  public virtual void PhysicsUpdate(float delta) { }

  public virtual void _Submit(Canvas2D canvas) { }
  public virtual void Submit(Canvas2D canvas) { }

  public virtual void _ExitTree() { }
  public virtual void ExitTree() { }
}
