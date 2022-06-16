using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using VOrb;

public class UIWindowsManager : Singlton<UIWindowsManager>
{
    

    public List<Window> Windows;


    protected override void Init()
    {
        base.Init();
        foreach (Window window in Windows)
        {
            if (window.gameObject.activeInHierarchy)
                window.gameObject.SetActive(false);
        }

    }
 

    void Start()
    {
        MainWindow selected = null;
            foreach (Window window in Windows)
            {
                if (window is MainWindow)
                {                   
                    selected = (MainWindow)window;
                }
                else
                    window.Close();
            }        
        selected.loadCallback = selected.OpenStartWindow;
        selected.Open(delegate { });
    }

    public static T GetWindow<T>() where T : Window => Instance.Windows.OfType<T>().FirstOrDefault();
    
}

