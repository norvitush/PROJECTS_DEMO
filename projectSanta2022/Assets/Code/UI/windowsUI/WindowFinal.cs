using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VOrb.CubesWar;
using TMPro;
using UnityEngine.Events;
using System;
using System.Linq;

public class WindowFinal : Window
{
    [SerializeField] GameObject _camPoint;
    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void SelfOpen()      //эффекты открытия и другая красота - здесь
    {                                       //opening effects and other beauty-here       
        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() => {});
        SvipeEvent.AddListener(() => { });

        gameObject.SetActive(true);
        GameService.ActiveNow = this;
    }
   
    public void SetCamToPoint()
    {
        var mainCam = Camera.main.gameObject;
        mainCam.transform.SetParent(_camPoint.transform);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.localRotation = Quaternion.Euler(0,0,0);
        RenderSettings.fog = false;
    }


}