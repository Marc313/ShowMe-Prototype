using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPlayerTrigger : PlayerTrigger
{
    protected override void OnPlayerTriggerEnter(Player player)
    {
        Debug.Log("ENTER");
        Boat boat = GetComponentInParent<Boat>();
        boat.Enter(player);
    }

    protected override void OnPlayerTriggerExit(Player player)
    {
        Debug.Log("EXIT");
        Boat boat = GetComponentInParent<Boat>();
        boat.Exit(player);
    }
}
