using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.CubesWar;

public class DestoyableEnviropment : DestroyableAnimatedObject
{
    public override void Destroy()
    {
        GameService.Instance.SmilesScore = Mathf.Clamp(GameService.Instance.SmilesScore + Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.CurrentLevel.Speed), 1, 10), 0, int.MaxValue);
        UITextEffects.SplashMainScreen("+" + Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.CurrentLevel.Speed), 1, 5), new TextEffectBuilder()
                       .MakeMovable()
                       .Biuld(Camera.main.WorldToScreenPoint(transform.position)
                       , PooledObjectType.NumberPopup1_smile)
                       );
        base.Destroy();
    }
}
