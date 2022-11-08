using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : PlayerTrigger
{
    public List<Player> players = new List<Player>();

    protected override void OnPlayerTriggerEnter(Player player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            player.gameObject.SetActive(false);
        }

        if (players.Count == 2)
        {
            EventSystem.RaiseEvent(EventName.LEVEL_END);
        }
    }
}
