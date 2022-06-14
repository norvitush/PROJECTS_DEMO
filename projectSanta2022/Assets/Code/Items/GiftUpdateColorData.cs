using UnityEngine;

namespace VOrb.CubesWar
{
    public struct GiftUpdateColorData
    {
        public bool update;
        public Color32 value;
        public static GiftUpdateColorData None => new GiftUpdateColorData(false, Color.white);
        public GiftUpdateColorData(bool update, Color32 value)
        {
            this.update = update;
            this.value = value;
        }
    }

}