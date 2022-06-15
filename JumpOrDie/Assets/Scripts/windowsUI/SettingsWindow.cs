using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

public class SettingsWindow : Window
{
    [SerializeField] private GameObject _btnGooglePLay;
    [SerializeField] private GameObject _btnSounds;
    [SerializeField] private GameObject _btnHardness;
    public ScrollRect debugRect;

    protected override void SelfClose()
    {
        gameObject.SetActive(false);        
    }

    protected override void SelfOpen()
    {

        gameObject.SetActive(true);
        UpdateButtonContent();
        _btnHardness.transform.Find("Text").GetComponent<Text>().text = GameService.Instance.PlayerState.EasyMode ? "Easy" : "Hard";
        if (SceneLoader.sceneSettings.IsDebugMode)
        {
            ///отладка
        }
        
    }

    private void UpdateButtonContent()
    {
        if (GameService.Instance.PlayServicesConnected)
        {
            _btnGooglePLay.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_green");
        }
        else
        {
            _btnGooglePLay.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_gray");
        }


        if (GameService.Instance.Sounds)
        {
            _btnSounds.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_green");
            Image lable = _btnSounds.transform.Find("Image").GetComponent<Image>();
            lable.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_soundOn");
            lable.SetNativeSize();           
        }
        else
        {
            _btnSounds.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_gray");
            Image lable = _btnSounds.transform.Find("Image").GetComponent<Image>();
            lable.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_soundOff");
            lable.SetNativeSize();
        }      
    }

    public void BtnGooglePlayClick()
    {
       //Подключение сервисов GOOGLE

    }
     
    public void BtnSoundsClick()
    {
        if (_btnSounds!=null)
        {
            GameService.Instance.Sounds = !GameService.Instance.Sounds;
            DataKeeper.SaveParam("Sounds", GameService.Instance.Sounds ? 1 : 0);
            UpdateButtonContent();
        }
        AudioServer.PlaySound(Sound.ButtonClick);
    }
    public void BtnHardnessClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        if (_btnHardness != null)
        {
            bool EasyMode = !GameService.Instance.PlayerState.EasyMode;
            bool TuturEnded = GameService.Instance.PlayerState.TutorialEnded;
            GameService.Instance.SetPlayerPrefs(TuturEnded,EasyMode);
            DataKeeper.SaveParam("easymode", EasyMode?1:0);
            _btnHardness.transform.Find("Text").GetComponent<Text>().text = EasyMode ? "Easy" : "Hard";

            Rigidbody AvatarRig = GameService.Instance.GetAvatarObj()?.GetComponent<Rigidbody>();
            if (AvatarRig!=null)
            {
                AvatarRig.constraints = RigidbodyConstraints.None;
                AvatarRig.constraints |= RigidbodyConstraints.FreezePositionX;
                AvatarRig.constraints |= RigidbodyConstraints.FreezePositionZ;
                if (EasyMode)
                {
                    AvatarRig.constraints |= RigidbodyConstraints.FreezeRotationX;
                    //AvatarRig.constraints |= RigidbodyConstraints.FreezeRotationY;
                    AvatarRig.constraints |= RigidbodyConstraints.FreezeRotationZ;
                }
            }

            
        }
    }
    public void BtnCloseClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick,0.5f);
        UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow();
    }
   
    public void ShowLiderBoard()
    {
        //GOOGLE LIDERBOARD
    }
    public void ClearScroll()
    {
            
    }

}
