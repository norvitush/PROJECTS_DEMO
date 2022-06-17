using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VOrb.CubesWar.VFX.Modules
{
    public delegate Effect EffectAction<T, F>(T gameObjectBehaviour, F time);
    public class BattleVFXCreator : VFXCreator
    {
        
        class EffectHandle
        {
            private EffectAction<MonoBehaviour, float> _action;

            public EffectHandle(EffectAction<MonoBehaviour, float> action)
            {
                _action = action;
            }
            public Effect Invoke(MonoBehaviour obj, float t)
            {
                return _action.Invoke(obj, t);
            }
        }

        private EffectHandle[] _ActionHandles;
        public BattleVFXCreator(MonoBehaviour coroutineContainer, Dictionary<EffectsType,GameObject> effects) : base(coroutineContainer,effects)
        {
            _ActionHandles = new EffectHandle[]{
            new EffectHandle(HeroHit),
            new EffectHandle(BossHit),
            new EffectHandle(Lightning),
            new EffectHandle(CubeMerge),
            new EffectHandle(HeroPlaceShield),
            new EffectHandle(BossCollectPowerAndAttack),
            new EffectHandle(HeroPlaceShieldDestruction)
            };

        }
        public override Effect CreateEffect(MonoBehaviour parent, EffectsType type, float timeout)
        {
            // parent ??=_coroutineContainer    old Unity не воспринимает упрощенный синтаксис
            parent = (parent != null) ? parent : _coroutineContainer; 
            return _ActionHandles[(int)type].Invoke(parent, timeout);
        }


        private Effect HeroHit(MonoBehaviour parent, float timeout)
        {
            GameObject prototype = _effects[EffectsType.HeroHit];
            if (prototype != null)
            {
                return new Effect(prototype)
                .DelayedStart(timeout, parent, () =>
                    {
                        //Hero animate damage
                    })
				.StartForTimeAfterDelay(timeout, 1.2f, parent, () =>
                {
                    //Hero apply demage
                });
        }
            else
                return null;
        }

        private Effect HeroPlaceShield(MonoBehaviour parent, float timeout)
        {
            
            GameObject prototype = _effects[EffectsType.HeroPlaceShield];
            if (prototype != null)
            {
                return new Effect(prototype).StartForTime(timeout, parent);
            }
            else
                return null;
        }
        private Effect HeroPlaceShieldDestruction(MonoBehaviour parent, float timeout)
        {
            GameObject prototype = _effects[EffectsType.HeroPlaceShieldDestruction];
            if (prototype != null)
            {
                return new Effect(prototype).StartForTimeAfterDelay(timeout,2f, parent);
            }
            else
                return null;
        }
        private Effect BossCollectPowerAndAttack(MonoBehaviour parent, float timeout)
        {
            GameObject prototype = _effects[EffectsType.BossCollectPowerAndAttack];

            if (prototype != null)
            {
                return new Effect(prototype).StartForTimeAfterDelay(0.2f,timeout, parent);
            }
            else
                return null;            
        }

       
        private Effect CubeMerge(MonoBehaviour parent, float timeout)
        {
            GameObject prototype = _effects[EffectsType.CubeMerge];

            if (prototype != null)
            {
                return new Effect(prototype).Start().DestroyAfter(timeout);                
            }
            else
                return null;
        }

  

        private  Effect Lightning(MonoBehaviour parent, float timeout)
        {            
            GameObject prototype = _effects[EffectsType.Lightning];
            if (prototype != null)
            {
                return new Effect(prototype.gameObject).Start().DestroyAfter(2f);                 
            }
            else
                return null;
        }

        private  Effect BossHit(MonoBehaviour parent, float timeout )
        {
            GameObject prototype = _effects[EffectsType.BossHit];

            if (prototype != null)
            {
                prototype.transform.position = parent.transform.position + Vector3.up * 3;

                return new Effect(prototype).DelayedStart(timeout, parent).DestroyAfter(timeout + 1.5f);

            }
            else
                return null;                        
        }

    }

}