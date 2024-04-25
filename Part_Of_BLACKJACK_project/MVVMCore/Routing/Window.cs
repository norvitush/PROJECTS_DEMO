using UnityEngine;
using System;
using GoldenSoft.UI.MVVM;

/// <summary>
/// Каждое окно может иметь одного потомка, ссылка на него  в CurrentWindow
/// если открывают другое дочернее, предыдущее закрывается (и все его потомки).
/// 
/// Each window can have one child, a reference to it in the Current Window
/// if another child is opened, the previous one is closed (and all its childs).
/// </summary>
public abstract class Window : MonoBehaviour
{
    public bool IsOpen { get; private set; } = false;
    public Window ChildWindow { get; protected set; } = null; //ссылка на дочернее окно, если открыто 
    public Window ParentWindow { get; protected set; } = null; //ссылка на родительское окно, если есть 

    public abstract void Init(IViewModel viewModel);
    public abstract void SetContentVisible( bool val);
    
    public event Action OnDestroyWindow;
    public void ChangeParentWindow(Window newParent) => ParentWindow = newParent;

    //link to the child window, if open
    public void Open(Action<Window> ParentChangeCurWindow)
    {
        IsOpen = true; 
        ParentChangeCurWindow?.Invoke(this); //записываемся к родителю в ChildeWindow / sign up for a parent
       
        SelfOpen();

    }
    public void Close()
    {
        IsOpen = false;

        if (ChildWindow != null)
            ChildWindow.Close();

        //OnDestroyWindow?.Invoke();
        
        SelfClose();
    }
    public void CloseChildren()
    {
        ChildWindow.Close();
    }

    private void OnDestroy()
    {
        OnDestroyWindow?.Invoke();
    }
    protected abstract void SelfOpen();   //each heir opens as want
    protected abstract void SelfClose();  //each heir closes as want

    protected void ChangeChildWindow(Window sender)
    {
        if (ChildWindow != null)   //Если есть дочернее окно закрываем его
            ChildWindow.Close();   //If there is a child window close it

        ChildWindow = sender;      //запоминаем новое дочернее окно //storing the new child window
        
        if (sender == null) return;

        sender.ChangeParentWindow(this);
    }                                


}
