using GoldenSoft.Column21.Core;
using System;

public interface ICardManager
{
    public bool TryRegisterPlayerPickUpCard(
        out (Type ownerType, int index) newOwner,
        ICardSlot currentOwner,
        Card card);
    
    public bool TryRegisterPlayerPutDownCard(
        ICardSlot fromOwner,
        ICardSlot toOwner,
        Card card, int index);
}