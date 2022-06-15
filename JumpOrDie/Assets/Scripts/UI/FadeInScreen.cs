using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FadeInScreen : MonoBehaviour
{
    private const float MAX_VAL = 0.98f;

    private bool _needToFade = false;
    private bool _inLoadding = false;
    private Image _screen;
    

    void Start()
    {
        _screen = gameObject.transform.Find("Image").GetComponent<Image>();
        _screen.color = new Color(_screen.color.r, _screen.color.g, _screen.color.b, 0);
        _needToFade = true;
    }

    private void FixedUpdate()
    {
        if (_needToFade)
        {
            _screen.color = new Color(_screen.color.r, _screen.color.g, _screen.color.b, _screen.color.a+Time.deltaTime);
            if (_screen.color.a >= MAX_VAL && !_inLoadding)
            {
                _inLoadding = true;
                SceneManager.LoadSceneAsync(1);                
            }
        }

    }

}
