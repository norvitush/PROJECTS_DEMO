using GoldenSoft.Column21;
using GoldenSoft.Column21.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoldenSoft.UI.MVVM
{
    public class PlaySnapsKeeper
    {
        private struct SuccessKeeperData
        {
            public int Offset;            
            public int StepNum;
            public TableDataSnap tableData;
            public Dictionary<int, StepTransaction> OutputSteps;

            public SuccessKeeperData(int offset, int stepNum, TableDataSnap tableData, Dictionary<int, StepTransaction> outputSteps)
            {
                Offset = offset;
                StepNum = stepNum;
                this.tableData = tableData;
                OutputSteps = outputSteps;
            }
        }

        private SuccessKeeperData _lastValidatedData = new SuccessKeeperData(0, 0, TableDataSnap.Zero, new Dictionary<int, StepTransaction>());
        private int _stepNum = 0;
        private int _localOffset = 0;
        private bool _isInited = false;
        private readonly Dictionary<int, StepTransaction> _outputSteps = new Dictionary<int, StepTransaction>();
        private UrlGetParams _urlParams;
        private Action _onUpdateData;
        private Action<int> _onTimerChange;
        private Action _initSuccessAction;

        public TableDataSnap LocalPlayerSnap = TableDataSnap.Zero;
        public TableDataSnap ServerPlayerSnap = TableDataSnap.Zero;
        public bool IsLocalSnapActual { get { UnityEngine.Debug.Log($"Last sended step#{LocalPlayerSnap.GetLastSendedStep()} validated#{LocalPlayerSnap.GetLastValidatedStep()}"); return LocalPlayerSnap.IsActualSnap; } }
        public TableDataSnap OpponentSnap = TableDataSnap.Zero;

        public void Init(UrlGetParams urlParams, Action onUpdateData, Action successCallback, Action<int> onTimerChange)
        {
            _initSuccessAction = successCallback;

            UnityEngine.Debug.Log($"INIT ");

            _urlParams = urlParams;
            _onUpdateData = onUpdateData;
            _onTimerChange = onTimerChange;

            _stepNum = 0;

            var nullStep = new StepTransaction(_stepNum++, "0", "0", "", (res) => { OnValidateSnapError(res); }, (res) => {

                //success validated!

                var serverDeck = res.data.GetCards();

                ServerPlayerSnap.UpdateStateSteps(serverDeck, null, res.data.TimerValue);

                LocalPlayerSnap.UpdateStateSteps(serverDeck, null, -1);

                RecieveAdditionalData(res.data);

                _initSuccessAction.TryInvoke();

                _isInited = true;

                OnSuccess();

            });

            AddOutputStep(nullStep);

            ApiConnector.StateRoom(urlParams, _outputSteps.Select(v => v.Value.step).ToArray(), (res) => nullStep.Validate(res));
        }
        public void SendZeroStep()
        {


            var nullStep = new StepTransaction(0, "0", "0", "", null, null);

            OutputStep[] outMass = new OutputStep[1];

            outMass[0] = nullStep.step;

            ApiConnector.StateRoom(_urlParams, outMass, (res) => {

                RecieveAdditionalData(res);

                if (res == null) return;

                if (string.IsNullOrEmpty(res.State) && LocalPlayerSnap.addyState == ApiConnector.NO_RESPONSE)
                {
                    UnityEngine.Debug.Log($"CONNECTION RESTORED");
                    ResetToLastSuccess();
                    return;
                }

                _onUpdateData.TryInvoke();
            });


        }
        public IncomeStep GetLastOutputAsIncome()
        {
            var transaction = _outputSteps[_outputSteps.Max(el => el.Key)];

            var last = transaction.step;

            return new IncomeStep { action = last.action, StepNum = transaction.ID.ToString(), CardName = last.cardName };
        }
        public void AddLastStep()
        {
            LocalPlayerSnap = _lastValidatedData.tableData;
            LocalPlayerSnap.AddStepToCurrentState(GetLastOutputAsIncome());
        }
        public void RecieveAdditionalData(IncomeStateDataResponse data)
        {
            if (data == null) return;

            //LocalPlayerSnap.Time = data.TimerValue;
            ServerPlayerSnap.Time = data.TimerValue;

            //UnityEngine.Debug.Log($"RecieveAdditionalData timer {data.TimerValue}");
            _onTimerChange.TryInvoke(ServerPlayerSnap.Time);

            if (!string.IsNullOrEmpty(data.State))
            {
                UnityEngine.Debug.Log($"ADDITIONAL STATE: {data.State}");

                LocalPlayerSnap.addyState = data.State;
            }


            if (data == null || data.OpponentSteps == null || data.OpponentSteps.Length == 0)
            {
                OpponentSnap.UpdateStateSteps(new Card[0], null, data.TimerValue);

                return;

            }


            OpponentSnap.UpdateStateSteps(new Card[0], data.OpponentSteps.ToList(), data.TimerValue);


        }
        public void AddOutputStep(StepTransaction step)
        {
            if (!_outputSteps.TryGetValue(step.ID, out StepTransaction stepVal))
            {
                _outputSteps.Add(step.ID, step);

                if (step.ID != 0)
                {
                    LocalPlayerSnap.SetLastStep(step.ID);
                    ServerPlayerSnap.SetLastStep(step.ID);
                }
            }
        }
        public int MoveHandToHold(Card card)
        {
            UnityEngine.Debug.Log($"keeper send MOVE HAND TO HOLD");

            _localOffset++;

            CardLiteral cardLit = new CardLiteral(card);
            var transaction = new StepTransaction(_stepNum++, _localOffset.ToString(), "5", cardLit.Value, OnValidateSnapError, MoveHandToHoldSuccess);

            AddOutputStep(transaction);

            ApiConnector.StateRoom(_urlParams, _outputSteps.Select(v => v.Value.step).ToArray(), (res) => transaction.Validate(res));

            return _stepNum - 1;
        }
        public void MoveHoldToColumn(Card card, int columnIndex)
        {
            CardLiteral cardLit = new CardLiteral(card);

            var transaction = new StepTransaction(_stepNum++, _localOffset.ToString(), "5" + (columnIndex + 1).ToString()[0], cardLit.Value, OnValidateSnapError, MoveHoldToColumnSuccess);

            AddOutputStep(transaction);

            ApiConnector.StateRoom(_urlParams, _outputSteps.Select(v => v.Value.step).ToArray(), (res) => transaction.Validate(res));
        }
        public void MoveHandToColumn(Card card, int columnIndex)
        {
            UnityEngine.Debug.Log($"keeper send MOVE HAND TO COLUMN {columnIndex + 1} card {card}");

            _localOffset++;

            CardLiteral cardLit = new CardLiteral(card);

            var transaction = new StepTransaction(_stepNum++, _localOffset.ToString(), (columnIndex + 1).ToString().Substring(0, 1), cardLit.Value, OnValidateSnapError, MoveHandToColumnSucces);

            AddOutputStep(transaction);

            ApiConnector.StateRoom(_urlParams, _outputSteps.Select(v => v.Value.step).ToArray(), (res) => transaction.Validate(res));
        }

        private void MoveHandToHoldSuccess((int ID, IncomeStateDataResponse data) resInfo)
        {
            if (_stepNum - 1 == resInfo.ID)
            {
                ServerPlayerSnap.UpdateStateSteps(resInfo.data.GetCards(), resInfo.data.MySteps.ToList(), resInfo.data.TimerValue);

                UnityEngine.Debug.Log($"UPDATE//MoveHandToHold//  server snap {ServerPlayerSnap}");

                int stepsCount = (resInfo.data.MySteps != null) ? resInfo.data.MySteps.Length : 0;

                LocalPlayerSnap.UpdateDeck(resInfo.data.GetCards(), stepsCount);
                UnityEngine.Debug.Log($"UPDATE//MoveHandToHold//  local snap {LocalPlayerSnap}");

                RecieveAdditionalData(resInfo.data);

                if (!_isInited) { _isInited = true; _initSuccessAction.TryInvoke(); }

                OnSuccess();
            }
        }
        private void MoveHoldToColumnSuccess((int ID, IncomeStateDataResponse data) resInfo)
        {
            if (_stepNum - 1 == resInfo.ID)
            {
                ServerPlayerSnap.UpdateStateSteps(resInfo.data.GetCards(), resInfo.data.MySteps.ToList(), resInfo.data.TimerValue);

                UnityEngine.Debug.Log($"UPDATE//MoveHoldToColumn//  ServerPlayerSnap snap {ServerPlayerSnap}");

                LocalPlayerSnap.StepCount = resInfo.data.MySteps.Length;

                RecieveAdditionalData(resInfo.data);

                if (!_isInited) { _isInited = true; _initSuccessAction.TryInvoke(); }

                OnSuccess();
            }
        }
        private void MoveHandToColumnSucces((int ID, IncomeStateDataResponse data) resInfo)
        {
            if (_stepNum - 1 == resInfo.ID)
            {
                ServerPlayerSnap.UpdateStateSteps(resInfo.data.GetCards(), resInfo.data.MySteps.ToList(), resInfo.data.TimerValue);

                UnityEngine.Debug.Log($"UPDATE//MoveHandToColumn//  server snap {ServerPlayerSnap}");

                int stepsCount = (resInfo.data.MySteps != null) ? resInfo.data.MySteps.Length : 0;

                LocalPlayerSnap.UpdateDeck(resInfo.data.GetCards(), stepsCount);

                UnityEngine.Debug.Log($"UPDATE//MoveHandToColumn//  local snap {LocalPlayerSnap}");

                RecieveAdditionalData(resInfo.data);

                if (!_isInited) { _isInited = true; _initSuccessAction.TryInvoke(); }

                OnSuccess();
            }
        }
        private void OnSuccess()
        {
            _onUpdateData.TryInvoke();
            _onTimerChange.TryInvoke(ServerPlayerSnap.Time);

            _lastValidatedData = new SuccessKeeperData(_localOffset, _stepNum, ServerPlayerSnap, _outputSteps.ToDictionary(el => el.Key, el => el.Value));

        }
        private void ResetToLastSuccess()
        {
            UnityEngine.Debug.Log($"ResetLocalData ");

            if (_lastValidatedData.OutputSteps == null) return;

            var maxIDTransaction = _outputSteps.OrderByDescending(tr => tr.Key).FirstOrDefault().Value;

            _outputSteps.Clear();

            //copy output transactions
            if (_lastValidatedData.OutputSteps != null && _lastValidatedData.OutputSteps.Count > 0)
            {
                foreach (var transaction in _lastValidatedData.OutputSteps)
                {
                    _outputSteps.Add(transaction.Key, transaction.Value);
                }
            }

            UnityEngine.Debug.Log($"UNVALID? local snap {LocalPlayerSnap}");
            bool isLocalHandCard = LocalPlayerSnap.HandCard != Card.Zero;

            LocalPlayerSnap = _lastValidatedData.tableData;

            if (isLocalHandCard) { LocalPlayerSnap.HandCard = LocalPlayerSnap.PopDeckCard(); }

            _localOffset = _lastValidatedData.Offset;
            _stepNum = _lastValidatedData.StepNum;

            UnityEngine.Debug.Log($"restored snap {LocalPlayerSnap}");

            //if (maxIDTransaction.ID == _stepNum)
            //{
            //    UnityEngine.Debug.Log($"TRY SEND TRANSACTION AGAIN last:{maxIDTransaction.ID} and must be: {_stepNum}");

            //    _stepNum++;

            //    AddOutputStep(maxIDTransaction);

            //    if (maxIDTransaction.Action <= 5) _localOffset++;

            //    ApiConnector.StateRoom(_urlParams, _outputSteps.Select(v => v.Value.step).ToArray(), (res) => maxIDTransaction.Validate(res));
            //}
            //else
            //{

            UnityEngine.Debug.Log($"JUST RESET TO SNAP (ID FAR AWAY) last:{maxIDTransaction.ID} and must be: {_stepNum}");
            _onUpdateData.TryInvoke();
            //}

        }
        private void OnValidateSnapError((int ID, IncomeStateDataResponse data) wrongData)
        {


            if (wrongData.data == null) wrongData.data = new IncomeStateDataResponse { State = ApiConnector.NO_RESPONSE };

            UnityEngine.Debug.Log($"!!! OnValidateSnapError { wrongData.data.State}");

            if (!string.IsNullOrEmpty(wrongData.data.State))
            {
                UnityEngine.Debug.Log($"ADDITIONAL RESPONSE {wrongData.data.State}");

                LocalPlayerSnap.addyState = wrongData.data.State;

                _onUpdateData.TryInvoke();
                return;
            }

            //int wrongLenght = wrongData.data.MySteps != null ? wrongData.data.MySteps.Length : 0;

            //if (_lastWrongResponse == null || _lastWrongResponse.MySteps == null ||  _lastWrongResponse.MySteps.Length < wrongLenght)
            //        _lastWrongResponse = wrongData.data;

            ResetToLastSuccess();
        }
    }
}