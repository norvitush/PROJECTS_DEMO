using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UIElement : MonoBehaviour
{
    [Header("ID элемента для raycast`а")]
    public VOrb.UIElementType type = VOrb.UIElementType.UISkin;
    public int ID = 0;
    public string hint = "";
}