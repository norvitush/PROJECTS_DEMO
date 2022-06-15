using UnityEngine;

namespace VOrb
{
    public abstract class Singlton<InhClass> : MonoBehaviour where InhClass : Singlton<InhClass>
    {
        public static InhClass Instance = null;
        private void Awake()
        {
            if (Instance == null)       
            {
                    if (Instance == null)       
                    {
                        Instance = (InhClass)this;
                        Init();
                    }
            }
            else  
            {
                Destroy(gameObject);
            }
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

