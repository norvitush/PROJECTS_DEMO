using UnityEngine;

namespace GoldenSoft.UI.MVVM
{
    public interface ITextViewModel :IViewModel
    {
        public IReactiveProperty<string> GetMessage();
    }
    public interface IFloatViewModel : IViewModel
    {
        public IReactiveProperty<float> GetFloat();
    }

    public interface IIntViewModel : IViewModel
    {
        public IReactiveProperty<int> GetInt();
    }

    public interface ICompositViewModel : ITextViewModel, IFloatViewModel, IIntViewModel
    {
    }

    public interface IViewModel<T> : IViewModel
    {
        public IReactiveProperty<T> GetValue();
    }

    public interface IViewModel { public void ForceUpdateFromModel();}

}

[System.Serializable]
public enum CurrencyType
{
    NONE = 999 , SoftCoin = 0, USTD = 1, BTC = 2
}

[System.Serializable]
public struct VirtualCurrency
{
    public string StringValue;
    public float Value;
    public CurrencyType CurrencyType;
    public decimal DecimalValue => (decimal)Value;
    
}