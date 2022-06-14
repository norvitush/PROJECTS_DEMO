
using System;
using System.Collections.Generic;
using UnityEngine;


namespace VOrb.CubesWar
{
    [Serializable]
    public class ObjectPool
    {
        [SerializeField] private GameObject baseObject;
        private Transform parent;
        private int amount;
        public string ListName = "";
        public List<PooledElement> pool = new List<PooledElement>();

        public Vector3 basePosition;
        public Quaternion baseRotation;
        public Vector3 baseScale;
        public int count
        {
            get
            {
                int noneActive = 0;
                for (int i = 0; i < amount; i++)
                {
                    if (pool[i].Avalible())
                    {
                        noneActive++;
                    }
                }
                return noneActive;

            }
        }
        public void ReturnToPool(GameObject gameObject)
        {
            var pooled = pool.Find(po => po.GetObject() == gameObject);
            if (pooled!=null)
            {
                pooled.ReturnToPool();
                if (gameObject.transform.parent != parent)
                {
                    gameObject.transform.SetParent(parent);
                }
            }
            
        }
        public ObjectPool(PooledObjectType type, GameObject baseObj, int amountToPool, Transform parentToPool)
        {
            ListName = Enum.GetName(typeof(PooledObjectType), type);
            baseObject = baseObj;
            parent = (parentToPool != null) ? parentToPool: baseObj.transform ;
            amount = amountToPool;
            basePosition = baseObject.transform.position;
            baseRotation = baseObject.transform.rotation;
            baseScale = baseObject.transform.localScale;
        }
        public void Init()
        {
            if (baseObject == null)
            {
                return;
            }
            for (int i = 0; i < amount; i++)
            {
                GameObject tmp;
                if (parent != null)
                {
                    tmp = UnityEngine.Object.Instantiate(baseObject, parent);
                }
                else
                {
                    tmp = UnityEngine.Object.Instantiate(baseObject);
                }
                tmp.SetActive(false);

                pool.Add(new PooledElement(tmp));
            }
        }

        public GameObject AddNew()
        {
            GameObject tmp;
            if (parent != null)
            {
                tmp = UnityEngine.Object.Instantiate(baseObject, parent);
            }
            else
            {
                tmp = UnityEngine.Object.Instantiate(baseObject);
            }
            tmp.SetActive(false);

            pool.Add(new PooledElement(tmp));
            return tmp;
        }
        public GameObject GetObject()
        {            
            for (int i = 0; i < amount; i++)
            {
                if (pool[i].Avalible())
                {
                    GameObject founded = pool[i].UseObject();
                    if (founded != null)
                    {
                        founded.transform.position = basePosition;
                        founded.transform.rotation = baseRotation;
                        founded.transform.localScale = baseScale;
                        return founded;
                    }
                    
                    
                }
            }
            return null;
        }

        public GameObject[] GetObjects(int count)
        {
            if (count>0)
            {
                GameObject[] allneeded = new GameObject[count];
                int founded = 0;

                for (int i = 0; i < amount; i++)
                {
                    if (pool[i].Avalible())
                    {
                        GameObject foundedObj = pool[i].UseObject();
                        if (foundedObj != null)
                        {
                            foundedObj.transform.position = basePosition;
                            foundedObj.transform.rotation = baseRotation;
                            foundedObj.transform.localScale = baseScale;
                            allneeded[founded] = foundedObj;
                            founded++;
                        }

                        
                        if (founded >= count) break;

                    }
                }

                if (founded == count)
                {
                    return allneeded;
                }
                else
                    return null;
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