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
        Home home = collision.transform.GetComponentInParent<Home>();
        if (home != null)
        {
            _homeController.CleareHomeEnviropment(home);
            _homeController.ReleseMovebleObject(collision.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        Home home = other.transform.GetComponentInParent<Home>();
        if (home != null)
        {
            _homeController.CleareHomeEnviropment(home);
            _homeController.ReleseMovebleObject(other.gameObject);
        }
    }
    
}
