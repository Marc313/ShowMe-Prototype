using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFSM : MonoBehaviour
{
    protected State currentState;
    protected IStateMachineOwner owner;

    public AbstractFSM(IStateMachineOwner _owner)
    {
        owner = _owner;
    }

    public void SetState(State _newState)
    {
        currentState?.OnExit(owner);
        currentState = _newState;
        currentState?.OnEnter(owner);
    }

    private void Update() => currentState?.OnUpdate(owner);
    private void FixedUpdate() => currentState?.OnFixedUpdate(owner);
}

public abstract class State
{
    public virtual void OnEnter(IStateMachineOwner _owner) { }
    public virtual void OnUpdate(IStateMachineOwner _owner) { }
    public virtual void OnFixedUpdate(IStateMachineOwner _owner) { }
    public virtual void OnExit(IStateMachineOwner _owner) { }
}