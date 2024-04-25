using System;
using System.Collections.Generic;


namespace GoldenSoft.UI.MVVM
{
    public class ReactiveProperty<T> : IReactiveProperty<T>, IDisposable
    {
        [NonSerialized]
        bool _isDisposed = false;

        T _value = default(T);

        protected virtual IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return EqualityComparer<T>.Default;
            }
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {               
                if (!EqualityComparer.Equals(this._value, value))
                {
                    
                    _value = value;

                    if (_isDisposed) return;

                    OnChanged.TryInvoke();
                }
                
            }
        }

        public event Action OnChanged;

        public void Dispose()
        {
            _isDisposed = true;

        }

        ~ReactiveProperty()
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log("~ ReactiveProperty ");
#endif
        }
    }
}

