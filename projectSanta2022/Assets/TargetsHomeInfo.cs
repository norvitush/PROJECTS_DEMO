using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetsHomeInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _numbers;
    public void Cleare()
    {
        foreach (var numText in _numbers)
        {
            numText.text = "";
        }
    }
    public void SetText(int num, string text)
    {
        if (num<=_numbers.Length)
        {
            _numbers[num-1].text = text;
            _numbers[num - 1].color = Color.white;
        }
    }
}
