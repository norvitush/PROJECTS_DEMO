using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    public Text viewText;
    private void OnEnable()
    {
       // viewText.text = SceneLoader.DebugLog.debugString;
    }
}
