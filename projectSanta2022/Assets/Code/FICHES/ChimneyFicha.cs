using UnityEngine;

namespace VOrb.CubesWar.Levels
{
    public class ChimneyFicha : ILevelFicha
    {
        [SerializeField] protected int _baseLevel;
        public int minLevelForShow => _baseLevel;
        public ChimneyFicha(int minLvl) { _baseLevel = minLvl; }
        public ChimneyFicha(){}
    }
}