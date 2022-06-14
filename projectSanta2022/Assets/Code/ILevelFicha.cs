using System;
using System.Collections.Generic;
using UnityEngine;

namespace VOrb.CubesWar.Levels
{
    public interface ILevelFicha 
    {
        public int minLevelForShow { get; }
    }

    [Serializable]
    public class SmokedChimney : ILevelFicha
    {
        [SerializeField] private int BASE_LEVEL = 4;
        public int minLevelForShow => BASE_LEVEL;
        public SmokedChimney(int minLvl) { BASE_LEVEL = minLvl; }
        public SmokedChimney() {  }
    }
    
    [Serializable]
    public class LightHouseChimney : ILevelFicha
    {
        [SerializeField] private int BASE_LEVEL = 0;
        public int minLevelForShow => BASE_LEVEL;
        public LightHouseChimney(int minLvl) { BASE_LEVEL = minLvl; }
        public LightHouseChimney() { }
    }
    [Serializable]
    public class NumberedHouseChimney : ILevelFicha
    {
        [SerializeField] private int BASE_LEVEL = 7;
        public int minLevelForShow => BASE_LEVEL;
        public NumberedHouseChimney(int minLvl) { BASE_LEVEL = minLvl; }
        public NumberedHouseChimney() { }

    }

    
}