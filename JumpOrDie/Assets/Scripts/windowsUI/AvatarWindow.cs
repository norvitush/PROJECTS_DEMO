using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class AvatarWindow : Window
{        
    [SerializeField] private TextMeshProUGUI _totalCoins;
    [SerializeField] private TextMeshProUGUI _totalParts;
    [SerializeField] private AvatarScroller AllAvatars;
    [SerializeField] private FonScroller AllFons;
    [SerializeField] private InfoBar infoBar = null;
    [SerializeField] private Image _gameProgressBar = null;

    public GameObject[] points = new GameObject[11];

    public Image GameProgressBar { get => _gameProgressBar; set => _gameProgressBar = value; }

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
        SvipeEvent.RemoveAllListeners();
        
        TapEvent.AddListener(()=> { });
        SvipeEvent.AddListener(()=> { });
        CollectingSystem.Recieved AllGain = CollectingSystem.Instance.GetRecievedFromCollection();
        _totalCoins.text = AllGain.coins.ToString();
        _totalParts.text = AllGain.parts.ToString();
        infoBar?.Show();
        AllFons.UpdateCellsData();
    }

    
    public void OnPressAvatar(SafeSkinData tapedSkin)
    {
        SafeSkinData testSkin =  DataBaseManager.GetSkinInfo("", tapedSkin.PlaySkin_Id).Data;
        if (testSkin.PlaySkin_Id != 0)
        {
            GameService.Instance.PutAvatarToGame(tapedSkin);
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            mn.MainMessage.gameObject.SetActive(true);
            AudioServer.PlaySound(Sound.ButtonClick, 0.3f);
            Exit(false);
            UIWindowsManager.GetWindow<RestartWindow>().NewSkin.gameObject.SetActive(false);
        }
    }

    public void ExitClick()
    {
        AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
        Exit(true);
    }

    private void Exit(bool toStart)
    {
        GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(0);
        GameService.Instance.PartsTextMesh.text = "0";
        if (toStart)
        {
            UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow();
        }
        else
        {
            UIWindowsManager.ActiveNow = UIWindowsManager.GetWindow<MainWindow>();
 
            Close();
           
        }
    }

    internal void UpdateScrollWiew()
    {
        AllAvatars.UpdateCellsData();
    }

    public void PurchaseButton()
    {
        AudioServer.PlaySound(Sound.ButtonClick);
        
        if (AllAvatars.Selected.skinInfo.Data.Purchased==1)
        {
            OnPressAvatar(AllAvatars.Selected.skinInfo.Data);
            return;
        }
        
        var originSkinBase = DataBaseManager.Instance.GetPlayerSkinsCollection();
        
        SafeSkinData forReset = AllAvatars.Selected.skinInfo.Data;

        //ЕСЛИ ХВАТАЕТ ВАЛЮТЫ
        if (forReset.MaxParts<=CollectingSystem.Instance.GetRecievedFromCollection(forReset))
        {
            Debug.Log("Валюта - " + Enum.GetName(typeof(SkinType), (SkinType)(int)forReset.rareType));
            AllAvatars.PurchaseAnimator.Play("Bounce");
            CollectingSystem.Instance.SpendBalance(forReset.MaxParts, forReset);
            forReset.Purchased = 1;
            forReset.Enabled = 1;
            CollectingSystem.Instance.UpdateCollectionSkin(originSkinBase, forReset);
            CollectingSystem.Instance.UpdateAllSkinsCollection();
            DataBaseManager.Instance.SaveCollectionWithName(SceneLoader.sceneSettings.PlayerName);
            AllAvatars.UpdateCellsData();
            CollectingSystem.Recieved AllGain = CollectingSystem.Instance.GetRecievedFromCollection();
            _totalCoins.text = AllGain.coins.ToString();
            _totalParts.text = AllGain.parts.ToString();
            AudioServer.PlaySound(Sound.ChallengeDone, 0.8f);
            AudioServer.PlaySound(Sound.Purchase, 0.3f);
        }
    }
}
