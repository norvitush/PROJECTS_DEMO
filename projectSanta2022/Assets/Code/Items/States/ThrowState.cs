using System;
using UnityEngine;

namespace VOrb.SantaJam
{
    [Serializable]
    class ThrowState : IGiftState
    {
        public ThrowState()
        {
            this.name = "ThrowState";
        }

        public string name { get; set; }

        public IGiftState Transaction(GiftBehaviour gift, IGiftState NextState)
        {

            if (NextState is DeactivatedState)
            {
                return NextState;
            }
            else
            {
                Debug.Log("state error!");
                return gift.State;
            }
        }
    }

}
