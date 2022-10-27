using UnityEngine;

public class PickedupState : MovingState
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

    public override void HandleMovement(IStateMachineOwner _owner, float _currentSpeed)
    {
    }

    public override void HandleInteractInput(IStateMachineOwner _owner)
    {
        Player player = (Player) _owner;
        player.FreeSelf(player.carryingPlayer);
    }
}
