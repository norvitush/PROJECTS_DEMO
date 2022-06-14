using UnityEngine;


namespace VOrb.CubesWar.VFX.Modules
{
 
        public class VFX
        {
            
            private EffectAction _action;

            public VFX(EffectAction forExecution)
            {
                _action = forExecution;
            }
            public Effect Invoke(GameObject obj, float t)
            {
                return _action.Invoke(obj, t);
            }
        }
    

}