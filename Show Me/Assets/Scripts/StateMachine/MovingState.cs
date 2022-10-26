using UnityEngine;

public abstract class MovingState : State
{
    public virtual void HandleMovement(IStateMachineOwner _owner, float _currentSpeed)
    {
        Player player = _owner as Player;
        if (player == null || !player.walkingEnabled) return;

        float vertical = player.controls.GetVertical();
        float horizontal = player.controls.GetHorizontal();

        Vector3 moveDirection = (vertical * Vector3.forward + horizontal * Vector3.right).normalized;
        Vector3 movement = moveDirection * _currentSpeed * Time.deltaTime;
        player.rigidBody.position += movement;
    }

    public virtual void HandlePickupInput(IStateMachineOwner _owner) { }

    public virtual void HandleRowingInput(IStateMachineOwner _owner) { }
}
