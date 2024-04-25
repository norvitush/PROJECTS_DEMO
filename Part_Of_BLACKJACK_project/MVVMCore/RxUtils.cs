using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoldenSoft.UI.MVVM
{
    public static class RxUtils
    {
        public static void Subscribe<T>(this GameObject gameObject, IReactiveProperty<T> property, Action<T> handler)
        {
            SubscribeInternal(gameObject, property, () => handler(property.Value));
        }
        private static IDisposable SubscribeInternal(GameObject gameObject, IEventProvider eventProvider, Action handler)
        {
            IDisposable propertyHandler = new PropertyHandler(eventProvider, handler);

            DisposeOnDestroy(propertyHandler, gameObject);

            return propertyHandler;
        }

        private static void DisposeOnDestroy(IDisposable disposable, GameObject gameObject)
        {
            var trigger = gameObject.GetComponent<ObservableDestroyTrigger>();

            if (trigger == null)
            {
                trigger = gameObject.AddComponent<ObservableDestroyTrigger>();
            }

            trigger.AddDisposableOnDestroy(disposable);
        }

        public static void DisposeOnDestroy<T>(this GameObject gameObject, T disposable) where T : IDisposable
        {
            DisposeOnDestroy(disposable, gameObject);
        }


        public class PropertyHandler : IDisposable
        {
            IEventProvider _provider;
            Action _action;

            public PropertyHandler(IEventProvider provider, Action action)
            {
                _provider = provider;
                _action = action;
                _provider.OnChanged += _action;
            }

            public void Dispose()
            {
                _provider.OnChanged -= _action;
            }

        }
    }
}
