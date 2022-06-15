using UnityEngine;
using UnityEngine.EventSystems;

namespace VOrb
{
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
            endTime = Time.time;
        }
        public bool isGoodSwipe()//float timeLimit)
        {
            //float timePeriod = GetTimePeriod();
            //return ((timePeriod < timeLimit)&&(distance>0.1f));
            return distance > 100f;
        }
        public float GetTimePeriod()
        {
            return (endTime - beginTime);
        }
        public Vector2 EndPoint()
        {
            return screenPoint_end;
        }
        public Vector2 StartPoint()
        {
            return screenPoint_start;
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
}
