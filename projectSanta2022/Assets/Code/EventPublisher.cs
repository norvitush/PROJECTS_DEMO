using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace VOrb.CubesWar
{
    public class EventPublisher
    {
        public static JoystikMoveEvent JostikMovement = new JoystikMoveEvent();
        public static JoystikDirectionSelectedEvent JoystikUp = new JoystikDirectionSelectedEvent();
        public static CubeShootEvent onShoot = new CubeShootEvent();
        public static JoystikFirstTapEvent JoysticFirstTap = new JoystikFirstTapEvent();

    }

    public struct CubePuplishingInfo
    {
        public GiftBehaviour cubeObject;
        public CubePuplishingInfo(GiftBehaviour cubeObject, int presetPlace)
        {
            this.cubeObject = cubeObject;
        }
    }

    public class JoystikMoveEvent: PublishEvent<Vector3, float> { }
    public class JoystikDirectionSelectedEvent : PublishEvent<Vector3, float> { }
    public class JoystikFirstTapEvent : PublishEvent<Vector2> { }

    public class CubeShootEvent : PublishEvent<GameObject, int> { }

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