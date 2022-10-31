using UnityEngine;

// Default Move State. Player is able to move around and pick up objects.
public class DefaultMoveState : MovingState
{
    public override void OnEnter(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;
        float speed = (float) player.sharedData.Get("defaultSpeed");
        player.SetSpeed(speed);
    }

    public override void HandleInteractInput(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;
        player.OverlapInteractBoatPriority();
    }
}
