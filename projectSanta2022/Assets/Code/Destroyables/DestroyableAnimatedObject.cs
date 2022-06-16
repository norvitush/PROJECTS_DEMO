using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.CubesWar;

public class DestroyableAnimatedObject : MonoBehaviour, IDestoyableItem
{
    [SerializeField] Animator _selfAnimator;
    [SerializeField] float speed;
    [SerializeField] protected  PooledObjectType _poolType;

    protected Coroutine _destroying = null;
    public bool destroyed { get; private set; }

    private void OnEnable()
    {
        _destroying = null;
        destroyed = false;
    }
    private void Start()
    {
        //if (_selfAnimator!=null)
        //{
        //    _selfAnimator.speed = 0f;
        //}
        
    }
    public virtual void Destroy()
    {
        if (_selfAnimator != null)
        {
            //_selfAnimator.speed = 1;
            _selfAnimator.Play("delete");
        }
        else
        {
            _destroying = StartCoroutine(DecreaseScaleY());
        }
       
    }
    public void TurnOff()
    {
        GameService.Instance.GameElements.GetComponentInChildren<HomesLife>(true).DeleteFromMove(gameObject);
        gameObject.ReleaseToPool(_poolType);
        gameObject.SetActive(false);
    }

    IEnumerator DecreaseScaleY()
    {
        transform.localScale = transform.localScale * 0.98f;
        while (gameObject.activeInHierarchy && transform.localScale.y > 0f)
        {
            transform.localScale = transform.localScale - (new Vector3(0, speed * Time.deltaTime, 0));
            yield return null;
        }
        _destroying = null;
        destroyed = true;
        //transform.localScale = transform.localScale.SetYTo(transform.localScale.x);
        TurnOff();
    }


}
