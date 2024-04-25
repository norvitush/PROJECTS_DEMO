namespace GoldenSoft.Column21.Core
{
    [System.Flags]
    public enum CardsSet : uint
    {
        None = 0, Ace = 1, Two = 2, Three = 4, Four = 8, Five = 16, Six = 32, Seven = 64, Eight = 128, Nine = 256, Ten = 512, Jack = 1024, Queen = 2048, King = 4096
    }

    [System.Serializable]
    public enum CardType : uint
    {
        Ace = 0, Two = 1, Three = 2, Four = 3, Five = 4, Six = 5, Seven = 6, Eight = 7, Nine = 8, Ten = 9, Jack = 10, Queen = 11, King = 12, None = 999
    }


}
