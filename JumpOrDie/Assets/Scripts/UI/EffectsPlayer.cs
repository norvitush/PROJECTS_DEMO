using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VOrb;

public class EffectsPlayer : MonoBehaviour
{
    public Animator ani ;
    public UnityEvent EndAnimationCallback = null;

    public void StartAnimation(UnityAction callback)
    {
        EndAnimationCallback.RemoveAllListeners();
        EndAnimationCallback.AddListener(callback);
        ani.Play("CircleRotation");
    }
    public void DieAnimationEmpty()
    {
        ani.Play("CircleRotation");
    }

    public void OnEndAnimation()
    {
        ani.gameObject.SetActive(false);
        EndAnimationCallback?.Invoke();
        UIWindowsManager.GetWindow<MainWindow>().Splash_Timer.gameObject.SetActive(false);
    }
}
