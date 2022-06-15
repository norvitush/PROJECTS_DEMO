using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;


public class BounceControl : MonoBehaviour
{
    private AvatarController _driver;
    private MeshCollider _mesh = null;


    private void Start()
    {
        _driver = GameService.Instance.AvatarControl;
        _mesh = gameObject.GetComponent<MeshCollider>();
        _driver.InGround = false;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _driver.InGround == false)
        {            
            _driver.InGround = true;
            StopFall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameElem ObjGameElem = other.gameObject.GetComponent<GameElem>();

        if (ObjGameElem != null)
        {
            CollectingSystem.Instance.OnCollect(DataBaseManager.FindItem("", ObjGameElem.ID));
        }

        if (other.gameObject.CompareTag("Ground") && _driver.InGround==false)
        {
            _driver.InGround = true;
            StopFall();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") && _driver.InGround )
        {
            _driver.InGround = false;

            if (_mesh.material.bounciness
                != GameService.Instance.Current_physicMaterial.bounciness)
            {
                _mesh.material.bounciness = GameService.Instance.Current_physicMaterial.bounciness;                                     
            }            

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_driver.InFall && _driver.InGround) //свайпнули внутри тригерного коллайдера Земли
        {
            StopFall();
        }
    }
    private void StopFall()
    {
            _driver.PlayerJumpRemains = SceneLoader.sceneSettings.JumpLimit;
            if (_driver.InFall)
            {
                _driver.FallEnds();
            }
    }

}
