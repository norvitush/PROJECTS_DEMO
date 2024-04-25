using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoldenSoft.Column21.Core
{
    public static class  DeckHelper
    {
        public const int SUIT_CARDS_COUNT = 13;
        public const int FULL_CARDS_INT = 8191;
        public const string CARDS_LITERALS = "A23456789TJQK";
        public const string SUIT_LITERALS = "HCDS";
        
        public static char ToSuitChar(this SuitType type)
        {
            if (((int)type) > SUIT_LITERALS.Length - 1) return ' ';

            return SUIT_LITERALS[(int)type];
        }
        public static char ToCardChar(this CardType type)
        {
            if (((int)type) > CARDS_LITERALS.Length - 1) return ' ';

            return CARDS_LITERALS[(int)type];
        }
        public static SuitType ToSuitType(this char suit)
        {
            string suitStr = suit.ToString().ToUpper();

            if (SUIT_LITERALS.Contains(suitStr))
            {
                return (SuitType)SUIT_LITERALS.IndexOf(suitStr[0]);
            }
            else
                return SuitType.None;

        }
        public static CardType ToCardType(this char card)
        {
            string cardStr = card.ToString().ToUpper();

            if (CARDS_LITERALS.Contains(cardStr))
            {
                return (CardType)CARDS_LITERALS.IndexOf(cardStr[0]);
            }
            else
                return CardType.None;

        }

        public static CardsSet AllCardsTypes => (CardsSet)FULL_CARDS_INT;

        public static CardType FirstCard(this CardsSet cardsSet)
        {
            var values = (uint[])Enum.GetValues(typeof(CardsSet));

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == (int)CardsSet.None) continue;

                if ((cardsSet & (CardsSet)values[i]) == (CardsSet)values[i]) 
                {
                    return (CardType)(i-1);
                }
            }

            return CardType.None;
        }

        public static CardsSet AsCardSet(this CardType card)
        {

            return (CardsSet)(int)(Mathf.Pow(2, (int)card));
        }
            
        
        public static int GetOrderNumber(CardsSet type, SuitType suit)
        {
            
            if (type == CardsSet.None) type = CardsSet.Two;

            int baseNumber = SUIT_CARDS_COUNT * (int)suit;

            var values = (uint[])Enum.GetValues(typeof(CardsSet));

            int cnt = values.Length;

            for (int i = 0; i < cnt; i++)
            {
                if (values[i] != (int)type) continue;

                return baseNumber + i;
            }

            return 0;
        }

        public static (SuitType cardSuit, CardsSet cardType) GetCardTuplByNumber(uint value)
        {

            int innerVal = (int)value - 1;
            int suitNumber = Mathf.FloorToInt(innerVal / SUIT_CARDS_COUNT);

            int cardTypeNumber = (int)Mathf.Pow(2, (innerVal - suitNumber * SUIT_CARDS_COUNT));

            return ((SuitType)suitNumber, (CardsSet) cardTypeNumber);
        }

        public static Card GetCardByNumber(uint value)
        {
            var tupl = GetCardTuplByNumber(value);

            return new Card(tupl.cardSuit, tupl.cardType.FirstCard());
        }

        public static Deck GetFullDeck()
        {
            CardsSetBitData cardsBitData = new CardsSetBitData();

            cardsBitData.Set(DeckHelper.FULL_CARDS_INT);

            var suitValues = (uint[])Enum.GetValues(typeof(SuitType));
            SuitPack[] suitPacks = new SuitPack[suitValues.Length];
            int cnt = 0;

            foreach (uint suit in suitValues)
            {
                SuitType suitType = (SuitType)suit;
                suitPacks[cnt++] = new SuitPack(suitType, cardsBitData);
            }

            return new Deck(suitPacks);
        }

        public static LinkedList<int> GetRandomCardsNumbersSequence()
        {
            LinkedList<int> CardsSquence = new LinkedList<int>();
            HashSet<int> allCards = new HashSet<int>();

            for (int i = 1; i <= SUIT_CARDS_COUNT*4; i++) allCards.Add(i);

            for (int i = 0; i < SUIT_CARDS_COUNT * 4; i++)
            {
                int card = allCards.ElementAt(UnityEngine.Random.Range(0, allCards.Count));
                
                allCards.Remove(card);

                CardsSquence.AddLast(card);
            }

            return CardsSquence;
        }

        public static LinkedList<Card> GetRandomCardsSequence()
        {
            LinkedList<Card> CardsSquence = new LinkedList<Card>();
            HashSet<uint> allCards = new HashSet<uint>();

            for (uint i = 1; i <= SUIT_CARDS_COUNT * 4; i++) allCards.Add(i);

            for (uint i = 0; i < SUIT_CARDS_COUNT * 4; i++)
            {
                uint card = allCards.ElementAt(UnityEngine.Random.Range(0, allCards.Count));

                allCards.Remove(card);

                CardsSquence.AddLast(GetCardByNumber(card));
            }

            return CardsSquence;
        }
    }

}
