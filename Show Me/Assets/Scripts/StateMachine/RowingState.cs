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
        int horizontal = player.controls.HorizontalRaw;
        if (horizontal != 0 && horizontal != (int)boatSide)
        {
            player.boat.SwitchSide(player, horizontal);
        }
    }

    public override void HandleInteractInput(IStateMachineOwner _owner)
    {
        Player player = _owner as Player;

        if (player.boat == null) return;

        Collider[] colliders = Physics.OverlapSphere(player.boat.transform.position, 5f);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Vector3 playerLandingPos = CalculateLandingPos(player, collider);
                player.transform.position = playerLandingPos;

                player.boat.OnInteract(player);
                player.moveMachine.SetState(new DefaultMoveState());

                return;
            }
        }

        Debug.Log("No Ground Found!");
    }

    private Vector3 CalculateLandingPos(Player _player, Collider _collider)
    {
        Vector3 distance = _collider.transform.position - _player.boat.transform.position;
        distance.y = 0;
        Vector3 landDir = distance.normalized;
        Vector3 playerMovement = landDir * 5f;
        Vector3 playerPos = _player.boat.transform.position + playerMovement;
        playerPos.y = _collider.transform.position.y + _player.halfModelHeight;

        return playerPos;
    }

    private void ApplyCooldown()
    {
        timer = rowingCooldown;
    }
}
