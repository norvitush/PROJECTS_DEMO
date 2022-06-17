using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.SantaJam;

public enum SmilesBages
{
    smile = 0,
    angry = 1,
    hmm = 2
}
public class TextUp : MonoBehaviour
{
    private Transform _coin;
    private Vector3 _baseLocalPosition;

    [SerializeField] private AnimationCurve _speedByTime;
    [SerializeField] private SmilesBages _selfSmileType;
    [SerializeField] private float _moveTime  = 1.5f;
    [SerializeField] private float _smoothness = 3f;
    [SerializeField] private float _targetRadus = 0.25f;

    private void Start()
    {
        _coin = transform.GetChild(0);
        _baseLocalPosition = _coin.localPosition;
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
            expiredSeconds = expiredSeconds > _moveTime ? 0 : (expiredSeconds + Time.deltaTime);

            progress = expiredSeconds / _moveTime;

            transform.Translate(transform.up * Time.deltaTime * _speedByTime.Evaluate(progress),Space.World);          

            yield return null;
        }
        float timer = 3;
        
        if (_coin!=null && _selfSmileType!= SmilesBages.hmm)
        {
            Vector3 destiny = UITextEffects.animator.SmileAnimatorObjectPoint;
            while (!_coin.position.isNearToDestiny(destiny, _targetRadus) && timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
                _coin.position = Vector3.Lerp(_coin.position, destiny, _smoothness * Time.deltaTime);
            }
            UITextEffects.animator.SmileCountBounce();
            SoundService.PlaySound(Sound.Coin);
        }
        
       
        PooledObjectType forRet = PooledObjectType.NumberPopup1_smile;
        switch (_selfSmileType)
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
        _coin.localPosition = _baseLocalPosition;
    }

 

    private void OnDisable()
    {
        if (_coin!=null)
        {
            _coin.localPosition = _baseLocalPosition;
        }
        
    }



}
