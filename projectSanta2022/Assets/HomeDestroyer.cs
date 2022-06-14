using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.CubesWar;

public class HomeDestroyer : MonoBehaviour
{
    private HomesLife _homeController;
    private void Start()
    {
        _homeController = GameService.Instance.GameElements.GetComponentInChildren<HomesLife>(true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Empty_home home = collision.transform.GetComponentInParent<Empty_home>();
        if (home != null)
        {
            _homeController.CleareHomePoints(home);
            _homeController.ReleseMovebleObject(collision.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Empty_home home = other.transform.GetComponentInParent<Empty_home>();
        if (home != null)
        {
            _homeController.CleareHomePoints(home);
            _homeController.ReleseMovebleObject(other.gameObject);
        }
    }
    
}
