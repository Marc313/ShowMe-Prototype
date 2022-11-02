using System;
using UnityEngine;

public abstract class PlayerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            OnPlayerTriggerEnter(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            OnPlayerTriggerExit(player);
        }
    }

    protected virtual void OnPlayerTriggerEnter(Player player) { }
    protected virtual void OnPlayerTriggerExit(Player player) { }
}
