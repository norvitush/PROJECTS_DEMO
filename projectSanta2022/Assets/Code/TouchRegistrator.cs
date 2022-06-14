using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VOrb.CubesWar
{
    public enum TouchEvent
    {
        OnPointerDown = 0,
        OnPointerUp =   1,
        OnDrag =        2,
        OnDrop =        3
    }

    public interface ITouchSensetive
    {
        UnityEvent TapEvent { get; set; }
        UnityEvent SvipeEvent { get; set; }
        void InvokeTouchEvent(TouchEvent updateType, Swipe cont);
        TouchEventResponse[] responses { get; set; }
    }

    public struct Swipe
    {
        public int swipeID;
        public float beginTime;
        public float endTime;
        public float distance;
        public bool isFinished;
        public Vector2 screenPoint_start;
        public Vector2 screenPoint_end;
        public Vector2 direction;
        public void UpdateState(PointerEventData eventData)
        {
            screenPoint_end = eventData.position;
            direction = screenPoint_end - screenPoint_start;
            distance = direction.magnitude;
            direction = direction.normalized;
            endTime = Time.unscaledTime;
        }
       
        public float GetTimePeriod()
        {
            return (endTime - beginTime);
        }
        
        public override string ToString()
        {
            string outp = "";
            outp += "ID: " + swipeID + " |" + "Start: " + screenPoint_start + " |";
            outp += "End: " + screenPoint_end + " |" + "DeltaTime: e-b   " + endTime + "-" + beginTime + "= "+ GetTimePeriod().ToString() + "    |";
            outp += "Direction: " + direction + " |" + "Distance: " + distance + " |" + "Swipe closed: " + isFinished + " |";
            return outp;
        }
    };

    public class TouchRegistrator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler //IDropHandler,
    {


        private Swipe _currentSwipe;

        const int nil = -999;
        private Rect _screenRect = Rect.zero;
        public Vector2 CamMin {get; private set;}
        public Vector2 CamMax { get; private set; }


        private void Start()
        {            
            
            CamMin = Camera.main.ViewportToScreenPoint(new Vector3(0,0,0));
            CamMax = Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0));
            
            _screenRect = new Rect(CamMin.x, CamMin.y, CamMax.x, CamMax.y);
            
            _currentSwipe = new Swipe();
            _currentSwipe.swipeID = nil;
            _currentSwipe.isFinished = true;
            
        }

        public void OnDrag(PointerEventData eventData)
        {
                        
            //if (!_screenRect.Contains(eventData.position))
            //{
            //        DropMoving(eventData);
            //}
            //else
            //{
                _currentSwipe.UpdateState(eventData);
                GameService.ActiveNow?.InvokeTouchEvent(TouchEvent.OnDrag, _currentSwipe);
            //}
        }

   
     
    
        public void OnPointerDown(PointerEventData eventData)
        {
            
            //RaycastHit[] rayHitArray = Physics.RaycastAll(Camera.main.ScreenPointToRay(eventData.position));



            _currentSwipe.screenPoint_start = eventData.position;


            if (_currentSwipe.swipeID == nil )
            {
                  _currentSwipe.swipeID = eventData.pointerId;
                  _currentSwipe.beginTime = Time.unscaledTime;
                            
                  _currentSwipe.isFinished = false;
                  _currentSwipe.UpdateState(eventData);
            }
            GameService.ActiveNow?.InvokeTouchEvent(TouchEvent.OnPointerDown, _currentSwipe);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
                GameService.ActiveNow?.InvokeTouchEvent(TouchEvent.OnPointerUp, _currentSwipe);
                DropMoving(eventData);                
                
        }
        private void DropMoving(PointerEventData eventData)
        {
            if (_currentSwipe.isFinished)
            {
                return;
            }
            _currentSwipe.UpdateState(eventData);
            _currentSwipe.swipeID = nil;
            _currentSwipe.isFinished = true;

            GameService.ActiveNow?.InvokeTouchEvent(TouchEvent.OnDrop, _currentSwipe);
            
          
        }
       
    }

    [Serializable]
    public class BoolEvent: UnityEvent<bool>
    {     
    }
    public class TouchEventResponse
    {
        private Action<Swipe> Response;

        public TouchEventResponse(Action<Swipe> response)
        {
            Response = response;
        }
        public void Invoke(Swipe content)
        {
            Response.Invoke(content);
        }
    }
}
