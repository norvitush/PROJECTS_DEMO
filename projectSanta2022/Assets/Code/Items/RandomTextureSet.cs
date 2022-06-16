using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb.CubesWar;
public class RandomTextureSet : MonoBehaviour
{
 

    private void OnEnable()
    {
        Texture2D rand_texture = DataBaseManager.Instance.GetRandomGiftsTexture();
        gameObject.GetComponent<MeshRenderer>().material.mainTexture = rand_texture;
    }


}
