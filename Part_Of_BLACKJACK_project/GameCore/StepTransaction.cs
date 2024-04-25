using GoldenSoft.Column21.Core;
using System;
using System.Linq;

namespace GoldenSoft.UI.MVVM
{
    public class StepTransaction
    {
        public int ID;

        public OutputStep step;
        private IncomeStep _incomeStep;

        private Action<(int ID, IncomeStateDataResponse data)> _onValidateError;
        private Action<(int ID, IncomeStateDataResponse data)> _onValidateSuccess;

        public void ReplaceValidateSuccessAction(Action<(int ID, IncomeStateDataResponse data)> onValidateSuccess)
        {
            _onValidateSuccess = onValidateSuccess;
        }

        public StepTransaction(int iD, string offset, string action, string cardName,
                                Action<(int ID, IncomeStateDataResponse data)> onValidateError =  null, 
                                Action<(int ID ,IncomeStateDataResponse data)> onValidateSuccess = null)
        {
            ID = iD;
            step = new OutputStep();
            step.offset = offset;
            step.action = action;
            step.cardName = cardName.Length > 1?new string(new char[] { cardName.ToUpper()[0], cardName.ToLower()[1] }):"";

            _onValidateError = onValidateError;
            _onValidateSuccess = onValidateSuccess;
        }

        public void Validate(IncomeStateDataResponse incomeData)
        {
            
            bool validated = (incomeData != null && incomeData.MySteps == null);

            try
            {

                _incomeStep = (incomeData.MySteps == null || incomeData.MySteps.Length < ID) ? null : incomeData.MySteps[ID - 1];
            }
            catch (Exception)
            {

                validated = false;
            }

            if(ID > 0)
            {

                if (_incomeStep == null) validated = false;
                else
                {
                    if ((int.TryParse(_incomeStep.StepNum, out int intStep) && intStep == ID)
                        && 
                        (int.TryParse(_incomeStep.action, out int intAct) && intAct == Action))
                    {
                        validated = _incomeStep.CardName.ToLower().Trim() == step.cardName.ToLower().Trim();
                    }
                }
            }

            //test for any NO DATA RESULTS
            if (incomeData != null && !string.IsNullOrEmpty(incomeData.State)) validated = false;

            if (validated)
            {
                _onValidateSuccess.TryInvoke((ID, incomeData));
            }
            else
            { _onValidateError.TryInvoke((ID, incomeData)); }
        }

        public int Offset => int.Parse(step.offset);
        public int Action => int.Parse(step.action);
        public Card Card
        {
            get
            {
                if (String.IsNullOrEmpty(step.cardName)) return Card.Zero;

                char[] cardChars = step.cardName.ToLower().Substring(0, 2).ToArray();
                return new Card(cardChars[1], cardChars[0]);
            }
        }

        public override string ToString()
        {
            return $"ID#{ID} offset:{Offset} action:{Action} card: {Card}";
        }
    }


}