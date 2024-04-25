using System;

namespace GoldenSoft.Column21.Core
{
    public class Deck
    {
        /// <summary>
        /// Index is SuitType, Value is CardsSet wraped to CardsSetBitData
        /// </summary>
        CardsSetBitData[] _suits = new CardsSetBitData[4];

        public Deck(SuitPack[] packs)
        {

            for (int i = 0; i < _suits.Length; i++)
                _suits[i] = null;

            int cnt = packs.Length;
            for (int i = 0; i < cnt; i++)
            {
                SetSuitData(packs[i].Suit, packs[i].Cards);
            }

        }
        private void SetSuitData(SuitType type, CardsSetBitData val)
        {
            _suits[(int)type] = val;
        }

        public CardsSetBitData GetSuitData(SuitType suitType)
        {

            return _suits[(int)suitType];
        }

    }

}
