using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Collections;



/*  
 класс CFMove - для расширений и собственных функций движения объектов
 класс CFVectors -  для расширений и дополнительных функций работы с векторами
 CFGameObjects - класс расширений для GameObject
*/
namespace VOrb.Extensions
{
    public static class VorbExtensions
    {
        
    }

}    
namespace VOrb
{
    /// <summary>
    /// класс расширений для GameObject
    /// </summary>
    public static class CFGameObjects
    {
        public class RoutineResolver
        {
            private Action callback = null;
            private bool startValue = false;

            public RoutineResolver(Action callback, bool startValue)
            {
                this.callback = callback;
                this.startValue = startValue;
            }

            public IEnumerator ChangeBack(float t, GameObject obj)
            {
                yield return new WaitForSeconds(t);
                obj?.SetActive(!startValue);
                callback?.Invoke();
            }

            public IEnumerator InvokeAfter(float t)
            {
                yield return new WaitForSeconds(t);
                callback?.Invoke();
            }

            public IEnumerator DestroyAfter(GameObject obj, float t)
            {
                yield return new WaitForSeconds(t);
                UnityEngine.Object.Destroy(obj);
                callback?.Invoke();
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
        public static void Invoke(this MonoBehaviour parent, float Time, Action T )
        {
            RoutineResolver changer = new RoutineResolver(T, true);
            if (parent.gameObject.activeInHierarchy)
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

        public static T GetOrAddComponent<C,T>(this C self) where T: Component where C: Component
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

        public static string GetRigUID(this Rigidbody2D obj)
        {
            return (obj.gameObject.name + obj.GetInstanceID());
        }


    }

    public static class CFMesh
    {

        public static float AreaOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float a = Vector3.Distance(p1, p2);
            float b = Vector3.Distance(p2, p3);
            float c = Vector3.Distance(p3, p1);
            float p = 0.5f * (a + b + c);
            float s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
            return s;
        }
        public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float v321 = p3.x * p2.y * p1.z;
            float v231 = p2.x * p3.y * p1.z;
            float v312 = p3.x * p1.y * p2.z;
            float v132 = p1.x * p3.y * p2.z;
            float v213 = p2.x * p1.y * p3.z;
            float v123 = p1.x * p2.y * p3.z;

            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }
        public static float AreaOfMesh(this Mesh mesh)
        {
            float area = 0f;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];
                area += AreaOfTriangle(p1, p2, p3);
            }

            return area;
        }

        public static float VolumeOfMesh(this Mesh mesh)
        {
            float volume = 0;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 p1 = vertices[triangles[i + 0]];
                Vector3 p2 = vertices[triangles[i + 1]];
                Vector3 p3 = vertices[triangles[i + 2]];
                volume += SignedVolumeOfTriangle(p1, p2, p3);
            }
            return Mathf.Abs(volume * Mathf.Pow(10, 5));
        }
    }
public static class CFMove
    {
       public static void MoveRigToPoint(this Rigidbody2D Rig, Vector3 Direction, float Distance, float steps = 1, bool isFixedUpdate = true)
        {
            // деление только на дельтатайм - движение за один кадр (V=L/T)
            if (isFixedUpdate)
                Rig.velocity = (Direction * Distance) / (Time.fixedDeltaTime * steps);
            else
                Rig.velocity = (Direction * Distance) / (Time.deltaTime * steps);
        }
        public static void MoveRigToPoint(this Rigidbody Rig, Vector3 DirectionNormalized, float Distance, float steps = 1, bool isFixedUpdate = true)
        {
            // деление только на дельтатайм - движение за один кадр (V=L/T)
            if (isFixedUpdate)
                Rig.velocity = (DirectionNormalized * Distance) / (Time.fixedDeltaTime * steps);
            else
                Rig.velocity = (DirectionNormalized * Distance) / (Time.deltaTime * steps);
        }
        public static void MoveRigToPoint(this Rigidbody Rig, Vector3 Direction, float steps = 1, bool isFixedUpdate = true)
        {
            // деление только на дельтатайм - движение за один кадр (V=L/T)
            if (isFixedUpdate)
                Rig.velocity = Direction / (Time.fixedDeltaTime * steps);
            else
                Rig.velocity = Direction / (Time.deltaTime * steps);
        }
    }

    static class CFVectors
    {
        //расширения камеры (Координаты pixel, world, screen)
        public static Vector2 WorldCamMin(this Camera thisCamera)
        {
            return thisCamera.ViewportToWorldPoint(new Vector2(0, 0));
        }
        public static Vector2 WorldCamMax(this Camera thisCamera)
        {
            return thisCamera.ViewportToWorldPoint(new Vector2(1, 1));
        }
        
        public static float WorldCamWidth(this Camera thisCamera)
        {
            return thisCamera.WorldCamMax().x - thisCamera.WorldCamMin().x;
        }
        public static float WorldCamHeigth(this Camera thisCamera)
        {
            return thisCamera.WorldCamMax().y - thisCamera.WorldCamMin().y;
        }
        public static Vector2 WorldCamSize(this Camera thisCamera)
        {
            Vector2 min = thisCamera.WorldCamMin();
            Vector2 max = thisCamera.WorldCamMax();
            return new Vector2(max.x - min.x, max.y - min.y);
        }

        public static Vector3 GetWorldPositionOnCanvas (this Camera cam, Vector3 position,  Canvas canvas)
        {
            Vector3 scrn = cam.WorldToScreenPoint(position);
            return scrn.ConvertPixelCoordinatesToWorld(canvas);
        }

        //расширения векторов
        /// <summary>
        /// Возвращает мировую координату находящуюся на канве
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Canvas"></param>
        /// <returns></returns>
        public static Vector3 ConvertPixelCoordinatesToWorld(this Vector3 position, Canvas _Canvas)
        {
            Vector3 Return = Vector3.zero;

            if (_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
            Return = position;
            }
            else if (_Canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Vector2 tempVector = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_Canvas.transform as RectTransform, position, _Canvas.worldCamera, out tempVector);
                Return = _Canvas.transform.TransformPoint(tempVector);
            }

            return Return;
        }
        public static Vector3 ConvertPixelCoordinatesToWorld(this Vector3 position, Canvas cnv, float VerticalZ)
        {

            float maxy = cnv.pixelRect.yMax * 0.5f;
            float xCentr = cnv.pixelRect.center.x;
            float cnv_rotation = cnv.transform.rotation.eulerAngles.x;
            Vector3 top_point = new Vector3(position.x, cnv.pixelRect.yMax, 0);
            Vector3 cur_point = new Vector3(position.x, position.y, 0);


            Vector3 w_cur_point = cur_point.ConvertPixelCoordinatesToWorld(cnv);
            Vector3 w_top_point = top_point.ConvertPixelCoordinatesToWorld(cnv);
            float a = Vector3.Distance(w_top_point, w_cur_point);
            float b = Mathf.Tan(Mathf.Deg2Rad * cnv_rotation) * a;
            float vert_aligment = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));

            if(VerticalZ==0)
            {
                return new Vector3(w_top_point.x, w_top_point.y - vert_aligment, w_top_point.z);
            }
            else
            {
                w_top_point = w_top_point.GetVector2().GetVector3(VerticalZ);                
                a = Vector3.Distance(w_top_point, w_cur_point);
                Vector3 h_c = w_cur_point.GetVector2().GetVector3(VerticalZ);
                float h = Vector3.Distance(h_c, w_cur_point);
                float vert_aligment_part1 = Mathf.Sqrt(Mathf.Pow(a, 2) - Mathf.Pow(h, 2));
                float vert_aligment_part2 = Mathf.Tan(Mathf.Deg2Rad * cnv_rotation) * h;
                vert_aligment = (Mathf.Sqrt(Mathf.Pow(a, 2) - Mathf.Pow(h, 2)))
                                + (Mathf.Tan(Mathf.Deg2Rad * cnv_rotation) * h);                
                return new Vector3(w_top_point.x, w_top_point.y - vert_aligment, w_top_point.z);
            }


        }

        public static void SwingVectors(this Vector2 _, ref Vector2 numFirst, ref Vector2 numSecond)
        {
            Vector2 tmp;
            tmp = numFirst;
            numFirst = numSecond;
            numSecond = tmp;
        }
        /// <summary>
        /// Возвращает угол в градусах между векторами
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static float GetAngleWith(this Vector2 origin, Vector2 second)
        {
            if (origin.normalized == -second.normalized)
            {
                return 180;
            }
            if (origin.normalized == second.normalized)
            {
                return 0;
            }
            float a = (origin - second).magnitude;
            float cosAlph = (origin.magnitude * origin.magnitude + second.magnitude * second.magnitude - a * a) / (2 * origin.magnitude * second.magnitude);
            return Mathf.Acos(cosAlph) * Mathf.Rad2Deg;
        }

        public static bool isNearToDestiny(this Vector3 testedPoint, Vector2[] PointsArr,  float chekSize = 0.1f)
        {
            for (int i = 0; i < PointsArr.Length; i++)
            {
                Debug.Log(chekSize);
                if (new Rect(PointsArr[i] - new Vector2(chekSize, chekSize), (new Vector2(chekSize, chekSize)) * 2).Contains(testedPoint))
                {
                    return true;
                }
            }
            return false;

        }
        public static bool isNearToDestiny(this Vector3 testedPoint, Vector2 Destinypoint, float chekSize = 0.1f)
        {

            if (new Rect(Destinypoint - new Vector2(chekSize, chekSize), (new Vector2(chekSize, chekSize)) * 2).Contains(testedPoint))
                {
                    return true;
                }
            return false;

        }
        public static bool isNearToDestiny(this Vector2 testedPoint, Vector2 Destinypoint, float chekSize = 0.1f)
        {

            if (new Rect(Destinypoint - new Vector2(chekSize, chekSize), (new Vector2(chekSize, chekSize)) * 2).Contains(testedPoint))
            {
                return true;
            }
            return false;

        }
        public static Vector2 GetVector2(this Vector3 a)
        {
            return new Vector2(a.x, a.y);
        }

        public static Vector3 SetYTo(this Vector3 a, float newY)
        {
            return new Vector3(a.x, newY, a.z);
        }

        public static Vector3 SetXTo(this Vector3 a, float newX)
        {
            return new Vector3(newX, a.y, a.z);
        }

        public static Vector3 SetZTo(this Vector3 a, float newZ)
        {
            return new Vector3(a.x, a.y, newZ);
        }

        public static Vector2 GetVector2(this Vector3 a, bool needNormalize)
        {
            if (needNormalize)
                a = a.normalized;
            return new Vector2(a.x, a.y);
        }
        public static Vector3 GetVector3(this Vector2 a, float Z = 0f)
        {
            return new Vector3(a.x, a.y, Z);
        }
        public static Vector2 ParseToVector2(this string s)
        {
            
            int postX = s.IndexOf(',');
            int postY = s.LastIndexOf(',');
            if (postX==postY) 
                    postY = s.Length - 1;                           
            if (postY - postX > 1)
            {
                string sub;
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                try
                {
                    sub = s.Substring(1, postX - 1);
                    float x = float.Parse(sub, NumberStyles.Any, ci);
                    sub = s.Substring(postX + 1, postY - postX - 1);
                    float y = float.Parse(sub, NumberStyles.Any, ci);
                    return new Vector2(x, y);
                }
                catch (FormatException r)
                {
                    Debug.Log("Parse fail " + r.Message);
                    return Vector2.zero;
                }
            }
            else return Vector2.zero;
        }
    }
   
}