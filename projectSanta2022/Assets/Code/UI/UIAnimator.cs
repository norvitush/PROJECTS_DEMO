using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.SantaJam;

public class UIAnimator : MonoBehaviour
{
    [SerializeField] private Animator _smileCoinAnimator;
    [SerializeField] private Animator _giftCountAnimator;
    public GameObject CoinSmile;
    public GameObject CoinAngry;
    public GameObject CoinHmm;

    public Vector3 SmileAnimatorObjectPoint => _smileCoinAnimator.gameObject.transform.position;
    //public Vector3 SmileAnimatorObjectPoint => _smileCoinAnimator.gameObject.transform.position;
    public void SmileCountBounce()
    {
        _smileCoinAnimator.SetTrigger("bounce");
        UIWindowsManager.GetWindow<MainWindow>().SetSmilesInfo(GameService.Instance.SmilesScore);
    }
}
