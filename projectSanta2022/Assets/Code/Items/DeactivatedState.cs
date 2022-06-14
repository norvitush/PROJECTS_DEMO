using System;

namespace VOrb.CubesWar
{
    [Serializable]
    class DeactivatedState : IGiftState
    {
        public DeactivatedState()
        {
            this.name = "DeactivatedState";
        }

        public string name { get; set; }

        public IGiftState Transaction(GiftBehaviour gift, IGiftState NextState) 
        {
            return gift.State;

        }
    }
}
