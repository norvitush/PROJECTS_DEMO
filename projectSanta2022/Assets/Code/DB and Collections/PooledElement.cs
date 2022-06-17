using UnityEngine;


namespace VOrb.SantaJam
{
    public class PooledElement
        {
            private bool _wasAsked;
            public GameObject _pooledObject;

            public bool WasAsked { get => _wasAsked; }

            public bool Avalible() { return (!_wasAsked && !_pooledObject.activeInHierarchy); }
            public void ReturnToPool()
            {
                _wasAsked = false;
                _pooledObject.SetActive(false);
            }
            public PooledElement(GameObject pooled)
            {
                _wasAsked = false;
                _pooledObject = pooled;
            }

            public GameObject GetObject()
            {
                return _pooledObject;
            }
            public GameObject UseObject()
            {

                if (Avalible())
                {
                    _wasAsked = true;
                    return _pooledObject;
                }
                else
                    return null;
            }
        }
    


}