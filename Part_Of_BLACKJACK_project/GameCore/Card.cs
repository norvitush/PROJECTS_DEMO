using UnityEngine;

namespace GoldenSoft.Column21.Core
{
    [System.Serializable]
    public struct Card
    {
        [SerializeField] private SuitType _suit;
        [SerializeField] private CardType _cardType;
        
        public SuitType SuitType => _suit;
        public CardType CardType => _cardType;
                
        public Card(SuitType suit, CardType cardType)
        {
            _suit = suit;
            _cardType = cardType;
        }
        public Card(char suit, char cardType)
        {
            CardLiteral cardLiteral = new CardLiteral(suit, cardType);
            var card = cardLiteral.AsCard;
            _suit = card.SuitType;
            _cardType = card.CardType;
        }

        public static Card Zero => new Card(SuitType.None, CardType.None);

        public override bool Equals(object obj)
        {
            if (!(obj is Card)) return false;
            Card objCard = (Card)obj;
            return (this.SuitType == objCard.SuitType && this.CardType ==  objCard.CardType) 
                    || (this.CardType == CardType.None && objCard.CardType == CardType.None);
        }

        public static bool operator != (Card card1, Card card2)
        {
            return !card1.Equals(card2);
        }

        public static bool operator == (Card card1, Card card2)
        {
            return card1.Equals(card2);
        }
        public override string ToString()
        {
            return $"{_cardType} of {_suit}";
         
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }


}
