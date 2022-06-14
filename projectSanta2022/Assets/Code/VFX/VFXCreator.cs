using System.Collections;
using UnityEngine;

namespace VOrb.CubesWar.VFX.Modules
{

    public enum EffectType
    {
        Snowfall = 0,
        GiftPoof = 1,
        StarsPoof = 2
    }

    public delegate Effect EffectAction(GameObject gameObject, float time);

    public abstract class VFXCreator
    {

        protected MonoBehaviour _coroutineContainer;

        public VFXCreator(MonoBehaviour coroutineContainer)
        {
            _coroutineContainer = coroutineContainer;
        }

        public abstract Effect CreateEffect(GameObject parent, EffectType type, float timeout);
    }
   
       

}