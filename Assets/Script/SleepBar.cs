using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxSleep(int sleep)
    {
        slider.maxValue = sleep;
        slider.value = sleep;
    }
    public void SetSleep(int sleep)
    {
        slider.value = sleep; 
    }
}
