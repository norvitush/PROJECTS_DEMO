﻿using UnityEngine;

namespace GoldenSoft
{
    public abstract class Singlton<InhClass> : MonoBehaviour where InhClass : Singlton<InhClass>
    {
        public static InhClass Instance = null;
        public static object _lock = new object();
        protected void Awake()
        {
            if (Instance == null)       
            {
                lock (_lock)
                {
                    if (Instance == null)       //DOUBLE TAP
                    {
                        Instance = (InhClass)this;
                        Init();
                    }
                    else  
                        Destroy(gameObject);
                    
                }
            }
            else 
                Destroy(gameObject);
            
        }
        protected virtual void Init()
        {
            gameObject.name = "GLOB_" + typeof(InhClass).ToString();
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

