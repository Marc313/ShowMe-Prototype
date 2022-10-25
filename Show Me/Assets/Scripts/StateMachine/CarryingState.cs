using UnityEngine;

public class CarryingState : MovingState
{
    public override void OnEnter(IStateMachineOwner _owner)
    {
        Player player = (Player)_owner;
        IPickupable target = (IPickupable) _owner.sharedData.Get("pickedUp");
        float carryingObjectSpeed = (float) _owner.sharedData.Get("carryingSpeed");
        float carryingPlayerSpeed = (float) _owner.sharedData.Get("carryingPlayerSpeed");

        if (target == null) return;
        else if (target is Player)
        {
            player.SetSpeed(carryingPlayerSpeed);
        }
        else
        {
            player.SetSpeed(carryingObjectSpeed);
        }
    }

    public override void HandleMovement(IStateMachineOwner _owner, float _currentSpeed)
    {
        base.HandleMovement(_owner, _currentSpeed);

        IPickupable pickupable = (IPickupable)_owner.sharedData.Get("pickedUp");
        Vector3 yOffset = Vector3.up * (pickupable.modelHeight + ((IPickupable)_owner).modelHeight);
        ((MonoBehaviour) pickupable).transform.position = ((MonoBehaviour) _owner).transform.position + yOffset;
    }

    public override void HandlePickupInput(IStateMachineOwner _owner)
    {
        Player player = (Player)_owner;
        player.FreeTarget();
    }
}
