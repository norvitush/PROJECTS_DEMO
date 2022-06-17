using UnityEngine;
using VOrb.SantaJam;
using TMPro;
using System;
using VOrb.SantaJam.Levels;

public class StartPopupWindow : Window
{
    public Action afterInit;
    public Action afterClose;
    [SerializeField] private GameObject _iconGift;
    [SerializeField] private GameObject _iconSmoked;
    [SerializeField] private GameObject _iconLighted;
    [SerializeField] private GameObject _iconNumered;
    [SerializeField] private GameObject _iconSpeed;
    [SerializeField] private GameObject _iconBrick;
    [SerializeField] private TextMeshProUGUI _numberText;

    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            afterClose?.Invoke();
        }
    }

    protected override void SelfOpen()      //эффекты открытия и другая красота - здесь
    {                                       //opening effects and other beauty-here       
        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() => {});
        SvipeEvent.AddListener(() => {});
        gameObject.SetActive(true);
        GameService.Instance.ActiveNow = this;
        afterInit?.Invoke();
       
        SetIcons();
    }
   
    public void SetIcons()
    {
        DeactivateIcons();
        RectTransform popup = transform.Find("Popup") as RectTransform;
        
        
        var level = GameService.Instance.CurrentLevel;
        var giftText = _iconGift.transform.Find("count").GetComponent<TextMeshProUGUI>();
        
        giftText.text = "x "+ level.Giftscount +" gifts";

        var targetsInfo = UIWindowsManager.GetWindow<MainWindow>().UIPanel.GetComponentInChildren<TargetsHomeInfo>(true);
        targetsInfo.gameObject.SetActive(false);
        targetsInfo.Cleare();
        foreach (var fich in level.GetFiches())
        {
            if (fich is SmokedChimney)
            {
                _iconSmoked.SetActive(true);
            }
            if (fich is LightHouseChimney)
            {
                _iconLighted.SetActive(true);
            }
            
            if (fich is NumberedHouseChimney)
            {
                string output = "";
                var nums = level.GetHousesNumber();
                for (int i = 0; i < nums.Count - 1; i++)
                {
                    output += nums[i] + ", ";
                    targetsInfo.SetText(i + 1, nums[i].ToString());
                }
                output += nums[nums.Count - 1].ToString();
                targetsInfo.SetText(nums.Count, nums[nums.Count - 1].ToString());
                _numberText.text = output;
                targetsInfo.gameObject.SetActive(true);
                _iconNumered.SetActive(true);
            }
            
        }

        int generatedFiches = 0;
        if (level.Speed>2f)
        {
            _iconSpeed.SetActive(true);
            generatedFiches++;
        }
        if (level.LevelNumber > SceneLoader.SceneSettings.BrickShowLevel)
        {
            _iconBrick.SetActive(true);
            generatedFiches++;
        }

        popup.offsetMin = new Vector2(popup.offsetMin.x, Mathf.Clamp(popup.offsetMin.y - 150*Mathf.Clamp(level.GetFiches().Count+ generatedFiches- 3, 0,2)  , 150,475));

    }
    private void DeactivateIcons()
    {
        //_iconGift.SetActive(false);
        _iconSmoked.SetActive(false); 
        _iconLighted.SetActive(false); 
        _iconNumered.SetActive(false);
        _iconBrick.SetActive(false);
        _iconSpeed.SetActive(false);

    }
    public void OnOkey()
    {
        Close();  
    }



}