﻿using UnityEngine.Events;

namespace VOrb.SantaJam
{
    public interface ITouchSensetive
    {
        UnityEvent TapEvent { get; set; }
        UnityEvent SvipeEvent { get; set; }
        void InvokeTouchEvent(TouchEvent updateType, Swipe cont);
        TouchEventResponse[] responses { get; set; }
    }
}
