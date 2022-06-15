using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VOrb;
using UnityEngine.UI;

public class UIEffects : Singlton<UIEffects>
{
    public static void SplashMainScreen(string sp_Text = "Tap !")
    {
        Animator SplashTextAnimator;
        TextMeshProUGUI txtMesh;        
        GameObject SplashText = GameObjectPool.Instance.GetPooledObject(PooledObjectType.TapMessage);
        if (SplashText == null)
        {
            return;
        }
        SplashTextAnimator = SplashText.GetComponent<Animator>();
        txtMesh = SplashText.GetComponent<TextMeshProUGUI>();

        string[] strLines = new string[10];

        strLines = sp_Text.Split(' ');
        txtMesh.text = strLines[0] + " \n ";
        for (int i = 1; i < strLines.Length; i++)
        {
            txtMesh.text += strLines[i] + " ";
        }

        SplashText.SetActive(true);
        SplashTextAnimator.Play("SplashShow",-1,0);
        Instance.StartCoroutine(Instance.DeactivateSplash(SplashText, 1.7f));
    }

    private IEnumerator DeactivateSplash(GameObject obj, float tm)
    {
        yield return new WaitForSeconds(tm);
        obj?.SetActive(false);        
        
    }

    public static void SplashGainCoin(string splashText = "x1")
    {
        Animator TextAnimator;
        TextMeshProUGUI txtMesh;
        GameObject Text = GameObjectPool.Instance.GetPooledObject(PooledObjectType.FlowUpNumbers);
        
        TextAnimator = Text?.GetComponent<Animator>();
        txtMesh = Text?.GetComponent<TextMeshProUGUI>();
        txtMesh.text = splashText;
        if (Text!=null)
        {
            Text.SetActive(true);
            TextAnimator?.Play("text_up", -1, 0);
        }    
        
    }
    public static IEnumerator AnimateDecrScore(GameObject FlowText, Vector3 WorldCoord_p, Vector3 WorldCoord_T, string text, bool FaidOut = true, float smooth = 3)
    {
        Color curColor;
        TextMeshProUGUI curText;

        curText = FlowText.GetComponent<TextMeshProUGUI>();
        curText.text = text;
        Vector3 current_pos = WorldCoord_p;
        FlowText.transform.position = current_pos;


        float timer = 0;
        float TextAlpha = 0.8f;

        while (!current_pos.isNearToDestiny(WorldCoord_T, 1f) && timer <= 3)
        {
            FlowText.gameObject.SetActive(true);
            current_pos = Vector3.Lerp(current_pos, WorldCoord_T, Time.deltaTime * smooth);
            curColor = new Color(curText.color.r, curText.color.g, curText.color.b, TextAlpha);
            curText.color = curColor;
            if (FaidOut)
            {
                TextAlpha *= (1f - 0.03f * smooth);
            }
            

            FlowText.transform.position = current_pos;
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            FlowText.gameObject.SetActive(false);
        }

    }


    public static UIElement GetUIElementFrom(RaycastHit[] rayHitArray)
    {
        int maxUI_id = 0;
        UIElement maxUIElement = null;
        foreach (var _hit in rayHitArray)
        {
            UIElement hitedUI = _hit.transform.gameObject.GetComponent<UIElement>();
            if (hitedUI != null)
            {
                int ui_id = hitedUI.ID;
                if (ui_id > maxUI_id)
                {
                    maxUI_id = ui_id;
                    maxUIElement = hitedUI;
                }
            }
        }
        return maxUIElement;
    }
}
