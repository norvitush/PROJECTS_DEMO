using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VOrb
{

    public class TouchRegistrator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler 
    {
        const int NIL = -999;
        private UIElement _tapGoal = null;

        private Swipe _currentSwipe;
        private Rect _screenRect = Rect.zero;
        private Vector2 CamMin,CamMax;

        public UIElement TapGoal { get => _tapGoal; set => _tapGoal = value; }

        private void Start()
        {
            //поучаем размеры экрана в Worlds координатах            
            CamMin = Camera.main.ViewportToScreenPoint(new Vector3(0,0,0));
            CamMax = Camera.main.ViewportToScreenPoint(new Vector3(1, 1, 0));
            _screenRect = new Rect(CamMin.x, CamMin.y, CamMax.x, CamMax.y);
            _currentSwipe = new Swipe();
            _currentSwipe.swipeID = NIL;
            _currentSwipe.isFinished = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_screenRect.Contains(eventData.position))
            {
                    DropMoving(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {

            if (UIWindowsManager.ActiveNow is MainWindow)
            {
                if (GameService.Instance.GameStarted == false)
                {
                    MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
                    mn.MainMessage.gameObject.SetActive(false);
                    GameService.Instance.StartGame();
                    return;
                }
                else
                {
                    if (!GameService.Instance.PlayerIsStunned)
                    {
                        DropMoving(eventData);
                    }
                }
                    
            }
            else
            {
                DropMoving(eventData);
            }
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            RaycastHit[] rayHitArray = Physics.RaycastAll(Camera.main.ScreenPointToRay(eventData.position));
            TapGoal = UIEffects.GetUIElementFrom(rayHitArray);
            
            _currentSwipe.screenPoint_start = eventData.position;

            if (_currentSwipe.swipeID == NIL )
            {
                  _currentSwipe.swipeID = eventData.pointerId;
                  _currentSwipe.beginTime = Time.time;
                            
                  _currentSwipe.isFinished = false;
                  _currentSwipe.UpdateState(eventData);
            }
        }

        private void DropMoving(PointerEventData eventData)
        {
            if (_currentSwipe.isFinished)
            {
                return;
            }
            _currentSwipe.UpdateState(eventData);
            _currentSwipe.swipeID = NIL;
            _currentSwipe.isFinished = true;
                if (_currentSwipe.isGoodSwipe()&&_currentSwipe.direction.y<0)
                {
                    UIWindowsManager.ActiveNow.SvipeEvent.Invoke();                    
                }
                else
                {
                    UIWindowsManager.ActiveNow.TapEvent.Invoke();
                }
        }
    }

    [Serializable]
    public class BoolEvent: UnityEvent<bool>
    {     
    }
}
