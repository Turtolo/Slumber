using System.Collections.Generic;

namespace Slumber;

public class StateMachine : Node
{
  public State Initial { get; set; }

  public State Current { get; private set; }

  public Dictionary<string, State> States { get; } = new();

  public override void EnterTree()
  {
    base.EnterTree();

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

  public override void Process(float delta)
  {
    base.Process(delta);

    Current?.Update(delta);
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    Current?.Physics(delta);
  }

public void ChangeState(string newStateName)
  {
    var newState = States[newStateName.ToLower()];

    if (newState == null)
      return;

    Current?.OnExit();

    Current = newState;
    newState.OnEnter();
  }
}
