using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.CubesWar;



public class Empty_home : DestroyableAnimatedObject
{ 
    internal PooledObjectType type { get => _poolType; set=>_poolType = value; }
    [SerializeField] ParticleSystem _smoke;
    [SerializeField] GameObject _fence;
    [SerializeField] ParticleSystem[] _lights;
    [SerializeField] public GameObject[] _points;

    public bool InDestroyProcess => _destroying!=null;
    

    public void SetActiveSmoke(bool value) => _smoke?.gameObject.SetActive(value);
    public void SetActiveLight(bool value)
    {
        foreach (var window in _lights)
        {
            window?.gameObject.SetActive(value);
        }
    }
    public void SetActiveFence(bool value) => _fence?.SetActive(value);
    public void SetActiveNumber(bool value) 
	{
        if (value)
            GetComponentInChildren<ChimneySensor>()?.ShowNumber();
        else
            GetComponentInChildren<ChimneySensor>()?.HideNumber();

    }
    public bool isNumberOn => GetComponentInChildren<ChimneySensor>().isNumberOn;

 


    public override void Destroy()
    {
        GameService.Instance.ScoreGiftsCount = Mathf.Clamp(GameService.Instance.ScoreGiftsCount - Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.currentLevel.Speed), 1, 10), 0, int.MaxValue);
        UITextEffects.SplashMainScreen("-" + Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.currentLevel.Speed), 1, 10), new TextEffectBuilder()
                   .MakeMovable()
                   .Biuld(Camera.main.WorldToScreenPoint(transform.position)
                   , PooledObjectType.NumberPopup2_angry)
                   );
        base.Destroy();
    }    


}
