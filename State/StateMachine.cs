using System.Collections.Generic;

namespace Slumber;

public class StateMachine : Node
{
  public State Initial { get; set; }

  public State Current { get; private set; }

  public Dictionary<string, State> States { get; } = new();

  public override void _EnterTree()
  {
    base._EnterTree();

    var childStates = GetAll<State>();
    for (int i = 0; i < childStates.Count; i++)
    {
      var state = childStates[i];
      States[state.GetType().Name.ToLower()] = state; 

      state.Transition = ChangeState;
    }
    if (Initial != null)
    {
      Initial.OnEnter();
      Current = Initial;
    }
  }

  public override void _Process(float delta)
  {
    base._Process(delta);

    Current?.Update(delta);
  }

  public override void _PhysicsUpdate(float delta)
  {
    base._Process(delta);

    Current?.PhysicsUpdate(delta);
  }

  public void ChangeState(string newStateName)
  {
    var newState = States[newStateName.ToLower()];

    if (newState == null)
      return;

    Current?.OnExit();

    newState.OnEnter();
    Current = newState;
  }
}
