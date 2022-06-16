using System;
using System.Collections.Generic;
using System.Linq;


namespace VOrb.CubesWar
{
    public class PublishEvent<TData1,TData2>
    {
        private readonly List<Action<TData1, TData2>> _callbacks = new List<Action<TData1, TData2>>();
        public void Subscribe(Action<TData1, TData2> callback)
        {            
            _callbacks.Add(callback);
        }
        public void UnSubscribe(Action<TData1, TData2> callback)
        {
            _callbacks.Remove(callback);
        }
        public void Publish(TData1 param1, TData2 param2)
        {
            var ActualCallbacks = _callbacks.Where(cb => cb != null).ToList();
            foreach (Action<TData1, TData2> callback in ActualCallbacks)
            {
                callback(param1, param2);
            }
        }
    }

    public class PublishEvent<TData1>
    {
        private readonly List<Action<TData1>> _callbacks = new List<Action<TData1>>();
        public void Subscribe(Action<TData1> callback)
        {
            _callbacks.Add(callback);
        }
        public void UnSubscribe(Action<TData1> callback)
        {
            _callbacks.Remove(callback);
        }
        public void Publish(TData1 param1)
        {
            var ActualCallbacks = _callbacks.Where(cb => cb != null).ToList();
            foreach (Action<TData1> callback in ActualCallbacks)
            {
                callback(param1);
            }
        }
    }



}