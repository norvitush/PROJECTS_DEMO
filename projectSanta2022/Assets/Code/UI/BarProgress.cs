using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarProgress : MonoBehaviour
{
    public Image selfSprite;
    public TextMeshProUGUI procentText;
    public float bound0 = 0f;
    public float bound1 = 1f;
    private float value =1f;

    public void SetBounds(float min, float max)
    {
        bound0 = min;
        bound1 = max;
    }

    public void SetProgress(float procent)
    {
        if (procentText!=null)
        {
            procentText.text = (int)procent + "%";
        }
        
        value = Mathf.Lerp( bound0, bound1, procent / 100);
        selfSprite.fillAmount = value;        
    }
}
