using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VOrb.CubesWar;
using VOrb;
using TMPro;
using UnityEngine.Events;
using System;
using System.Linq;

public class LevelResultWindow : Window
{
    private Action _invokeAfterClose;
    private int _starscount;
    [SerializeField] private GameObject[] _particleStars;
    [SerializeField] private GameObject[] _Stars;
    [SerializeField] private TextMeshProUGUI _smilesRewardText;
    [SerializeField] private float _starsCooldown = 0.3f;

    public void Init(Action invokeAfterClose, int stars)
    {
        _invokeAfterClose = invokeAfterClose;
        _starscount = stars;
    }

    protected override void SelfClose()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            //if (activateAfter!= null)
            //{
            // GameService.Instance.ActiveNow = activateAfter;
            //}
            _invokeAfterClose?.Invoke();
        }
    }

    protected override void SelfOpen()      //opening effects and other beauty-here
    {
        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() => {});
        SvipeEvent.AddListener(() => { });

        gameObject.SetActive(true);
        GameService.Instance.ActiveNow = this;
        foreach (var item in _Stars)
        {
            item?.SetActive(false);
        }
        foreach (var item in _particleStars)
        {
            item?.SetActive(false);
        }
        StartCoroutine(StarsShow(_starscount));
    }
   
    public void OnClose()
    {
        Close();
    }

    private IEnumerator StarsShow(int count)
    {
        yield return null;
      
        _smilesRewardText.text = (GameService.Instance.ScoreGiftsCount * GameService.Instance.multiplyX).ToString();
        _particleStars[0]?.SetActive(true);
        for (int i = 1; i < count+1; i++)
        {
            yield return new WaitForSeconds(_starsCooldown);
            _Stars[i-1]?.SetActive(true);
            yield return new WaitForSeconds(_starsCooldown/3);
            _particleStars[i]?.SetActive(true);
            SoundService.PlaySound(Sound.StarShow);
        }
        yield return new WaitForSeconds(0.5f);
        if (_starscount > 0 && GameService.Instance.NoAds == 0)
        {
            InterstitialShow();
        }
    }

    public void InterstitialShow()
    {
        //interstitial
    }




}