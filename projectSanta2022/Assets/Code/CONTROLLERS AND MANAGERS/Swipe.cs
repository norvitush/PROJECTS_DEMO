using UnityEngine;
using UnityEngine.EventSystems;

namespace VOrb.CubesWar
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
}
