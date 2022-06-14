using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;
using VOrb;

public class GA_initializer : Singlton<GA_initializer>
{
    
    void Start()
    {
        GameAnalytics.Initialize();
    }

    public void Buisness()
    {
 
    }

}
