using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using VOrb;
using VOrb.CubesWar;
using UnityEngine.EventSystems;

/// <summary>
/// Каждое окно может иметь одного потомка, ссылка на него  в CurrentWindow
/// если открывают другое дочернее, предыдущее закрывается (и все его потомки).
/// 
/// Each window can have one child, a reference to it in the Current Window
/// if another child is opened, the previous one is closed (and all its childs).
/// </summary>
public abstract class Window : MonoBehaviour, ITouchSensetive//IngameContainer, ITouchSensetive
{
    public bool IsOpen { get; private set; } = false;
    public Window CurrentWindow { get; protected set; } = null; //ссылка на дочернее окно, если открыто 
    public UnityEvent TapEvent { get; set; } = new UnityEvent();
    public UnityEvent SvipeEvent { get; set; } = new UnityEvent();
    public TouchEventResponse[] responses { get; set; }

    //link to the child window, if open
    public void Open(Action<Window> ParentChangeCurWindow)
    {
        IsOpen = true; 
        ParentChangeCurWindow?.Invoke(this); //записываемся к родителю в CurrentWindow / sign up for a parent
        
        responses = new TouchEventResponse[] {
                new TouchEventResponse(DownTouch),
                new TouchEventResponse(UpTouch),
                new TouchEventResponse(DragTouch),
                new TouchEventResponse(DropTouch)
            };

        SelfOpen();

    }
    public void Close()
    {
        IsOpen = false;

        if (CurrentWindow != null)
            CurrentWindow.Close();
        SelfClose();
    }

    protected abstract void SelfOpen();   //each heir opens as want
    protected abstract void SelfClose();  //each heir closes as want

    protected void ChangeCurrentWindow(Window sender)
    {
        if (CurrentWindow != null)   //Если есть дочернее окно закрываем его
            CurrentWindow.Close();   //If there is a child window close it

        CurrentWindow = sender;      //запоминаем новое дочернее окно
    }                                //storing the new child window

    public virtual void InvokeTouchEvent(TouchEvent updateType, Swipe data)
    {
        responses[(int)updateType].Invoke(data);
    }

    public virtual void DropTouch(Swipe data) { }
    public virtual void DownTouch(Swipe data) { }
    public virtual void UpTouch(Swipe data) { }
    public virtual void DragTouch(Swipe data) { }
}
