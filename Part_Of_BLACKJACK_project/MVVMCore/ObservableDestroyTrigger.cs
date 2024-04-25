using GoldenSoft.UI.MVVM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ObservableDestroyTrigger : MonoBehaviour
{
    bool calledDestroy = false;

    CompositeDisposable disposablesOnDestroy;

    public bool IsActivated { get; private set; }

    /// <summary>
    /// Check called OnDestroy.
    /// This property does not guarantees GameObject was destroyed,
    /// when gameObject is deactive, does not raise OnDestroy.
    /// </summary>
    public bool IsCalledOnDestroy { get { return calledDestroy; } }

    void Awake()
    {
        IsActivated = true;
    }

    /// <summary>This function is called when the MonoBehaviour will be destroyed.</summary>
    void OnDestroy()
    {
        if (!calledDestroy)
        {
            calledDestroy = true;
            if (disposablesOnDestroy != null) disposablesOnDestroy.Dispose();
        }
    }


    /// <summary>Invoke OnDestroy, this method is used on internal.</summary>
    public void ForceRaiseOnDestroy()
    {
        OnDestroy();
    }

    public void AddDisposableOnDestroy(IDisposable disposable)
    {
        if (calledDestroy)
        {
            disposable.Dispose();
            return;
        }

        if (disposablesOnDestroy == null) disposablesOnDestroy = new CompositeDisposable();
        disposablesOnDestroy.Add(disposable);

    }
}
