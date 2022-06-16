using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VOrb.CubesWar
{

    public class TouchRegistrator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Swipe _currentSwipe;

        public Vector2 CamMin { get; private set; }
        public Vector2 CamMax { get; private set; }

        public float Width => CamMax.x - CamMin.x;
        public float Height => CamMax.y - CamMin.y;

        private IGameManager _gameManager;
        private ITouchSensetive _activeNow => _gameManager.ActiveNow;
        public void SetDependences(IGameManager gameManager) => _gameManager = gameManager;

        private void Start()
        {            
            
            CamMin = Camera.main.ViewportToScreenPoint(new Vector2(0,0));
            CamMax = Camera.main.ViewportToScreenPoint(new Vector2(1, 1));

            _currentSwipe = new Swipe
            {
                swipeID = JoystykBaseConst.NO_ID,
                isFinished = true
            };

        }

        public void OnDrag(PointerEventData eventData)
        {
           _currentSwipe.UpdateState(eventData);
           _activeNow?.InvokeTouchEvent(TouchEvent.OnDrag, _currentSwipe);
        }

   
     
    
        public void OnPointerDown(PointerEventData eventData)
        {
            _currentSwipe.screenPoint_start = eventData.position;

            if (_currentSwipe.swipeID == JoystykBaseConst.NO_ID )
            {
                  _currentSwipe.swipeID = eventData.pointerId;
                  _currentSwipe.beginTime = Time.unscaledTime;
                            
                  _currentSwipe.isFinished = false;
                  _currentSwipe.UpdateState(eventData);
            }
            _activeNow?.InvokeTouchEvent(TouchEvent.OnPointerDown, _currentSwipe);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
                _activeNow?.InvokeTouchEvent(TouchEvent.OnPointerUp, _currentSwipe);
                DropMoving(eventData);                
                
        }
        private void DropMoving(PointerEventData eventData)
        {
            if (_currentSwipe.isFinished)
            {
                return;
            }
            _currentSwipe.UpdateState(eventData);
            _currentSwipe.swipeID = JoystykBaseConst.NO_ID;
            _currentSwipe.isFinished = true;

            _activeNow?.InvokeTouchEvent(TouchEvent.OnDrop, _currentSwipe);
            
          
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
