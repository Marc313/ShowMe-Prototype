using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStateMachine : AbstractFSM
{
    private void Awake()
    {
        owner = GetComponent<IStateMachineOwner>();
        currentState = new DefaultMoveState();
    }

    public MoveStateMachine(IStateMachineOwner _owner) : base(_owner)
    {
        currentState = new DefaultMoveState();
    }

    public MovingState GetState()
    {
        return (MovingState) currentState;
    }
}
