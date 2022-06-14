using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb.CubesWar;
using TMPro;

public class StageView : MonoBehaviour
{
    public int StageNumber;
    private int _levelStars;
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private GameObject _lock;
    private bool _isLocked = true;
    public bool IsLocked =>_isLocked;
    public int Stars => _levelStars;
    public void OnStartStage()
    {
        #if UNITY_EDITOR
                 UIWindowsManager.GetWindow<StartWindow>().BtnStartClick(StageNumber);
        #else
                if (!IsLocked)
                {
                    UIWindowsManager.GetWindow<StartWindow>().BtnStartClick(StageNumber);
                }
        #endif


    }
    public void DropStars() => _levelStars = 0;
    public void SetLevelStars(int value, bool isLocked=false)
    {
        _isLocked = isLocked;
        _numberText.text = StageNumber.ToString();
        GameObject starsGroup = GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject;
        starsGroup.gameObject.SetActive(!isLocked);
        _numberText.gameObject.SetActive(!isLocked);
        _lock.SetActive(isLocked);
        if (!isLocked)
        {
            _levelStars = Mathf.Max(_levelStars, value);

            int turnOn = 0;
            for (int i = 0; i < starsGroup.transform.childCount; i++)
            {
                starsGroup.transform.GetChild(i).GetChild(0).gameObject.SetActive(turnOn++ < _levelStars);
            }
        }
 
        
    }
}
