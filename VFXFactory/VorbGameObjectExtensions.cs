using System;
using System.Collections;
using UnityEngine;

namespace VOrb.Extensions
{
    /// <summary>
    /// Класс расширений
    /// </summary>
    public static class VorbGameObjectExtensions
    {


            public class RoutineResolver
            {
                private Action _callback = null;
                private bool _startValue = false;

                public RoutineResolver(Action callback, bool startValue)
                {
                    this._callback = callback;
                    this._startValue = startValue;
                }

                public IEnumerator ChangeBack(float time, GameObject obj)
                {
                    yield return new WaitForSeconds(time);
                    obj?.SetActive(!_startValue);
                    _callback?.Invoke();
                }

                public IEnumerator InvokeAfter(float time)
                {
                    yield return new WaitForSeconds(time);
                    _callback?.Invoke();
                }

                public IEnumerator DestroyAfter(GameObject obj, float time)
                {
                    yield return new WaitForSeconds(time);
                    UnityEngine.Object.Destroy(obj);
                    _callback?.Invoke();
                }

            }

            public static Coroutine TryStartCoroutine(this MonoBehaviour self, IEnumerator routine)
            {
                if (self.gameObject.activeInHierarchy)
                {
                    return self.StartCoroutine(routine);
                }
                else return null;
            }
            public static void SetActive(this GameObject T, bool value, float Time, MonoBehaviour parent)
            {
                RoutineResolver changer = new RoutineResolver(null, value);
                T.SetActive(value);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.ChangeBack(Time, T));
                }

            }

            public static void SetActive(this GameObject T, bool value, float Time, MonoBehaviour parent, Action callback)
            {
                RoutineResolver changer = new RoutineResolver(callback, value);
                T.SetActive(value);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.ChangeBack(Time, T));
                }
            }

            public static void ActivateAfter(this GameObject T, float Time, MonoBehaviour parent)
            {
                RoutineResolver changer = new RoutineResolver(null, false);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.ChangeBack(Time, T));
                }
            }
            public static void ActivateAfter(this GameObject T, float Time, MonoBehaviour parent, Action callback)
            {
                RoutineResolver changer = new RoutineResolver(callback, false);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.ChangeBack(Time, T));
                }
            }

            public static void Invoke(this Action T, float Time, MonoBehaviour parent)
            {
                RoutineResolver changer = new RoutineResolver(T, true);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.InvokeAfter(Time));
                }
            }

            public static void Destroy(this GameObject T, float Time, MonoBehaviour parent, Action callback)
            {
                RoutineResolver changer = new RoutineResolver(callback, true);
                if (parent.isActiveAndEnabled)
                {
                    parent.StartCoroutine(changer.DestroyAfter(T, Time));
                }
            }

            public static T GetOrAddComponent<C, T>(this C self) where T : Component where C : Component
            {
                var component = self.GetComponent<T>();
                if (component == null)
                {
                    return self.gameObject.AddComponent<T>();
                }
                return component;
            }
            public static T GetOrAddComponent<T>(this GameObject self) where T : Component
            {
                var component = self.GetComponent<T>();
                if (component == null)
                {
                    return self.AddComponent<T>();
                }
                return component;
            }



        
    }

}  