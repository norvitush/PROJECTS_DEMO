
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VOrb.CubesWar;
using VOrb;
using VOrb.CubesWar.VFX;
using System;
using VOrb.CubesWar.Levels;

public class StartWindow : Window
{
    [SerializeField] private GameObject debugPanel;
    [SerializeField]private GameObject _levelsContainer;
    [SerializeField] private WindowPause _pauseWindow;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private win_window _Congratulation;
    [SerializeField] private GameObject _noads;
    [SerializeField] private GameObject _dimmedScreen;
    public bool Dimmed { set => _dimmedScreen.SetActive(value); }
    public bool NoAdsShow { set => _noads.GetComponent<Image>().enabled = value; }

    public Action InvokeAfterLoad;
    public string Score { set => _score.text = value; }
    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

    }

    public void ShowDebug() => debugPanel.SetActive(!debugPanel.activeInHierarchy);

    protected override void SelfOpen()
    {

        gameObject.SetActive(true);
        TapEvent.RemoveAllListeners();
        TapEvent.AddListener(()=> { });
        SvipeEvent.RemoveAllListeners();
        SvipeEvent.AddListener(()=> { });
        _levelsContainer.SetActive(Time.timeScale != 0);

        if (Time.timeScale == 0)
        {
            _pauseWindow?.Open(ChangeCurrentWindow);  //opening the pause window   
            NoAdsShow = false;
        }
        else
        {
            _pauseWindow?.Close();
            _Congratulation.gameObject.SetActive(false);
            InvokeAfterLoad?.Invoke();
            InvokeAfterLoad = null;
            NoAdsShow = (GameService.Instance.NoAds == 0);
        }
        Score = GameService.Instance.GlobalScore + "/2022";
        SoundService.PlayPauseBackground(true);
       
        Dimmed = false;
    }
    public void ShowWinnerScreen()
    {
        SoundService.PlaySound(Sound.Congratulation);
        _Congratulation.gameObject.SetActive(true);
    }

public void BtnStartClick(int level = 1)
    {
        SoundService.PlaySound(Sound.ButtonClick);
        SoundService.PlayPauseBackground(false);
        Close();
        MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
        GameService.ActiveNow = UIWindowsManager.GetWindow<MainWindow>();
        var levelInfo = DataBaseManager.Instance.LevelsInfo.Find((lvl) => lvl.LevelNumber == level);
        if (levelInfo!=null)
        {
            GameService.Instance.StartGame(levelInfo);
        }
        
    }

    public void NoAdsClick()
    {
        Dimmed = true;
    }
   
    public void BuyFailed()
    {
        Dimmed = false;
    }
    public void BtnExitClick()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        QuitNow();
    }

    private void QuitNow()
    {
        Application.Quit();
    }
    public void BtnSettingsClick()
    {
        SoundService.PlaySound(Sound.ButtonClick);
        UIWindowsManager.GetWindow<MainWindow>().OpenSettingsWindow();
    }

    
}









