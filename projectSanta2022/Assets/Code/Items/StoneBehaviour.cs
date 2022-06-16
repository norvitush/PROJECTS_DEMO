using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;
using VOrb.CubesWar;

public class StoneBehaviour : MonoBehaviour
{
    private bool isActive;

    private bool _landed = false;
    public bool Landed => _landed;

    private void OnEnable()
    {
        isActive = true;
        _landed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.GetComponent<GroundBehaviour>()!=null && !_landed)
        {
            _landed = true;
            GameService.Instance.GameElements.GetComponentInChildren<HomesLife>().PushToMoveList(new PooledObject(gameObject, PooledObjectType.Brick));
        }
        if (isActive)
        {
            DestroyCheck(collision);
        }
        
    }
   
    private void DestroyCheck(Collision collision)
    {
        DestroyableAnimatedObject destroyable = null;
        ChimneySensor chimney = collision.gameObject.GetComponent<ChimneySensor>();
        if (chimney!=null)
        {
            destroyable = collision.gameObject.transform.parent.GetComponentInParent<DestroyableAnimatedObject>();
        }
        else
            destroyable = collision.gameObject.GetComponentInParent<DestroyableAnimatedObject>();

        if (destroyable != null)
        {

                isActive = false;
                destroyable.Destroy();
                SoundService.PlaySound(Sound.StoneBang);
                SelfDelete();
        }
    }
    private void SelfDelete()
    {
       
        GameService.Instance.GameElements.GetComponentInChildren<HomesLife>().DeleteFromMove(gameObject);
        gameObject.ReleaseToPool(PooledObjectType.Brick);
        
    }

}
