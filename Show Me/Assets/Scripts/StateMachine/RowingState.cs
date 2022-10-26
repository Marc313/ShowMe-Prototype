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

        player.boat.AddForce(player);
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

    private void ApplyCooldown()
    {
        timer = rowingCooldown;
    }
}
