using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStateMachine : AbstractFSM
{
    // Refactor for all entities
    public MoveStateMachine(IStateMachineOwner _owner) : base(_owner)
    {
        currentState = new DefaultMoveState();
    }

    public MovingState GetState()
    {
        return (MovingState) currentState;
    }
}
