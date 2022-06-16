using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.CubesWar;

public class SantaController : MonoBehaviour
{
    private const float BASE_SCALE_GIFT = 0.0266f;
    private const float BASE_SCALE_X = 0.0333f;
    private const float BASE_SCALE_Y = 0.0133f;
    private const float BASE_SCALE_Z = 0.019f;

    [SerializeField] private GameObject _gift;
    [SerializeField] private GameObject _giftPoint;
    [SerializeField] private Animator _animator;
    private Vector3 _storedScale;

    public void HookTheGift(PooledObject gift)
    {
        _gift = gift.GameObject;
        _storedScale = _gift.transform.localScale;
        _gift.transform.SetParent(_giftPoint.transform);
        _gift.transform.localPosition = Vector3.zero;
        _gift.transform.localRotation = Quaternion.Euler(Vector3.zero);
        if (gift.SelfType== PooledObjectType.Gift)
        {
            _gift.transform.localScale = new Vector3(BASE_SCALE_GIFT, BASE_SCALE_GIFT, BASE_SCALE_GIFT);
        }
        if (gift.SelfType == PooledObjectType.Brick)
        {
            _gift.transform.localScale = new Vector3(BASE_SCALE_X, BASE_SCALE_Y, BASE_SCALE_Z);
        }


    }
    public void ReturnForThrow(Transform parent)
    {
        _gift.transform.SetParent(parent);
        _gift.transform.localScale = _storedScale;
        _gift = null;
    }

    internal void PlayReadyMove()
    {
        _animator.ResetTrigger("dropReadyState");
        _animator.ResetTrigger("throw");
        _animator.SetTrigger("firstTap");
    }

    internal void PlayDropReadyMove()
    {
        _animator.SetTrigger("dropReadyState");
    }

    internal void PlayThrow()
    {
        _animator.SetTrigger("throw");

    }
    internal void DropState()
    {
        _animator.ResetTrigger("dropReadyState");
        _animator.ResetTrigger("throw");
        _animator.ResetTrigger("firstTap");
        _animator.Play("idle");
    }
}
