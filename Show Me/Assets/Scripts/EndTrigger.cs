using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : PlayerTrigger
{
    protected override void OnPlayerTriggerEnter(Player player)
    {
        EventSystem.RaiseEvent(EventName.LEVEL_END);
    }
}
