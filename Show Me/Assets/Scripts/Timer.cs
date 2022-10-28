using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float currentTime;
    private bool isActive;

    public void Tick()
    {
        if (isActive)
        currentTime += Time.deltaTime;
    }

    public void Start()
    {
        isActive = true;
    }

    public void Reset()
    {
        currentTime = 0;
    }

    public string TimeToString()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        return time.ToString(@"mm\:ss");
    }

    public void FreezeTimer()
    {
        isActive = false;
    }
}
