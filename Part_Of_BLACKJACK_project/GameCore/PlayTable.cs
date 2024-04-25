using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoldenSoft.Column21.Core
{
    public struct PlayTable
    {
        private bool _activated;
        private Card[] _deck;
        private Dictionary<int, List<Card>> _columnsData ;
        private Dictionary<int, ColumnTotalValue> _columnTotals;
        private Stack<Card> _hold ;
        private Stack<Card> _deckFive;
        private Card _handPlace;
        private int _maxHoldCards;
        private int _maxLifes;
        private int _score;
        private int _lifes;
        private TimeSpan _timer;
        private int _currentIndex;

        public Card HandCard => _handPlace;
        public TimeSpan Timer { get => _timer; set => _timer = value; }
        public int Score { get => _score; }
        public int Lifes { get => _lifes; }
       
        public int DeckCount
        {
            get
            {
                if (CurrentIndex < 0 || CurrentIndex >= _deck.Length) return 0;

                return _deck.Length - CurrentIndex;
            }
        }
        public int CurrentIndex 
        { 
            get => _currentIndex;
            set 
            {
                UnityEngine.Debug.Log($"set to offset {_currentIndex}");
                _currentIndex = value; 
            } 
        }

        public PlayTable(IEnumerable<Card> deck, Settings settings)
        {
            _deck = deck.ToArray();
            
            _maxLifes = _lifes = settings.PlayerLifes;
            _maxHoldCards = settings.HoldCardsCount;

            _hold = new Stack<Card>();
            _handPlace = Card.Zero;

            _score = 0;
            _timer = TimeSpan.Zero;

            _columnsData = new Dictionary<int, List<Card>>() 
            {
                { 0, new List<Card>()},{ 1, new List<Card>()}, { 2, new List<Card>()}, { 3, new List<Card>()}
            };

            _deckFive = new Stack<Card>();
            _currentIndex = 0;

            _activated = true;
            _columnTotals = new Dictionary<int, ColumnTotalValue>() 
            { 
                { 0, ColumnTotalValue.Zero}, {1,ColumnTotalValue.Zero }, {2, ColumnTotalValue.Zero }, {3, ColumnTotalValue.Zero }
            };

            FillDeckFive();
            
        }
        public void RevertPopLast()
        {
            if (CurrentIndex == 0) return;

            if (CurrentIndex < 0) CurrentIndex = _deck.Length - 1;
            else
                CurrentIndex--;
            
            FillDeckFive();
        }
        public Card PopCardFormDeckFive()
        {
            if (_deckFive.Count == 0) return Card.Zero;

            Card card = _deckFive.Pop();
            
            if (CurrentIndex >=0 ) CurrentIndex++;

            if (CurrentIndex >= _deck.Length) CurrentIndex = -1;
            
            FillDeckFive();
            
            return card;
        }
        public List<Card> PeekDeckFive()
        {
            if (_deckFive.Count < 5)
            {
                FillDeckFive();
            }

            return _deckFive.ToList();
        }
        public int DecreaseLifes(int val)
        {
            _lifes = Mathf.Clamp(_lifes - val, 0, _maxLifes);
            return _lifes;
        }
        public void AddToScore(int val) => _score += val;
        public bool TrySetHandCard(Card card)
        {
            if (_handPlace != Card.Zero || card == Card.Zero) return false;

            _handPlace = card;

            return true;
        }
        public void DropHandCardValue() => _handPlace = Card.Zero;
        public bool TryGetFromHand(out Card card)
        {
            if (_handPlace == Card.Zero) { card = Card.Zero; return false; }

            card = _handPlace;

            _handPlace = Card.Zero;

            return true;
        }
        public Card PeekHold()
        {
            if (_hold.Count == 0) {  return Card.Zero; }

            return _hold.Peek();
        }
        public int FreeHoldSlots()
        {
            return _maxHoldCards - _hold.Count;
        }
        public bool TryPushHold(Card card)
        {
            if (_hold.Count >= _maxHoldCards || card == Card.Zero) return false;

            _hold.Push(card);

            return true;
        }
        public bool TryPopHold( out Card card)
        {
            if (_hold.Count == 0) { card = Card.Zero; return false; }

            card = _hold.Pop();

            return true;
        }
        public bool TryAddToColumn(int columnIndex, Card card) 
        {
            if(_columnsData.TryGetValue(columnIndex, out List<Card> cards))
            {
                cards.Add(card);
                return true;
            }

            return false;
        }
        public bool TryRemoveFromColumn(int columnIndex, Card card)
        {
            if (_columnsData.TryGetValue(columnIndex, out List<Card> cards))
            {
                if(cards.Contains(card))
                {
                    cards.Remove(card);
                    return true;
                }
            }

            return false;
        }
        public void ClearColumn(int columnIndex)
        {
            if (_columnsData.TryGetValue(columnIndex, out List<Card> cards))
            {
                cards.Clear();
            }
        }
        public Card[] GetCardsInColumn(int columnIndex)
        {
            if (_columnsData.TryGetValue(columnIndex, out List<Card> cards))
            {
                return cards.ToArray();
            }

            return null;
        }
        public TableDataSnap GetSnap()
        {
            
            Card[][] columns = new Card[4][] 
            {
               _columnsData[0].ToArray(), _columnsData[1].ToArray(), _columnsData[2].ToArray(), _columnsData[3].ToArray()
            };

            ColumnTotalValue[] totals = new ColumnTotalValue[4]
            {
               _columnTotals[0], _columnTotals[1],_columnTotals[2], _columnTotals[3]
            };

            Card[] deck = _deckFive.ToArray();
            Card hold = Card.Zero;

            if (_hold.Count > 0) hold = _hold.Peek();
            
            
            return new TableDataSnap();
        }
        public void SetColumnTotal(int columnIndex, ColumnTotalValue totalVal)
        {
            if (_columnTotals.ContainsKey(columnIndex))
            {
                _columnTotals[columnIndex] = totalVal;
            }
        }
        public int GetColumnTotal(int columnIndex)
        {
            if (_columnTotals.ContainsKey(columnIndex))
            {
                return _columnTotals[columnIndex].Value;
            }

            return -1;
        }

        private void FillDeckFive()
        {
            if (CurrentIndex < 0 || CurrentIndex >= _deck.Length) return;

            _deckFive.Clear();

            int avalibleAfterIndex = (_deck.Length - 1) - CurrentIndex;

            for (int i = CurrentIndex + Mathf.Clamp(avalibleAfterIndex, 0, 4); i >= CurrentIndex; i--)
            {
                _deckFive.Push(_deck[i]);
            }

        }

    }

}
