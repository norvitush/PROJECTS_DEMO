using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VOrb;

[RequireComponent(typeof(BoxCollider))]
public class GameElem : MonoBehaviour
{
    [Header("ID элемента для raycast`а или коллизий")]    
    public int ID = 0;
    public string hint = "";

}


