﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VOrb
{
public class SplashLoader : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI procentText;
    [SerializeField] public Image progressBar;
    [SerializeField]  public GameObject ActivateNext;

    void Start()
    {
        procentText.text = "0%";
        IEnumerator loader = Loadding(1.5f);
        progressBar.fillAmount = 0;
        StartCoroutine(loader);
    }

    IEnumerator Loadding(float tm)
    {
        float proc = 0;
        float cnt = Mathf.Clamp(tm, 0.1f, 4);
        float TicVal = 100 / Mathf.Clamp((cnt/Time.fixedDeltaTime),0.1f,200f);
        while (cnt>0)
        {
            yield return new WaitForFixedUpdate();
            cnt -= Time.fixedDeltaTime;
            proc += TicVal; 
            procentText.text = string.Format("{0}%",Mathf.Clamp(Mathf.RoundToInt(proc),0,100));
            progressBar.fillAmount += TicVal/100;
        }
        ActivateNext.SetActive(true);
    }

}

}
