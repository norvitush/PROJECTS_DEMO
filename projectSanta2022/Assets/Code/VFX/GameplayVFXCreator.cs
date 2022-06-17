using System;
using UnityEngine;


namespace VOrb.SantaJam.VFX.Modules
{
    

    public class GameplayVFXCreator : VFXCreator
    {
        private VFX[] _effects;

        public GameplayVFXCreator(MonoBehaviour coroutineContainer) : base(coroutineContainer)
        {
            _effects = new VFX[]{
            new VFX(SnowFall),
            new VFX(GiftPoof),
            new VFX(StarsPoof)
            };

        }

        private Effect GiftPoof(GameObject effect, float time)
        {
            return new Effect(effect).StartForTime(time,_coroutineContainer);
        }
        private Effect StarsPoof(GameObject effect, float time)
        {
            return new Effect(effect).StartForTime(time, _coroutineContainer);
        }

        private Effect SnowFall(GameObject effect, float time)
        {
            return new Effect(effect).Start();
        }


        public override Effect CreateEffect(GameObject operand, EffectType type, float timeout)
        {
            return _effects[(int)type].Invoke(operand, timeout);
        }
    }

}