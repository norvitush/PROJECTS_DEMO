using System.Collections;
using UnityEngine;
using TMPro;
using VOrb;

public class InfoBar : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _output;
    [SerializeField] protected float _autoHideDelay = 5f;
    private IEnumerator _infoHide = null;

    public virtual void Show()
    {

        if (gameObject.activeInHierarchy)
        {
            if (_infoHide != null)
            {
                UIWindowsManager.Instance.StopCoroutine(_infoHide);
                _infoHide = null;
            }
            gameObject.SetActive(false);
        }
        else
        {
            UpdateContent();

            gameObject.SetActive(true);
            if (_infoHide == null)
            {
                _infoHide = AutoHide();
                UIWindowsManager.Instance.StartCoroutine(_infoHide);
            }


        }

    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(_autoHideDelay);
        gameObject.SetActive(false);
        _infoHide = null;
    }

    protected virtual void UpdateContent()
    {

    }

}
