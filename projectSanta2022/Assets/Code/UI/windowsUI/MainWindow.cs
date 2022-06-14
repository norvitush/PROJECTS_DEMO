﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VOrb.CubesWar;
using TMPro;
using UnityEngine.Events;
using System;
using System.Linq;
using UnityEngine.UI;
using GameAnalyticsSDK;

public class MainWindow : Window
{
    [Header("Дочерние окна:")]
    [SerializeField] private StartWindow start_window;
    [SerializeField] private StartPopupWindow _startInfoWindow;
    [SerializeField] private LevelResultWindow _resultsWindow;

    [Header("Игровые контейнеры:")]
    [SerializeField] internal GameObject UIContainer;
    [SerializeField] internal GameObject VFXContainer;
    [SerializeField] internal GameObject FonContainer;
    [SerializeField] internal GameObject GameElements;
    [SerializeField] internal GameObject TutorialContainer;
    [SerializeField] internal GameObject GunContainer;

    
    [SerializeField] private GameObject _camPoint1;
    [SerializeField] private GameObject _camPoint2;
    [SerializeField] private TextMeshProUGUI _giftsInfoText;
    [SerializeField] private TextMeshProUGUI _smilesInfoText;
    [SerializeField] private TextMeshProUGUI _targetsInfoText;
    [SerializeField] private Slider _levelProgress;


    [Header("КОНТРОЛЛЕР ЗВЁЗДНОГО ПУТИ")]
    [SerializeField] private PathController _starsPath;
    public PathController StarsPath => _starsPath;

    private int _camPoint=1;


    [Header("События:")]
    public Action StartAfterLoad;


   // public delegate void whenReady();



    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);          
        }
    }

    protected override void SelfOpen()      //эффекты открытия и другая красота - здесь
    {                                       //opening effects and other beauty-here
        start_window = UIWindowsManager.GetWindow<StartWindow>();
        _startInfoWindow = UIWindowsManager.GetWindow<StartPopupWindow>();
        _resultsWindow = UIWindowsManager.GetWindow<LevelResultWindow>();
        _starsPath = GetComponent<PathController>();
        

        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() =>
        {
           
        });
        SvipeEvent.AddListener(() => { });

        gameObject.SetActive(true);
        GameService.ActiveNow = this;

        StartAfterLoad?.Invoke();


    }
    public void OpenSettingsWindow()
    {
        HideMainUI();
        //GameService.ActiveNow = settings_window;
        //settings_window?.Open(ChangeCurrentWindow);  //opening the settings window   
        
    }
    public void TryOpenLevelResultsPopup(Action invokeAfterClose)
    {
       
        int stars = GameService.Instance.CalculateStarsResult();

        if (stars > 0)
        {
            _starsPath.SetLevelStars(GameService.Instance.Current_level, stars);
            SceneLoader.Instance.FB_manager?.LevelEnded(GameService.Instance.currentLevel.LevelNumber);
            _resultsWindow.Init(invokeAfterClose, stars);
            _resultsWindow?.Open(ChangeCurrentWindow);
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Level " + GameService.Instance.currentLevel.LevelNumber.ToString(),
            GameService.Instance.ScoreGiftsCount);

            if (GameService.Instance.NoAds == 0)
            {
                #if UNITY_EDITOR
                                Debuger.AddToLayout("*IS_else: Interstitial Try SHOW!");

                #elif UNITY_ANDROID
                          Debuger.AddToLayout("*IS: Interstitial Try SHOW!");
                          if (IronSource.Agent.isInterstitialReady())
                           {
                              Debuger.AddToLayout("*IS: Interstitial Try SHOW!  -   ReadyForShow");

                             /// показываем межстраничную                    

                             IronSource.Agent.showInterstitial();
                          }
                #endif
            }
            invokeAfterClose?.Invoke();
        }
       
    }
    public void OpenLevelInfoPopup()
    {
        _startInfoWindow.afterInit = () => {
            if (GameService.Instance.GameStarted && Time.timeScale == 1)
            {
                GameService.Instance.Pause();
            }
        };
        _startInfoWindow.afterClose = () => {
            if (GameService.Instance.GameStarted && Time.timeScale == 0)
            {
                GameService.Instance.Pause();
                GameService.ActiveNow = this;
            }
        };
        //GameService.ActiveNow = _startInfoWindow;
        _startInfoWindow?.Open(ChangeCurrentWindow);  //opening the pause window  
    }
    public void OpenStartWindow()
    {
        HideMainUI();
        GameService.ActiveNow = start_window;
        start_window?.Open(ChangeCurrentWindow);  //opening the settings window  
        
    }
    public void OpenRestartWindow()
    {
        HideMainUI();
        //GameService.ActiveNow = restart_window;
        //restart_window?.Open(ChangeCurrentWindow);  //opening the settings window    

    }

    internal void SetProgressSlider(float value)
    {
        _levelProgress.value = value;
        if (value<0.2f)
        {
            _levelProgress.transform.Find("mark1").Find("active").gameObject.SetActive(false);
            _levelProgress.transform.Find("mark2").Find("active").gameObject.SetActive(false);
        }
        if (value>0.5f)
        {
            _levelProgress.transform.Find("mark1").Find("active").gameObject.SetActive(true);
        }
        if (value>0.98f)
        {
            _levelProgress.transform.Find("mark2").Find("active").gameObject.SetActive(true);
        }
    }

    internal void SetGiftsInfo(int giftscount)
    {
        _giftsInfoText.text = giftscount +  "/"+GameService.Instance.currentLevel.Giftscount;
    }
    internal void SetSmilesInfo(int count)
    {
        _smilesInfoText.text = Mathf.Clamp(count,0, 100000).ToString();
    }
    internal void SetTargetsInfo(int count)
    {
        _targetsInfoText.text = Mathf.Clamp(count, 0, 100000).ToString();
    }

    public void CloseChildren()
    {
        CurrentWindow.Close();
    }


    public void Pause()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        GameService.Instance.Pause();
        UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow();
    }


    public override void InvokeTouchEvent(TouchEvent updateType, Swipe data)
    {
        if (TutorialContainer.activeInHierarchy)
        {
            TutorialContainer.SetActive(false);
            GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Tutorial, 1);
        }

        GameService.Instance.GameJoystik.InvokeTouchEvent(updateType, data);
    }

    public void SetSettingsByDefaults()
    {
        SetCamToPoint();
        ShowMainUI();
    }
    private void ShowMainUI()
    {
        UIContainer.SetActive(true);
        var title = UIContainer.GetComponentInChildren<EmptyTitle>().GetComponent<TextMeshProUGUI>();
        title.text = "STAGE " + GameService.Instance.Current_level;

        if ((bool)(SafeInt)(int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Tutorial,0) == false)
        {
            TutorialContainer.SetActive(true);
        }
        else
            TutorialContainer.SetActive(false);

    }
    private void HideMainUI() => UIContainer.SetActive(false);

    public void SwitchCam()
    {
        if (_camPoint==1)
        {
            
            var mainCam = Camera.main.gameObject;
            mainCam.transform.SetParent(_camPoint2.transform);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _camPoint = 2;
            RenderSettings.fog = false;
        }
        else
        {
            var mainCam = Camera.main.gameObject;
            mainCam.transform.SetParent(_camPoint1.transform);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _camPoint = 1;
            RenderSettings.fog = true;
        }
    }

    public void SetCamToPoint()
    {
        var mainCam = Camera.main.gameObject;
        mainCam.transform.SetParent(_camPoint1.transform);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
        RenderSettings.fog = true;
    }

}