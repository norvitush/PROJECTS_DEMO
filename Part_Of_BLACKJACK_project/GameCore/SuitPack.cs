using System;

namespace GoldenSoft.Column21.Core
{
    public struct SuitPack
    {
        private SuitType _suit;
        private CardsSetBitData _cards;

        public SuitPack(SuitType suit, CardsSetBitData cards)
        {
            _suit = suit;
            _cards = cards;
        }

        public SuitType Suit => _suit;
        public CardsSetBitData Cards => _cards;
    }

    public struct CardLiteral
    {
        public char[] Literal;

        public CardLiteral(char suit, char card)
        {
            Literal = new char[2];
            Literal[1] = suit.ToString().ToLower()[0];
            Literal[0] = card.ToString().ToUpper()[0];
        }
        public CardLiteral(Card card)
        {
            Literal = new char[2];
            Literal[1] = card.SuitType.ToSuitChar();
            Literal[0] = card.CardType.ToCardChar();
        }

        public string Value => new String(Literal);
        public CardType GetCardType() => Literal[0].ToCardType();
        public SuitType GetSuitType() => Literal[1].ToSuitType();
        public Card AsCard => new Card( GetSuitType(), GetCardType());
        

        public override string ToString()
        {
            return Value;
        }
    }

}
