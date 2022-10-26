using System;
using UnityEngine;

public class RowingState : MovingState
{
    public Boat.BoatDirection boatSide;
    public float rowingCooldown = 0.5f;
    private float timer = 0;

    public RowingState(float _rowingCooldown)
    {
        rowingCooldown = _rowingCooldown;
    }

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

    public override void OnUpdate(IStateMachineOwner _owner)
    {
        base.OnUpdate(_owner);

        if (timer > 0) timer -= Time.deltaTime;
    }

    public override void HandleRowingInput(IStateMachineOwner _owner)
    {
        if (timer > 0) return;
        Player player = _owner as Player;

        player.boat.AddRowForce(player);
        ApplyCooldown();
    }

    public override void HandleMovement(IStateMachineOwner _owner, float _currentSpeed)
    {
        Player player = _owner as Player;
        int horizontal = player.controls.GetHorizontalPressed();
        if (horizontal != 0/* && horizontal != (int)boatSide*/)
        {
            player.boat.SwitchSide(player, horizontal);
        }
    }

    public override void HandleInteractInput(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, 5f);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                player.boat.OnInteract(player);
                player.moveMachine.SetState(new DefaultMoveState());
                Vector3 distance = collider.transform.position - player.transform.position;
                distance.y = player.transform.position.y;
                Vector3 landDir = distance.normalized;
                Vector3 playerLandingPos = landDir * 5f;
                player.transform.position += playerLandingPos;

                return;
            }
        }

        Debug.Log("No Ground Found!");
    }

    private void ApplyCooldown()
    {
        timer = rowingCooldown;
    }
}
