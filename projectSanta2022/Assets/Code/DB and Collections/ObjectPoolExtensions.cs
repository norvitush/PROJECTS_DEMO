using UnityEngine;


namespace VOrb.SantaJam
{
    public static class ObjectPoolExtensions
    {
        public static void ReleaseToPool(this GameObject poolledObj, PooledObjectType type)
        {
            ObjectPoolManager.ReturnToPool(poolledObj, type);
        }

        public static void ReleaseToPool(this PooledObject poolledObj)
        {
            ObjectPoolManager.ReturnToPool(poolledObj.GameObject, poolledObj.SelfType);
        }
    }

    
}