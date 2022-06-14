
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


namespace VOrb.CubesWar
{
    public partial class ObjectPoolManager: Singlton<ObjectPoolManager>
    {
        public List<ObjectPool> AllPools = new List<ObjectPool>();
        
        [Header("Функция для продолжения загрузки")]
        [SerializeField] public UnityEvent funcForInvoke;


        void Start()
        {
            foreach (var pool in AllPools)
            {
                pool.Init();
            }

            funcForInvoke?.Invoke();
        }
        public static void ReturnToPool(GameObject poolledObj, PooledObjectType type)
        {
            Instance.GetPull(type).ReturnToPool(poolledObj);
        }

        public PooledObject GetPooledObject(PooledObjectType type)
        {
            var obj = GetPull(type).GetObject();            
            if (obj == null)
            {
                return new PooledObject(Instance.GetPull(type).AddNew(),type);
            }
            else
                return new PooledObject(obj,type);
        }
        public GameObject GetPooledGameObject(PooledObjectType type)
        {
            var obj = GetPull(type).GetObject();
            if (obj == null)
            {
                return Instance.GetPull(type).AddNew();
            }
            else
                return obj;
        }

        public GameObject GetPooledGameObject(PooledObjectType type, Transform parent)
        {
            var parented = GetPull(type).GetObject();
            if (parented == null)
            {
                parented = Instance.GetPull(type).AddNew();
                parented.transform.SetParent(parent);
                return parented;
            }
            else
            {
                parented.transform.SetParent(parent);
                return parented;
            }
                
        }
        public PooledObject GetPooledObject(PooledObjectType type, Transform parent)
        {
            var parented = GetPull(type).GetObject();
            if (parented == null)
            {
                parented = Instance.GetPull(type).AddNew();
                parented.transform.SetParent(parent);

                return new PooledObject(parented,type);
            }
            else
            {
                parented.transform.SetParent(parent);
                return new PooledObject( parented,type);
            }

        }

        public GameObject[] GetPooledGameObjects(PooledObjectType type, int count)
        {
            return GetPull(type).GetObjects(count);
        }
        

        public static void AddToPool(GameObject obj, PooledObjectType type)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Instance.GetPull(type).pool.Add(new PooledElement(obj));
            }
        }
        public static IEnumerable<GameObject> GetActivePool(PooledObjectType type)
        {
            List<GameObject> activ = new List<GameObject>();
            ObjectPool current = Instance.GetPull(type);

            activ = current.pool.Where(ob => ob.GetObject().activeInHierarchy).Select(ob=> ob.GetObject()).ToList();

            return activ;
        }
        public static IEnumerable<PooledObject> GetActivePoolWithType(PooledObjectType type, bool withAsked = false)
        {
            List<PooledObject> activ = new List<PooledObject>();
            ObjectPool current = Instance.GetPull(type);
            if (withAsked)
            {
                activ = current.pool.Where(ob => ob.GetObject().activeInHierarchy || ob.WasAsked).Select(ob => new PooledObject(ob.GetObject(), type)).ToList();
            }
            else
                activ = current.pool.Where(ob => ob.GetObject().activeInHierarchy).Select(ob => new PooledObject(ob.GetObject(),type)).ToList();

            return activ;
        }


        public static void DeactivatePool(PooledObjectType type)
        { 
            foreach (var obj in Instance.GetPull(type).pool)
            {
                obj.ReturnToPool();
            } 
        }
       private ObjectPool GetPull(PooledObjectType type)
        {
            foreach (var pool in AllPools)
            {
                if (pool.ListName == Enum.GetName(typeof(PooledObjectType), type))
                {
                    return pool;
                }
            }
            return null;
        }
    }

    /* Пример использования
     * 
    * GameObject bullet = ObjectPool.SharedInstance.GetPooledObject();
    * if (bullet != null) 
     * { 
     * bullet.transform.position = turret.transform.position;
     * bullet.transform.rotation = turret.transform.rotation;
     * bullet.SetActive(true); 
     * }
     * 
     * 
     * 
        */

}