using System;

namespace VOrb.CubesWar
{
    [Serializable]
    class StartState : IGiftState 
    {
        public StartState()
        {
            this.name = "StartCubeState";
        }
      
        public string name { get; set; }

        public IGiftState Transaction(GiftBehaviour gift,  IGiftState NextState = null)
        {
            if (NextState is ThrowState)
            {
                return NextState;
            }
            else
            {
                return gift.State;
            }
        }
    }
    
}
