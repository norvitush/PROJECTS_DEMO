using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VOrb.CubesWar.VFX.Modules;

namespace VOrb.CubesWar.VFX
{
    public class VFX_Manager: MonoBehaviour
    {        
        public bool isActiveEffects { get; private set; }
        private VFXCreator _VFXFabric;
        private List<Effect> _runningEffects = new List<Effect>();
        private bool _initialized = false;
        private Coroutine _WaitForEffectCountZero = null;

        [SerializeField] private GameObject _snowFall;
        [SerializeField] private GameObject _giftPoof;
        [SerializeField] private GameObject _starsPoof;


        private void Start()
        {
            _VFXFabric = new GameplayVFXCreator(this);
            UpdateContainerData();
        }

        public void UpdateContainerData()
        {
            if (!_initialized)
            {
                List<bool> Check =  new List<bool>();
                _snowFall = GetComponentInChildren<EmptySnowfall>(true).gameObject;
                _giftPoof = GetComponentInChildren<EmptyPoof>(true).gameObject;
                _starsPoof = GetComponentInChildren<EmptyStarsPoof>(true).gameObject;
                Check.Add(_snowFall != null);
                Check.Add(_giftPoof != null);
                Check.Add(_starsPoof != null);

                _initialized = Check.TrueForAll((ch) => ch);
            }
        }

        public void StartSnow()
        {            
            AddEffect(_VFXFabric.CreateEffect(_snowFall, EffectType.Snowfall, 0));
        }
        public void PoofGift(Vector3 position)
        {
            var effect = _VFXFabric.CreateEffect(_giftPoof, EffectType.GiftPoof, 1f);
            effect.EffectBody.transform.position = position;
            AddEffect(effect);
        }
        public void PoofStars(Vector3 position)
        {
            var effect = _VFXFabric.CreateEffect(_starsPoof, EffectType.StarsPoof, 1f);
            effect.EffectBody.transform.position = position;
            AddEffect(effect);
        }

        private bool AddEffect(Effect effect, bool withControlofEnding =false)
        {
            isActiveEffects = true;
            if (effect != null)
            {
                _runningEffects.Add(effect);
                if (withControlofEnding)
                {
                    EffectsControl();
                }
                
                return true;
            }
            else
                return false;
        }
        private void EffectsControl()
        {
            if (_WaitForEffectCountZero == null && _runningEffects.Where((e) => e.isEnabled).Count() != 0)
            {
                _WaitForEffectCountZero = StartCoroutine(WaitForEffectCountZero());
            }
        }

        private IEnumerator WaitForEffectCountZero()
        {
            yield return new WaitUntil(() => {
                return _runningEffects.Where((e) => e.isEnabled).Count() == 0;
            });

            _runningEffects.Clear();

            _WaitForEffectCountZero = null;
            yield return new WaitForSeconds(0.5f);
            isActiveEffects = false;
        }
    }

}
