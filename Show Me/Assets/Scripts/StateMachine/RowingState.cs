using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowingState : MovingState
{
    public override void OnEnter(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;
        player.DisableMovement();
    }

    public override void OnExit(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;
        player.EnableMovement();
    }

    public override void HandleRowingInput(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;

        player.boat.AddForce(player);
    }
}
