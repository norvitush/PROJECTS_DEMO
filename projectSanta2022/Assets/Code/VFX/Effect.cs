using System;
using UnityEngine;


namespace VOrb.CubesWar.VFX.Modules
{
    public class Effect
    {
        private GameObject _selfContainer;
        public GameObject EffectBody => _selfContainer;

        public EffectType Type;
        public bool isEnabled
        {
            get
            {
                if (_selfContainer != null)
                {
                    if (_selfContainer.activeInHierarchy)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        public Effect(GameObject effectContainer) { _selfContainer = effectContainer; }
        public Effect Start() 
        {
            if (_selfContainer != null)
            {
                _selfContainer.SetActive(true);               
                return this;
            }
            else
                return null;
        }

        public Effect DelayedStart(float delay, MonoBehaviour coroutineParent)
        {
            if (_selfContainer != null)
            {
                _selfContainer.ActivateAfter(delay, coroutineParent);
                return this;
            }
            else
                return null;
        }
        public Effect DelayedStart(float delay, MonoBehaviour coroutineParent, Action callback)
        {
            if (_selfContainer != null)
            {
                _selfContainer.ActivateAfter(delay, coroutineParent, callback);
                return this;
            }
            else
                return null;
        }

        public Effect DestroyAfter(float delay)
        {
            if (_selfContainer != null)
            {               
                UnityEngine.Object.Destroy(_selfContainer, delay);
                return this;
            }
            else
                return null;
        }

        public Effect StartForTime(float liveTime, MonoBehaviour coroutineParent, Action callback = null)
        {
            
                if (_selfContainer != null)
                {
                    _selfContainer.SetActive(true, liveTime, coroutineParent, callback);                    
                    return this;
                }
                else
                    return null;
            
        }

        public Effect StartForTimeAfterDelay(float delay, float liveTime, MonoBehaviour coroutineParent, Action callback = null)
        {
            if (_selfContainer != null)
            {
                _selfContainer.ActivateAfter(delay, coroutineParent, () =>
                {                   
                    _selfContainer.SetActive(true, liveTime, coroutineParent, callback);
                });                
                return this;
            }
            else
                return null;
           
        }
        
        
    }
       

}