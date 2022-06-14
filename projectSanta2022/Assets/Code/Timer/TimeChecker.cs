using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

public class TimeChecker : MonoBehaviour
{      
    private Color curColor;

    private void Start()
    {        
        curColor = new Color(r: 0, g: 0, b: 0, a: 0.7f);
    }

    public void CountTick()
    {    
        //GameService.Instance.TimeCount++;
        //int count = GameService.Instance.TimeCount;        
        //GameService.Instance.OnTimerTick(count);        
    }    
}
