using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Santa_changer : MonoBehaviour
{
    [SerializeField] Sprite[] santas;
    [SerializeField] Image santa;

    // Start is called before the first frame update
    void Start()
    {
        santa.sprite = santas[Random.Range(0,santas.Length)];
    }


}
