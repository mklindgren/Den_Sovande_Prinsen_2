using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepBar : MonoBehaviour
{
    public Slider slider;

    private float sleepPoints; // = Player.sleep;

    private void Start()
    {
        sleepPoints = Player.sleep;
    }

    private void Update()
    {
        sleepPoints = Player.sleep;
        SetMaxSleep(sleepPoints);
        SetSleep(sleepPoints);
        //Debug.Log(sleepPoints);
    }

    public void SetMaxSleep(float sleepy)
    {
        slider.maxValue = sleepy;
        slider.value = sleepy;
    }
    public void SetSleep(float sleepy)
    {
        slider.value = sleepy; 
    }
}
