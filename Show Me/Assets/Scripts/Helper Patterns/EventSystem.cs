using System;
using System.Collections.Generic;

public enum EventName
{
    PLAYER_KILLED,
    BOAT_READY,
    BOAT_EXIT,
    LEVEL_END
}

public delegate void EventCallback(EventName _event, object _value);

public static class EventSystem
{
    private static Dictionary<EventName, List<EventCallback>> eventRegister = new Dictionary<EventName, List<EventCallback>>();

    public static void Subscribe(EventName _evt, EventCallback _func)
    {
        if (!eventRegister.ContainsKey(_evt))
        {
            eventRegister[_evt] = new List<EventCallback>();
        }

        eventRegister[_evt].Add(_func);
    }

    public static void Unsubscribe(EventName _evt, EventCallback _func)
    {
        if (eventRegister.ContainsKey(_evt))
        {
            eventRegister[_evt].Remove(_func);
        }
    }

    public static void RaiseEvent(EventName _evt, object _value = null)
    {
        if (eventRegister.ContainsKey(_evt))
        {
            foreach (EventCallback e in eventRegister[_evt])
            {
                e.Invoke(_evt, _value);
            }
        }
    }
}