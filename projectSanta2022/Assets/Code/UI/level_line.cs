using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class level_line : MonoBehaviour
{
    [SerializeField] private int _level;
    [SerializeField] private int _value;
    [SerializeField] private TextMeshProUGUI _numberText;

    public int Value { get => _value;  }
    public int Level { get => _level;  }

    private void Start()
    {
        SetCollected(UIWindowsManager.GetWindow<MainWindow>().StarsPath.GetStarsCount(_level-1));
    }
    public void SetCollected(int collected)
    {
        collected = Mathf.Clamp(collected, 0, _value);
        _numberText.text = collected+"/"+_value.ToString();
    }
}
