using System.Collections;
using UnityEngine;


namespace VOrb.SantaJam
{
    public static class EventPublisher
    {
        public class JoystikMoveEvent : PublishEvent<Vector3, float> { }
        public class JoystikDirectionSelectedEvent : PublishEvent<Vector3, float> { }
        public class JoystikFirstTapEvent : PublishEvent<Vector2> { }
        public class GiftShootEvent : PublishEvent<GameObject, int> { }

        public static JoystikMoveEvent JostikMovement = new JoystikMoveEvent();
        public static JoystikDirectionSelectedEvent JoystikUp = new JoystikDirectionSelectedEvent();
        public static GiftShootEvent onShoot = new GiftShootEvent();
        public static JoystikFirstTapEvent JoysticFirstTap = new JoystikFirstTapEvent();
    }

   

}