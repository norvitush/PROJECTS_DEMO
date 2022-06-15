using System;
using System.Collections.Generic;


namespace VOrb
{
    [Serializable]
    public struct PLayerCollectionInfo
    {
        public string PlayerName;        
        public SafeInt coins;
        public SafeInt parts;
        public SafeInt progress;
        public SafeInt LastNotFilled_id;
        public List<SafeSkinData> Skins;
        public PLayerCollectionInfo(string name)
        {
            PlayerName = name;
            coins = 0;
            parts = 0;
            progress = 0;
            LastNotFilled_id = 0;
            Skins = new List<SafeSkinData>();
        }
        public override string ToString()
        {
            string s = "IPlayerName: " + PlayerName + "   Coins: " + coins + "   parts: " + parts + "   Progress: " + progress;
            s += "   LastNotFilled: " + LastNotFilled_id + " SkinsList: ";
            foreach (var skin in Skins)
            {
                s += skin.PlaySkin_Id + "-" + skin.PrefabName + "-" + skin.CollectedParts + "-" + skin.MaxParts ;
                s += ((skin.Enabled == 1)? "e":"u.e" )  + "-";
               s+= ((skin.Purchased==1) ? "p" : "u.p")+", ";
            }
            return s;
        }
    }

}
