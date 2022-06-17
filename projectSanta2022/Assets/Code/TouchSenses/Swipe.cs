using UnityEngine;
using UnityEngine.EventSystems;

namespace VOrb.CubesWar
{
    public struct Swipe
    {
        private int _swipeID;
        private float _beginTime;
        private float _endTime;
        private float _distance;
        private bool _isFinished;
        public Vector2 screenPoint_start;
        public Vector2 screenPoint_end;
        public Vector2 _direction;

        public int SwipeID { get => _swipeID; set => _swipeID = value; }
        public float BeginTime { get => _beginTime; set => _beginTime = value; }
        public bool IsFinished { get => _isFinished; set => _isFinished = value; }

        public void UpdateState(PointerEventData eventData)
        {
            screenPoint_end = eventData.position;
            _direction = screenPoint_end - screenPoint_start;
            _distance = _direction.magnitude;
            _direction = _direction.normalized;
            _endTime = Time.unscaledTime;
        }
       
        public float GetTimePeriod()
        {
            return (_endTime - BeginTime);
        }
        
        public override string ToString()
        {
            string outp = "";
            outp += "ID: " + SwipeID + " |" + "Start: " + screenPoint_start + " |";
            outp += "End: " + screenPoint_end + " |" + "DeltaTime: e-b   " + _endTime + "-" + BeginTime + "= "+ GetTimePeriod().ToString() + "    |";
            outp += "Direction: " + _direction + " |" + "Distance: " + _distance + " |" + "Swipe closed: " + IsFinished + " |";
            return outp;
        }
    }}
