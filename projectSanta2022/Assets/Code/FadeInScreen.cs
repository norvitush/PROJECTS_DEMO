using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FadeInScreen : MonoBehaviour
{
    bool NeedToFade = false;
    bool Inloadding = false;
    private Image screen;
    private const float MAX_VAL = 0.98f;

    void Start()
    {
        screen = transform.Find("Image").GetComponent<Image>();
        screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, 0);
        NeedToFade = true;
    }



    private void FixedUpdate()
    {
        if (NeedToFade)
        {
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, screen.color.a+Time.deltaTime);
            if (screen.color.a >= MAX_VAL && !Inloadding)
            {
                Inloadding = true;
                SceneManager.LoadSceneAsync(1);                
            }
        }

    }

}
