namespace GoldenSoft.UI.MVVM
{
    public interface IReactiveProperty<T> : IEventProvider
    {
        T Value { get; }
    }
}

