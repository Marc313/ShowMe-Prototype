using UnityEngine;

public abstract class MovingState : State
{
    public virtual void HandleMovement(IStateMachineOwner _owner, float _currentSpeed)
    {
        Player player = _owner as Player;
        if (player == null || !player.walkingEnabled) return;

        float vertical = player.controls.Vertical;
        float horizontal = player.controls.Horizontal;

        Vector3 moveDirection = (vertical * Vector3.forward + horizontal * Vector3.right).normalized;
        player.transform.forward = moveDirection;
        Vector3 movement = moveDirection * _currentSpeed * Time.fixedDeltaTime;
        movement.y = player.rigidBody.velocity.y;
        player.rigidBody.velocity = movement;   // Setting velocity had the best collision results
    }

    public virtual void HandleInteractInput(IStateMachineOwner _owner) { }

    public virtual void HandleRowingInput(IStateMachineOwner _owner) { }
}
