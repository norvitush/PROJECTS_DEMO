
using System;
using System.Collections.Generic;
using UnityEngine;


namespace VOrb
{
    [Serializable]
    public class NamedPoolList
    {
        [SerializeField] private GameObject _baseObject;
        private Transform _parent;
        private int _amount;
        public string ListName = "";
        public List<GameObject> Pool = new List<GameObject>();
        public int Count
        {
            get
            {
                int noneActive = 0;
                for (int i = 0; i < _amount; i++)
                {
                    if (!Pool[i].activeInHierarchy)
                    {
                        noneActive++;
                    }
                }
                return noneActive;

            }
        }

        public NamedPoolList(string listName, GameObject baseObj, int amountToPool, Transform parentToPool)
        {
            ListName = listName;
            _baseObject = baseObj;
            _parent = (parentToPool != null) ? parentToPool: baseObj.transform ;
            _amount = amountToPool;            
        }
        public void Init()
        {
            if (_baseObject == null)
            {
                return;
            }
            for (int i = 0; i < _amount; i++)
            {
                GameObject tmp;
                if (_parent != null)
                {
                    tmp = UnityEngine.Object.Instantiate(_baseObject, _parent);
                }
                else
                {
                    tmp = UnityEngine.Object.Instantiate(_baseObject);
                }
                tmp.SetActive(false);
                Pool.Add(tmp);
            }
        }
        public GameObject GetObject()
        {
            for (int i = 0; i < _amount; i++)
            {
                if (!Pool[i].activeInHierarchy)
                {

                    return Pool[i];
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
        */

}