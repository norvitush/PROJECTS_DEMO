using GoldenSoft.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoldenSoft.Column21.Core
{
    public struct TableDataSnap
    {        
        private int _lastSendedStep;
        private int _lastValidatedStep;
        private int _gameTime;
        public bool IsActualSnap => (_lastSendedStep == _lastValidatedStep);

        public int DeckCount;
        
        public static TableDataSnap Zero => new TableDataSnap(true);
        public bool IsZero() => Deck == null || !Activated;

        public Card[] Deck;
        public Card HandCard;
        public Card HoldCard;
        public int Score;
        public  int Lifes;

        public Card[][] ColumnsCards;
        public ColumnTotalValue[] ColumnsTotals;

        public int Time { get => _gameTime; set { _gameTime = value; } }

        public bool Activated;
        public string addyState;

        public int StepCount { set => _lastValidatedStep = value; }

        private Dictionary<int, List<Card>> _columnsTempData;
        private Dictionary<int, ColumnTotalValue> _columnsTotalTempData;

        private IncomeStep[] _stepsOrdered;
        public IncomeStep[] Steps => _stepsOrdered != null && _stepsOrdered.Length != 0 ? _stepsOrdered : new IncomeStep[0];

        public TableDataSnap(bool initZeroValues = true) : this()
        {
            DeckCount = 52;
            Lifes = 3;
            Score = 0;

            HoldCard = Card.Zero;
            HandCard = Card.Zero;

            Deck = new Card[0];

            if (initZeroValues)
            {
                ColumnsCards = new Card[4][]
                {
                    new Card[0], new Card[0], new Card[0], new Card[0]
                };

                ColumnsTotals = new ColumnTotalValue[4] { ColumnTotalValue.Zero, ColumnTotalValue.Zero, ColumnTotalValue.Zero , ColumnTotalValue.Zero };               
            }

            Activated = false;

        }
        public void SetLastStep(int stepId)
        {
            if (_lastSendedStep < stepId)
                _lastSendedStep = stepId;
        }

        public int GetLastSendedStep() => _lastSendedStep;
        public int GetLastValidatedStep() => _lastValidatedStep;

        public int DecreaseLifes(int val)
        {
            Lifes = Mathf.Clamp(Lifes - val, 0, 3);
            return Lifes;
        }

        public void AddToScore(int val) => Score += val;

        public void SetColumnTotal(int columnIndex, ColumnTotalValue totalVal)
        {
            if (ColumnsTotals == null || columnIndex > ColumnsTotals.Length) return;
            
            ColumnsTotals[columnIndex] = totalVal;            
        }

        public void AddToColumn(int columnIndex, Card card)
        {
            if (ColumnsCards == null || columnIndex > ColumnsCards.Length) return;

            int currentLenght = ColumnsCards[columnIndex].Length;
            
            Card[] oldVal = ColumnsCards[columnIndex];

            ColumnsCards[columnIndex] = new Card[currentLenght + 1];

            for (int i = 0; i < currentLenght; i++)
            {
                ColumnsCards[columnIndex][i] = oldVal[i];
            }

            ColumnsCards[columnIndex][currentLenght] = card;

        }
        public Card[] GetCardsInColumn(int columnIndex)
        {
            if (ColumnsCards == null || columnIndex > ColumnsCards.Length) return null;

            return ColumnsCards[columnIndex];

        }



        public void ClearColumn(int columnIndex)
        {
            if (ColumnsCards == null || columnIndex > ColumnsCards.Length) return;

            ColumnsCards[columnIndex] = new Card[0];
        }

        public Card PopDeckCard()
        {
            int deckCount = Deck.Length;

            if (Deck == null || deckCount == 0) return Card.Zero;

            DeckCount = Mathf.Clamp(DeckCount - 1, 0, 51);

            Card card = Deck[0];
            
            if (deckCount == 1)
            {
                
                Deck = new Card[0];
               
                return card;
            }

            Card[] newDeck = new Card[deckCount - 1];

            for (int i = 1; i < deckCount; i++)
            {
                newDeck[i - 1] = Deck[i];
            }

            Deck = newDeck;

            return card;
        }

        public void UpdateDeck(Card[] deck, int stepsCount = 0 )
        {
            _lastValidatedStep = stepsCount;
            // DeckCount = Mathf.Clamp(DeckCount - offset, 0, 52);

            if (deck == null) Deck = new Card[0];

            if(HandCard == Card.Zero)
                Deck = deck;
            else
            {
                if (deck.Length == 0) return;

                if (deck[0] != HandCard) { HandCard = Card.Zero;  Deck = deck; }
                else
                {
                    Card[] partedDeck = new Card[deck.Length - 1];
                    for (int i = 0; i < partedDeck.Length; i++)
                    {
                        partedDeck[i] = deck[i + 1];
                    }
                    Deck = partedDeck;
                }
            }

            string deckStr = "";

            if (Deck != null)
            {
                for (int i = 0; i < Deck.Length; i++)
                {
                    deckStr += Deck[i] + ":";
                }
            }

          
        }
        public bool TryAddToHand(Card card)
        {
            if (HandCard != Card.Zero) return false;

            HandCard = card;

            return true;
        }

        public void UpdateStateSteps(Card[] deck, List<IncomeStep> steps, int gameTime)
        {
            if(gameTime > 0 )
            _gameTime = gameTime;

            Card savedHandCard = Card.Zero;

            if (HandCard != Card.Zero) savedHandCard = HandCard;

            if (steps != null && steps.Count > 0)
                _stepsOrdered = steps.OrderBy(v => v.StepID).ToArray();
            else
                _stepsOrdered = new IncomeStep[0];

            Lifes = 3;
            Score = 0;
            DeckCount = 52;

            if (_columnsTempData == null) 
                _columnsTempData = new Dictionary<int, List<Card>>()
                {
                    { 0, new List<Card>()},{ 1, new List<Card>()}, { 2, new List<Card>()}, { 3, new List<Card>()}
                };
            else
            {
                foreach (var item in _columnsTempData.ToList())
                {
                    item.Value.Clear();
                }
            }

            
            if (_columnsTotalTempData == null)
            {
                
                _columnsTotalTempData = new Dictionary<int, ColumnTotalValue>()
                {
                    { 0, ColumnTotalValue.Zero}, {1,ColumnTotalValue.Zero }, {2, ColumnTotalValue.Zero }, {3, ColumnTotalValue.Zero }
                };
            }              
            else
            {
                foreach (var item in _columnsTotalTempData.ToList())
                {
                    _columnsTotalTempData[item.Key] = ColumnTotalValue.Zero;
                }
            }

            HoldCard = Card.Zero;
            HandCard = Card.Zero;

            Deck = deck;
            
            if(_stepsOrdered.Length > 0)
            {
                

                int cnt = _stepsOrdered.Length;

                for (int i = 0; i < cnt; i++)
                {
                    AddStep(_stepsOrdered[i]);
                }

                ColumnsCards = new Card[4][]
                {
                   _columnsTempData[0].ToArray(), _columnsTempData[1].ToArray(), _columnsTempData[2].ToArray(), _columnsTempData[3].ToArray()
                };

                ColumnsTotals = new ColumnTotalValue[4]
                {
               _columnsTotalTempData[0], _columnsTotalTempData[1],_columnsTotalTempData[2], _columnsTotalTempData[3]
                };
            }
            else
            {
                ColumnsCards = new Card[4][] {  new Card[0], new Card[0], new Card[0], new Card[0]  };

                ColumnsTotals = new ColumnTotalValue[4] { ColumnTotalValue.Zero, ColumnTotalValue.Zero, ColumnTotalValue.Zero, ColumnTotalValue.Zero };
            }
            
            

            if(savedHandCard != Card.Zero && deck.Length > 0)
            {
                if (savedHandCard == deck[0])
                {
                    HandCard = deck[0];

                    if (deck.Length == 1) 
                        Deck = new Card[0];
                    else 
                        Deck = new Card[4] { deck[1], deck[2], deck[3], deck[4] };

                    DeckCount--;

                }

            }

            

            _lastValidatedStep = steps != null? steps.Count : 0;

            Activated = true;
        }

        private void AddStep(IncomeStep step)
        {
            //UnityEngine.Debug.Log($"Add Step  {step}");

            int action = int.Parse(step.action);
            string cardName = step.CardName.Trim();
            Card card = new Card(cardName[1], cardName[0]);

            int columnN = -1;

            if (action < 5 )
            {
                columnN = action - 1;

                _columnsTempData[columnN].Add(card);
                DeckCount--;
            }

            if(action == 5 && HoldCard == Card.Zero)
            {
                HoldCard = card;
                DeckCount--;
            }

            if(action > 50 && HoldCard != Card.Zero)
            {
                columnN = action - 51;

                HoldCard = Card.Zero;

                _columnsTempData[columnN].Add(card);
               
            }

             
            if (columnN >= 0)
            {
                int tmpTotal = CalculateColumnTotal(_columnsTempData[columnN].ToArray());                                

                if (tmpTotal > 21)
                {
                    Lifes--;
                    _columnsTempData[columnN].Clear();
                    tmpTotal = 0;
                }
                if (tmpTotal == 21)
                {
                    Score += 200;
                    _columnsTempData[columnN].Clear();
                    tmpTotal = 0;
                }

                _columnsTotalTempData[columnN] = new ColumnTotalValue(tmpTotal, tmpTotal>=21, 21);
            }
            

        }

        public void AddStepToCurrentState(IncomeStep step)
        {
            if (step == null || string.IsNullOrEmpty(step.CardName)) return;

            addyState = "ADD_LAST";

            UnityEngine.Debug.Log($"+1 STEP {step} ");

            _columnsTempData = new Dictionary<int, List<Card>> { 
                { 0, ColumnsCards[0].ToList() },
                { 1, ColumnsCards[1].ToList() },
                { 2, ColumnsCards[2].ToList() },
                { 3, ColumnsCards[3].ToList() }
            };

            _columnsTotalTempData = new Dictionary<int, ColumnTotalValue> {
                { 0, ColumnsTotals[0] },
                { 1, ColumnsTotals[1] },
                { 2, ColumnsTotals[2] },
                { 3, ColumnsTotals[3] },
            };


            AddStep(step);

            ColumnsCards = new Card[4][]
               {
                   _columnsTempData[0].ToArray(), _columnsTempData[1].ToArray(), _columnsTempData[2].ToArray(), _columnsTempData[3].ToArray()
               };

            ColumnsTotals = new ColumnTotalValue[4]
            {
               _columnsTotalTempData[0], _columnsTotalTempData[1],_columnsTotalTempData[2], _columnsTotalTempData[3]
            };
        }

        public int CalculateColumnTotal(Card[] cards)
        {
            int cnt = cards.Length;
            int sum = 0;

            DeckAsset deck = SceneLoader.StartSettings.DeckData;
            List<CardAsset> assetsOfA = new List<CardAsset>();

            for (int i = 0; i < cnt; i++)
            {
                CardAsset asset = deck.GetCardAsset(cards[i]);

                if (asset.CardType == CardType.Ace) 
                    assetsOfA.Add(asset);
                else
                {
                    sum += asset.BaseValue;
                }
            }

            //Try find how much Aces with max value make total <=21
            int allACECount = assetsOfA.Count;

            if (allACECount == 0) return sum;  // skip if 0 ACES

            int maxVal = 11; //max ACE value = 11 
            
            int findedMaxCount = 0; // if value will not finded  - all ACES to min value

            int tmpSum, tmpMaxCount;

            for (int i = 0; i <= allACECount; i++)
            {
                tmpMaxCount = allACECount - i;
                tmpSum = sum + allACECount - tmpMaxCount + tmpMaxCount * maxVal;

                if (tmpSum <= 21) { findedMaxCount = tmpMaxCount;  break; }
                
            }

            sum += allACECount - findedMaxCount + findedMaxCount * maxVal;

            return sum;
        }


        public override string ToString()
        {
            string deckStr = "";

            if(Deck != null)
            {
                for (int i = 0; i < Deck.Length; i++)
                {
                    deckStr += Deck[i]+":";
                }
            }

            string colStr = "";

            if (ColumnsCards != null)
            {
                for (int col = 0; col < ColumnsCards.Length; col++)
                {
                    for (int j = 0; j < ColumnsCards[col].Length; j++)
                    {
                        colStr += ColumnsCards[col][j] + ",";
                    }
                    colStr += " | ";
                }
            }

            string totalsStr = "";

            if (ColumnsTotals != null)
            {
                for (int col = 0; col < ColumnsTotals.Length; col++)
                {
                    totalsStr += $"({ColumnsTotals[col].Value}, {ColumnsTotals[col].IsFull}) | ";
                }
            }

            return $"cards: {DeckCount}  {TimeSpan.FromSeconds(Time).Minutes.ToString().PadLeft(2,'0')}:{TimeSpan.FromSeconds(Time).Seconds.ToString().PadLeft(2, '0')}" +
                $"   deck [{deckStr}],  columns [{colStr}], totalsStr[{totalsStr}], Hand [{HandCard}], Hold [{HoldCard}], score {Score}, lifes {Lifes}";
        }
    }

    public struct ColumnTotalValue
    {
        public int WinAmount;
        public int Value;
        public bool IsFull;        

        public ColumnTotalValue(int value, bool isFull, int winAmount = 21)
        {
            Value = value;
            IsFull = isFull;
            WinAmount = winAmount;
        }

        public static ColumnTotalValue Zero => new ColumnTotalValue(0, false);
    }


}
