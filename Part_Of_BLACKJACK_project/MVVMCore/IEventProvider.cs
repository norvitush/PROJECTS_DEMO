using System;
namespace GoldenSoft.UI.MVVM
{
    public interface IEventProvider
    {
        event Action OnChanged;
    }
}

