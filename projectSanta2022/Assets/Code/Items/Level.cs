using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VOrb.SantaJam.Levels
{
    [Serializable]
    public class Level
    {
        [SerializeField] private float _speed;
        [SerializeField] private int _levelNumber;
        [SerializeField] private int _giftscount;
        [SerializeField] protected List<string> _fiches = new List<string>();

        public List<string> Fiches { get => _fiches; }

        public int LevelNumber { get => _levelNumber; }
        public float Speed { get => _speed; }
        public int Giftscount { get => _giftscount; set => _giftscount = value; }

        public Level( float speed, int levelNumber, int giftscount)
        {
            _speed = speed;
            _levelNumber = levelNumber;
            _giftscount = giftscount;
        }
    }
    [Serializable]
    public class LevelInfo : Level
    {
        private List<ILevelFicha> _Ifiches;
        private List<int> _houseNumbers = new List<int>();
        [SerializeField] private int _fichesCount;

        public int FichesCount { get => _fichesCount;}

        public LevelInfo(List<ILevelFicha> fiches, float speed, int levelNumber, int giftscount) : base(speed, levelNumber, giftscount)
        {
            this._Ifiches = fiches;
            _Ifiches.Clear();
            foreach (var f in fiches)
            {
                _fiches.Add(Parse(f));
            }
        }
        private string Parse(ILevelFicha ficha)
        {
            string output = "";

            if (ficha is NumberedHouseChimney)
            {
                output = Enum.GetName(typeof(ChimneyState), ChimneyState.Numered);
            }
            if (ficha is LightHouseChimney)
            {
                output = Enum.GetName(typeof(ChimneyState), ChimneyState.LightOn);
            }
            if (ficha is SmokedChimney)
            {
                output = Enum.GetName(typeof(ChimneyState), ChimneyState.Smoked);
            }
            return output;
        }
        public List<ILevelFicha> GetFiches() => _Ifiches;
        public List<int> GetHousesNumber() => _houseNumbers.Distinct().ToList();
        public void AddFich(ILevelFicha ficha)
        {
            _fiches.Add(Parse(ficha));
            _Ifiches.Add(ficha);
            _fichesCount = _Ifiches.Count;

        }
        public void GenerateNumbersList(int count)
        {
            _houseNumbers.Clear();
            for (int i = 0; i < count; i++)
            {
                _houseNumbers.Add(UnityEngine.Random.Range(1, 20));
            }

        }

        public bool Contains<T>() where T : ILevelFicha 
        {
            bool inLevel = false;
            foreach (var fich in _Ifiches)
            {
                if (fich is T)
                {
                    inLevel = true;
                }
            }
            return inLevel;
        }

    }

    

}