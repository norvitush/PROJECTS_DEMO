using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.CubesWar;

public class DestoyableForSmile : DestroyableAnimatedObject
{
    public override void Destroy()
    {
        GameService.Instance.ScoreGiftsCount = Mathf.Clamp(GameService.Instance.ScoreGiftsCount + Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.currentLevel.Speed), 1, 10), 0, int.MaxValue);
        UITextEffects.SplashMainScreen("+" + Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.currentLevel.Speed), 1, 5), new TextEffectBuilder()
                       .MakeMovable()
                       .Biuld(Camera.main.WorldToScreenPoint(transform.position)
                       , PooledObjectType.NumberPopup1_smile)
                       );
        base.Destroy();
    }
}
