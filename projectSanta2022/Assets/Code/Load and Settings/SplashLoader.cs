using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SplashLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _procentText;
    [SerializeField] private Image _progressBar;
    [SerializeField] private GameObject _activateNext;

    void Start()
    {
        _procentText.text = "0%";
        _progressBar.fillAmount = 0;
        StartCoroutine(Loadding(1.5f));
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
            _procentText.text = string.Format("{0}%",Mathf.Clamp(Mathf.RoundToInt(proc),0,100));
            _progressBar.fillAmount += TicVal/100;
        }
        _activateNext?.SetActive(true);
    }

}
