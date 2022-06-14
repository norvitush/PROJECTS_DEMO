using UnityEngine;


namespace VOrb.CubesWar
{
    public struct PooledObject
    {
        private GameObject _gameObject;
        private PooledObjectType _selfType;

        public PooledObject(GameObject gameObject, PooledObjectType selfType)
        {
            _gameObject = gameObject;
            _selfType = selfType;
        }

        public GameObject GameObject { get => _gameObject; }
        public PooledObjectType SelfType { get => _selfType; }
    }
    


}