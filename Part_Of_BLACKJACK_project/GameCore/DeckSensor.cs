using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckSensor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] DeckSlot deckSlot;
    public void OnPointerDown(PointerEventData eventData)
    {
        deckSlot.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        deckSlot.OnPointerUp(eventData);
    }
}

