
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VOrb;

public class StartWindow : Window
{

    public GameObject shop = null;
    public GameObject ShopLabel = null;
    public TextMeshProUGUI TimerRecord;
    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            
        }

    }

    protected override void SelfOpen()
    {

        gameObject.SetActive(true);
        TapEvent.RemoveAllListeners();
        TapEvent.AddListener(()=> { });
        SvipeEvent.RemoveAllListeners();
        SvipeEvent.AddListener(()=> { });
        if(shop!=null)
        {
            shop.SetActive(Time.timeScale != 0);
            if (CollectingSystem.Instance.NewSkinsCount!=0)
            {
                ShopLabel.gameObject.SetActive(true);
                ShopLabel.transform.Find("lable").GetComponent<TextMeshProUGUI>().text = CollectingSystem.Instance.NewSkinsCount.ToString();                 
            }
            else
            {
                ShopLabel.gameObject.SetActive(false);
            }
            
        }
        TimerRecord.text = GameService.Instance.BestTime.ToString("0.00");
    }
   

    public void BtnSkinsClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        UIWindowsManager.GetWindow<MainWindow>().OpenAvatarWindow(true);
    }


    public void BtnStartClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        //start
        if (Time.timeScale != 0)
        {
            if (CollectingSystem.Instance.CheckLvlComplete() == true)
            {
                CollectingSystem.Instance.GetPlayerAvatar(out SafeSkinData selectSkin);                               
                Close();
                GameService.Instance.PutAvatarToGame(selectSkin);
                MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
                mn.MainMessage.text = SceneLoader.sceneSettings.BeginText;
                mn.MainMessage.gameObject.SetActive(true);
                UIWindowsManager.ActiveNow = UIWindowsManager.GetWindow<MainWindow>();
                mn.ChallengeInfo.Show();
            }
            else
            {
                GameService.Instance.PlayerIsStunned = false;
                GameService.Instance.CompleteLastSession();
            }
        }
        else
        {
            
            Close();
            GameService.Instance.Pause();
            UIWindowsManager.GetWindow<MainWindow>().Btn_pause.SetActive(true);
            UIWindowsManager.GetWindow<MainWindow>().Btn_timer.SetActive(true);
            UIWindowsManager.ActiveNow = UIWindowsManager.GetWindow<MainWindow>();
        }

    }

    public void BtnExitClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
        //СОХРАНЕНИЕ?
        QuitNow();
    }

    private void QuitNow()
    {
        Application.Quit();
    }
    public void BtnSettingsClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        //СОХРАНЕНИЕ?
        UIWindowsManager.GetWindow<MainWindow>().OpenSettingsWindow();
    }
}









