using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.CubesWar;

public enum SmilesBages
{
    smile = 0,
    angry = 1,
    hmm = 2
}
public class text_up : MonoBehaviour
{

    [SerializeField] private AnimationCurve _speedByTime;
    [SerializeField] private SmilesBages selfSmileType;
    Transform coin;
    Vector3 baseLocalPosition;

    [SerializeField] private float MoveTime  = 1f;
    [SerializeField] private float smoothness = 3f;
    [SerializeField] private float targetRadus = 0.22f;

    

    private void Start()
    {
        coin = transform.GetChild(0);
        baseLocalPosition = coin.localPosition;
    }
    private void OnEnable()
    {
        StartCoroutine(PoolUp());
    }
    private IEnumerator PoolUp()
    {
        
        float expiredSeconds = 0;
        float progress = 0;        

        while (progress < 1)
        {
            expiredSeconds = expiredSeconds > MoveTime ? 0 : (expiredSeconds + Time.deltaTime);

            progress = expiredSeconds / MoveTime;

            transform.Translate(transform.up * Time.deltaTime * _speedByTime.Evaluate(progress),Space.World);          

            yield return null;
        }
        float timer = 3;
        
        if (coin!=null && selfSmileType!= SmilesBages.hmm)
        {
            Vector3 destiny = UITextEffects.animator.SmileAnimatorObjectPoint;
            while (!coin.position.isNearToDestiny(destiny, targetRadus) && timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
                coin.position = Vector3.Lerp(coin.position, destiny, smoothness * Time.deltaTime);
            }
            UITextEffects.animator.SmileCountBounce();
            SoundService.PlaySound(Sound.Coin);
        }
        
       
        PooledObjectType forRet = PooledObjectType.NumberPopup1_smile;
        switch (selfSmileType)
        {
            case SmilesBages.smile:
                forRet = PooledObjectType.NumberPopup1_smile;
                break;
            case SmilesBages.angry:
                forRet = PooledObjectType.NumberPopup2_angry;
                break;
            case SmilesBages.hmm:
                forRet = PooledObjectType.NumberPopup3_hmm;
                break;
            default:
                break;
        }
        gameObject.ReleaseToPool(forRet);
        coin.localPosition = baseLocalPosition;
    }

 

    private void OnDisable()
    {
        if (coin!=null)
        {
            coin.localPosition = baseLocalPosition;
        }
        
    }



}
