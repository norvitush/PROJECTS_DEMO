using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VOrb;
using TMPro;
using UnityEngine.Events;

public class MainWindow : Window
{

    public delegate void loadCallback();
    public loadCallback ExecuteAfterLoad;

    [SerializeField] private SettingsWindow settings_window;
    [SerializeField] private AvatarWindow avatar_window;
    [SerializeField] private RestartWindow restart_window;
    [SerializeField] private StartWindow start_window;
    [SerializeField] private ChallengeInfoBar _challengeInfo;
    [SerializeField] private GameObject _btn_info;
    [SerializeField] private GameObject _btn_pause;
    [SerializeField] private GameObject _btn_timer;
    [SerializeField] private TextMeshProUGUI _mainMessage;
    [SerializeField] private BarProgress _healthBar;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _partText;
    [SerializeField] private TextMeshProUGUI _bonusText;
    [SerializeField] private GameObject _bonusFon;
    [SerializeField] private EffectsPlayer _dieEffect;
    [SerializeField] private ParticleSystem _splash_Timer;
    [SerializeField] private ParticleSystem _itemPreViewEffect;
    [SerializeField] private ParticleSystem _stampEffect;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private Animator _splash_Best;

    public ChallengeInfoBar ChallengeInfo => _challengeInfo; 
    public GameObject Btn_info  => _btn_info; 
    public GameObject Btn_pause  => _btn_pause; 
    public GameObject Btn_timer  => _btn_timer; 
    public TextMeshProUGUI MainMessage => _mainMessage; 
    public BarProgress HealthBar => _healthBar; 
    public TextMeshProUGUI CoinText  => _coinText; 
    public TextMeshProUGUI PartText => _partText; 
    public TextMeshProUGUI BonusText => _bonusText;
    public GameObject BonusFon  => _bonusFon; 
    public EffectsPlayer DieEffect => _dieEffect;
    public ParticleSystem Splash_Timer => _splash_Timer;
    public ParticleSystem ItemPreViewEffect => _itemPreViewEffect;
    public ParticleSystem StampEffect => _stampEffect;
    public TextMeshProUGUI Timer => _timer;
    public Animator Splash_Best => _splash_Best;

    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    protected override void SelfOpen()      //эффекты открытия и другая красота - здесь
    {                                       
        settings_window = UIWindowsManager.GetWindow<SettingsWindow>();
        avatar_window = UIWindowsManager.GetWindow<AvatarWindow>();
        restart_window = UIWindowsManager.GetWindow<RestartWindow>();
        start_window = UIWindowsManager.GetWindow<StartWindow>();

        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() => {});
        SvipeEvent.AddListener(() => { });

        gameObject.SetActive(true);
        UIWindowsManager.ActiveNow = this;

 
        string beg_txt = SceneLoader.sceneSettings.BeginText;
        string[] strLines = new string[10];
        strLines = beg_txt.Split(' ');
        if (strLines.Length > 2)
        {
            MainMessage.text = strLines[0] +" "+ strLines[1] + " \n ";
            for (int i = 2; i < strLines.Length; i++)
            {
                MainMessage.text += strLines[i] + " ";
            }
        }
        else
            MainMessage.text = beg_txt;
        ExecuteAfterLoad?.Invoke();
        ExecuteAfterLoad = null;
       
    }
    public void OpenSettingsWindow()
    {
        UIWindowsManager.ActiveNow = settings_window;
        settings_window?.Open(ChangeCurrentWindow);  //opening the settings window   
        HideNotNeededs();
        
    }



    public void OpenAvatarWindow(bool needToHideScore)
    {
        UIWindowsManager.ActiveNow = avatar_window;
        avatar_window?.Open(ChangeCurrentWindow);  //opening the settings window  
        if (needToHideScore)
        {
            GameService.Instance.ShowScoreContainer(false);
        }
        else
        {
            GameService.Instance.ShowScoreContainer(true);
        }
       
    }
    public void OpenStartWindow()
    {
        UIWindowsManager.ActiveNow = start_window;
        start_window?.Open(ChangeCurrentWindow);  //opening the settings window  
        HideNotNeededs();

    }
    public void OpenRestartWindow(bool needToHideScore)
    {
        UIWindowsManager.ActiveNow = restart_window;
        HealthBar.SetProgress(0);
        restart_window?.Open(ChangeCurrentWindow);  //opening the settings window  

        if (needToHideScore)
        {
            GameService.Instance.ShowScoreContainer(false);
        }
        else
        {
            GameService.Instance.ShowScoreContainer(true);
        }

    }
    public void CloseChildren()
    {
        CurrentWindow.Close();
    }

    private void HideNotNeededs()
    {
        GameService.Instance.ShowScoreContainer(false);
    }


    public void Pause()
    {
        AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
        Btn_pause.SetActive(false);
        Btn_timer.SetActive(false);
        GameService.Instance.Pause();
        UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow();
    }
    public void SetFon(int id)
    {
        Transform Container;        
        Container = GameService.Instance.FonContainer.transform;
      
        var loaded = Resources.Load("prefabs/levels/lvl_" + id, typeof(GameObject));

        if (loaded != null)
        {
            GameService.Instance.CleanContainer(GameService.Instance.FonContainer);
            GameObject newFon = Instantiate(loaded, Container) as GameObject;
            newFon.SetActive(true);            
            GameService.Instance.CurrentFonNumber = id;
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_CurrentFonNumber", id);            
        }
        else
        {
            GameObject newFon = Instantiate(Resources.Load("prefabs/levels/lvl_1", typeof(GameObject)), Container) as GameObject;
            if (newFon != null)
            {
                GameService.Instance.CleanContainer(GameService.Instance.FonContainer);
                newFon.SetActive(true);
                GameService.Instance.CurrentFonNumber = id;
            }

        }

        
    }
}
