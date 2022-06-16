using UnityEngine;

namespace VOrb.CubesWar
{
    [System.Serializable]
    public struct JoystykData
    {
        public Vector2 Direction;
        public float Angle;
        public float Power;
        public float Speed;
        public int Horizontal;
        public int Vertical;
        public bool DirectionChoosed;
    }
} 