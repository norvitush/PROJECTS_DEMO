using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

namespace VOrb.CubesWar
{

    [System.Serializable]
    public class GiftItem
    {
        public string Name;
        public string Prefab;
        public int Id;
    }

    public class SafeGiftItem: IComparable
    {
        private string _prefabName;
        public string Name;
        public string PrefabName
        {
            get
            {
                return DataKeeper.UncryptStr(_prefabName);
            }
        }
        public SafeInt Id;

        public SafeGiftItem() { }

        public SafeGiftItem(string name, string prefabName, SafeInt id)
        {
            
            Name = name;
            _prefabName = DataKeeper.SaveCryptedTo("", prefabName, false);
            Id = id;
        }
        
        public override string ToString()
        {
            return "(" + DataKeeper.UncryptStr(_prefabName) + ") cube_id: " +
                   Id.ToString() ;
        }

        public static bool operator == (SafeGiftItem f1, SafeGiftItem f2)
        {
            if ((object)f1 is null || (object)f2 is null)
            {
                return (object)f1 == (object)f2;
            }            
            else
            {
                return f1.Id == f2.Id;
            }
            
        }
        public static bool operator != (SafeGiftItem f1, SafeGiftItem f2)
        {
            if ((object)f1 is null || (object)f2 is null)
            {
                return (object)f1 != (object)f2;
            }
            else
            {
                return f1.Id != f2.Id;
            }
        }


        public int CompareTo(object obj)
        {
            SafeGiftItem cube = (SafeGiftItem)obj;
            if (cube.Id != 0)
            {
                int ThisInt = this.Id;
                return ThisInt.CompareTo((int)cube.Id);
            }
            else
                throw new Exception("Невозможно сравнить два объекта");
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SafeGiftItem(GiftItem simple)
        {
            return new SafeGiftItem(simple.Name, simple.Prefab,simple.Id);
        }

        public static implicit operator GiftItem(SafeGiftItem simple)
        {
            
            GiftItem tmp = new GiftItem();
            
            tmp.Name = simple.Name;
            tmp.Prefab = simple.PrefabName;
            tmp.Id = simple.Id;
     
            return tmp;
        }
    }

    


   
}