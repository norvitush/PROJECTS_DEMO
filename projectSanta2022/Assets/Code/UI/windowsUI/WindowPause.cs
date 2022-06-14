using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VOrb.CubesWar;
using TMPro;
using UnityEngine.Events;
using System;
using System.Linq;
using UnityEngine.UI;
using VOrb.CubesWar.Levels;

public class WindowPause : Window
{
    [SerializeField] TextMeshProUGUI _stageInfo;
    [SerializeField] Sprite _sound_on;
    [SerializeField] Sprite _sound_of;
    [SerializeField] Image _check;
    [SerializeField] GameObject _btnSound;
    private Image _iconSound = null;
    private TextMeshProUGUI _soundText = null;

    public Action afterInit;
    public Action afterClose;
    
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
        SvipeEvent.AddListener(() => { });

        gameObject.SetActive(true);
        GameService.ActiveNow = this;
        _stageInfo.text = "Stage " + GameService.Instance.Current_level;
        afterInit?.Invoke();

        if (_iconSound==null)
        {
            _iconSound = _btnSound?.transform.Find("Icon").GetComponent<Image>();
            _soundText = _btnSound?.GetComponentInChildren<TextMeshProUGUI>();
        }
        _iconSound.sprite = GameService.Instance.Sounds ? _sound_on : _sound_of;
        _soundText.text = GameService.Instance.Sounds ? "Sound On" : "Sound Off";
        SoundService.PlayPauseBackground(true, withIncrease: false);
    }

    public void OnResume()
    {
        UIWindowsManager.GetWindow<StartWindow>().BtnStartClick(GameService.Instance.Current_level);       
    }

    public void OnRestart()
    {
        SoundService.PlaySound(Sound.ButtonClick);

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


        int level = GameService.Instance.currentLevel.LevelNumber;
        GameService.Instance.StopGame( WithoutCalc: true);
        UIWindowsManager.GetWindow<StartWindow>().BtnStartClick(level);
    }

    public void OnGiveUp()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        GameService.Instance.StopGame(true);
    }

    public void OnSound()
    {
        GameService.Instance.Sounds = !GameService.Instance.Sounds;
        SoundService.PlaySound(Sound.ButtonClick);
        _iconSound.sprite = GameService.Instance.Sounds ? _sound_on : _sound_of;
        _soundText.text = GameService.Instance.Sounds ? "Sound On" : "Sound Off";
        GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Sound, GameService.Instance.Sounds ? 1 : 0);
        
        SoundService.AttestMusic();
        if (GameService.Instance.Sounds)
        {
            SoundService.PauseMusic(true);
        }
        SoundService.PlayPauseBackground(GameService.Instance.Sounds,withIncrease: false);

    }

    public void OnDropProgress()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        GameStorageOperator.DropSavedPlayerInfo();
        SceneLoader.UpdateFromStorage();
        string player = SceneLoader.sceneSettings.PlayerName + SystemInfo.deviceUniqueIdentifier;
        GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Player, player);
        OnGiveUp();
    }

    public void TestModeSwitch()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        SceneLoader.sceneSettings.IsTestMode = !SceneLoader.sceneSettings.IsTestMode;
        _check.gameObject.SetActive(SceneLoader.sceneSettings.IsTestMode);
    }
}