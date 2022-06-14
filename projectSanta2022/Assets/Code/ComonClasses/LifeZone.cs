using UnityEngine;

namespace VOrb
{
    public class LifeZone
    {
        Rect ZoneRect;       
        public float Left { get => ZoneRect.xMin; private set => ZoneRect.xMin = value; }
        public float Top { get => ZoneRect.yMax; private set => ZoneRect.yMax = value; }
        public float Right { get => ZoneRect.xMax; private set => ZoneRect.xMax = value; }
        public float Bottom { get => ZoneRect.yMin; private set => ZoneRect.yMin = value; }
        public static LifeZone Zero()
        {
            return new LifeZone();
        }

        public LifeZone()
        {
            this.SetLifeZone();
        }
        public LifeZone(float left, float bottom , float right , float top)
        {
            this.SetLifeZone(left, bottom, right, top);
        }
        public LifeZone(Vector2 LeftBottomBorder, Vector2 RightTopBorder)
        {
            this.SetLifeZone(LeftBottomBorder.x , LeftBottomBorder.y, RightTopBorder.x, RightTopBorder.y);
        }
        public bool isEmpty()
        {
            if (Left >= 0 || Top >= 0 || Right >= 0 || Bottom >= 0)
                return false;
            else
                return true;
        }
        public bool Contains(Vector2 point, float delta = 0f)
        {
            if (this.isEmpty())
            {
                Debug.Log("Contains: EMPTY");
                return true;
            }

            Rect tempRect = ZoneRect;
            if (Left < 0) tempRect.xMin = point.x - 10;
            if (Right < 0) tempRect.xMax = point.x + 10;
            if (Top < 0) tempRect.yMax = point.y + 10;
            if (Bottom < 0) tempRect.yMin = point.y - 10;

            if (delta==0)
            {               
                if (tempRect.Contains(point))
                    return true;
                else
                    return false;
            }
            else
            {
                Vector2 pos1 = new Vector2(tempRect.xMin - delta, tempRect.yMin - delta);
                Vector2 pos2 = new Vector2(tempRect.xMax + delta, tempRect.yMax + delta);
                Rect tmpRect = new Rect(pos1, pos2);
               
                if (tmpRect.Contains(point))
                    return true;
                else
                    return false;
            }

        }
        public void SetEmpty()
        {
            SetLifeZone();
        }

        ///<summary>
        ///Устанавливает края зоны, -1 означает что границы нет и функция Contains
        ///будет считать такую границу - unlimite;
        ///</summary>
        public void SetLifeZone(float left = -1, float bottom = -1,float right = -1 ,float top = -1 )
        {            
            Left = left;
            Right = right;
            Bottom = bottom;
            Top = top;
        }
        public void SetLifeZone(LifeZone zone)
        {
            Left = zone.Left;
            Right = zone.Right;
            Bottom = zone.Bottom;
            Top = zone.Top;
        }
        public override string ToString()
        {
            string result;
            result = "Zone(l,b,r,t): (" + Left.ToString() + ", " + Bottom.ToString() + ", " + Right.ToString() + ", " + Top.ToString() + ") ";
            return result;
        }

    }
}