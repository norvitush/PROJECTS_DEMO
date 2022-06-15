using System;
using UnityEngine;


namespace VOrb
{
    public struct SafeSkinData : IComparable
    {
        private string prefab;   
        public string PrefabName
        {
            get
            {
                return DataKeeper.UncryptStr(prefab);
            }
        }
        public SafeInt PlaySkin_Id;
        public SafeInt MaxParts;
        public SafeInt CollectedParts;
        public SafeInt Purchased;
        public SafeInt Enabled;
        public SafeInt rareType;
        public Color matColor;

        public SafeSkinData(string pref, int skin_id, int maxPart, int collected, Color32 clr, int IsPurchased = 0, int typeofRare = (int)SkinType.basic)
        {
            prefab = DataKeeper.SaveCryptedTo("", pref, false);
            PlaySkin_Id = skin_id;
            MaxParts = maxPart;
            CollectedParts = collected;
            Purchased = IsPurchased;
           
            rareType = typeofRare;
            matColor = clr;
            if (typeofRare == (int)SkinType.basic)
            {
                Enabled = 1;
            }
            else
                Enabled = (collected>=maxPart)?1:0;
        }

        public override string ToString()
        {
            return "("+ Enum.GetName(typeof(SkinType),(int)rareType)+"-"+DataKeeper.UncryptStr(prefab) + ")skin_id: " + 
                   PlaySkin_Id.ToString() + " MaxParts: " + MaxParts.ToString() + "   collected: " +
                   CollectedParts.ToString() + " isPurchased -" + Purchased.ToString();
        }

        public int CompareTo(object obj)
        {
            SafeSkinData skn = (SafeSkinData)obj;
            if (skn.PlaySkin_Id != 0)
            {
                int ThisInt = this.PlaySkin_Id;
                return ThisInt.CompareTo((int)skn.PlaySkin_Id);
            }
            else
                throw new Exception("Невозможно сравнить два объекта");
        }

        public static implicit operator SafeSkinData( SkinView simple)
        {
            return new SafeSkinData (simple.PrefabName, simple.Id,simple.FullSkinParts, 0,simple.MatColor,  0, (int)simple.RareType);
        }

        public static implicit operator SkinView(SafeSkinData simple)
        {
            SkinView tmp = new SkinView();
            tmp.PrefabName = simple.PrefabName;
            tmp.Id = simple.PlaySkin_Id;            
            tmp.FullSkinParts = simple.MaxParts;
            tmp.MatColor = simple.matColor;
            tmp.RareType = (SkinType)(int)simple.rareType;
            return tmp;
        }


    }

}
