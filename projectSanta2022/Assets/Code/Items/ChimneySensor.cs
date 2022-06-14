using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChimneySensor : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private List<ChimneyState> _statesData;
    [SerializeField] private int _houseNumber;
    [SerializeField] private TextMeshPro _houseNumberView;
    public void ShowNumber() => _houseNumberView.transform.parent.gameObject.SetActive(true);
    public void HideNumber() => _houseNumberView.transform.parent.gameObject.SetActive(false);
    public bool isNumberOn => _houseNumberView.transform.parent.gameObject.activeInHierarchy;
    public int HouseNumber { get => _houseNumber; 
                             set { _houseNumber = value; _houseNumberView.text = value.ToString(); } }
    public List<ChimneyState> StatesData => _statesData;
    public Vector3 ColliderPosition => _collider.bounds.center;

    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void TurnOff() { if (_collider != null) _collider.enabled = false; }
    public void TurnOn() { if (_collider != null) _collider.enabled = true; }

}
