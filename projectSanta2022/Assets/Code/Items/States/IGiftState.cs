namespace VOrb.SantaJam
{
    public interface IGiftState
    {
        string name { get;  set; }

        IGiftState Transaction(GiftBehaviour cube, IGiftState NextState);
    }

}
