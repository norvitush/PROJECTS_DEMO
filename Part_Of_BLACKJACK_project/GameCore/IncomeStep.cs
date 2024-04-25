namespace GoldenSoft.UI.MVVM
{
    public class IncomeStep
    {
        public string StepNum;
        public string action;
        public string CardName;

        public int StepID => int.Parse(StepNum);

        public override string ToString()
        {
            return $"StepNum {StepNum} action {action}  CardName { CardName}";
        }
    }
}