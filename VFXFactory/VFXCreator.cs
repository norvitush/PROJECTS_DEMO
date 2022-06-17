using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VOrb.CubesWar.VFX.Modules
{

    public abstract class VFXCreator
    {
        protected MonoBehaviour _coroutineContainer; //бихейвор на котором пускаютс€ корутины активации эффектов
        protected Dictionary<EffectsType, GameObject> _effects; //об№екты эффектов с соответствием по типу

        public VFXCreator(MonoBehaviour coroutineContainer, Dictionary<EffectsType, GameObject> effects)
        {
            _coroutineContainer = coroutineContainer;
            _effects = effects;
        }

        public abstract Effect CreateEffect(MonoBehaviour parent, EffectsType type, float timeout);
    }
      
}