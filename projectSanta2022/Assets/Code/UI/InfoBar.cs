using System.Collections;
using UnityEngine;
using TMPro;
using VOrb;

public class InfoBar : MonoBehaviour
{
    public TextMeshProUGUI Output;
    public float AutoHideDelay = 5f;
    private IEnumerator infoHide = null;

    public virtual void Show()
    {

        if (gameObject.activeInHierarchy)
        {
            if (infoHide != null)
            {
                UIWindowsManager.Instance.StopCoroutine(infoHide);
                infoHide = null;
            }
            gameObject.SetActive(false);
        }
        else
        {
            UpdateContent();

            gameObject.SetActive(true);
            if (infoHide == null)
            {
                infoHide = AutoHide();
                UIWindowsManager.Instance.StartCoroutine(infoHide);
            }


        }

    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(AutoHideDelay);
        gameObject.SetActive(false);
        infoHide = null;
    }

    protected virtual void UpdateContent()
    {

    }

}
