using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarProgress : MonoBehaviour
{
    [SerializeField] private Image _selfSprite;
    [SerializeField] private TextMeshProUGUI _procentText;
    private float _boundMin = 0f;
    private float _boundMax = 1f;
    private float value =1f;

    public void SetBounds(float min, float max)
    {
        _boundMin = min;
        _boundMax = max;
    }

    public void SetProgress(float procent)
    {
        _procentText.text = (int)procent + "%";
        value = Mathf.Lerp( _boundMin, _boundMax, procent / 100);
        _selfSprite.fillAmount = value;        
    }
}
