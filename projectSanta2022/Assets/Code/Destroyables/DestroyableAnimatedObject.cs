using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.SantaJam;

public class DestroyableAnimatedObject : MonoBehaviour, IDestoyableItem
{
    [SerializeField] private Animator _selfAnimator;
    [SerializeField] private float _speed = 3f;
    [SerializeField] protected  PooledObjectType _poolType;

    protected Coroutine _destroying = null;
    public bool Destroyed { get; private set; }

    private void OnEnable()
    {
        _destroying = null;
        Destroyed = false;
    }
    public virtual void Destroy()
    {
        if (_selfAnimator != null)
        {
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
            transform.localScale = transform.localScale - (new Vector3(0, _speed * Time.deltaTime, 0));
            yield return null;
        }
        _destroying = null;
        Destroyed = true;
        TurnOff();
    }


}
