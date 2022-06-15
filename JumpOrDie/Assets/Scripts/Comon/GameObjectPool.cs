using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VOrb
{
    public enum PooledObjectType
    {
        TapMessage = 1,
        FlowMessage = 2,
        FlowUpNumbers = 3
    }

    public class GameObjectPool: Singlton<GameObjectPool>
    {     
     
        public List<NamedPoolList> AllPools = new List<NamedPoolList>();
        [Header("Функция для продолжения загрузки")]public UnityEvent readyCallback;


        void Start()
        {
            foreach (var pool in AllPools)
            {
                pool.Init();
            }

            readyCallback?.Invoke();
        }
        public GameObject GetPooledObject(PooledObjectType num)
        {
            if ((int)num>AllPools.Count)
            {
                return null;
            }
            return AllPools[(int)num-1].GetObject();
            
        }
        public static int GetPoolCount(PooledObjectType num)
        {
            return Instance.AllPools[(int)num-1].Count;
        }
        public static void AddToPool(GameObject obj, PooledObjectType type)
        {
            if (obj != null)
            {
                obj.SetActive(false);
                Instance.AllPools[(int)type-1].Pool.Add(obj);
            }
        }
        public static IEnumerable<GameObject> GetActivePool(PooledObjectType type)
        {
            List<GameObject> activ = new List<GameObject>();
            int Active = 0;
            for (int i = 0; i < Instance.AllPools[(int)type-1].Pool.Count; i++)
            {
                if (Instance.AllPools[(int)type-1].Pool[i].activeInHierarchy)
                {
                    Active++;
                    activ.Add(Instance.AllPools[(int)type-1].Pool[i]);
                }
            }
            return activ;
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
        */

}