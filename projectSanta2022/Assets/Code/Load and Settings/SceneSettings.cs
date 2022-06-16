using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

public class SceneSettings : MonoBehaviour
{
    [Header("ИГРОВЫЕ УСТАНОВКИ")]
    [SerializeField] private GameObject _topUISensor;

    //кулдаун появления следующего подарка
    [SerializeField] private float _coolDown = 0.5f;

    [SerializeField] private GameObject _giftPrefab;
    [SerializeField] private Transform _parentForPoolOfGifts;

    [Tooltip("Test mode!")]
    [SerializeField] private bool _isTestMode = false;

    [Tooltip("Сброс сохранённых данных")]
    [SerializeField]  private bool _dropAccountState = false;

    [Tooltip("Имя игрока")]
    public string PlayerName = "Cesar";

    [SerializeField] private Transform _uiEffectsContainer;

    [SerializeField] private GameObject[] _popupsSmilesPrefabs;


    [SerializeField] private GameObject[] _homesPrefab;
    [SerializeField] private GameObject[] _envirPrefab;
    [SerializeField] private GameObject _stonePrefab;
    [SerializeField] private Texture2D _grayTexture;


    [Header("Базовые установки коробки подарка")]
    [SerializeField] private Vector3 _baseGiftScal = new Vector3(0.8f, 0.8f, 0.8f);

    [SerializeField] private int _brickShowLevel = 5;

    public GameObject TopUISensor  => _topUISensor;
    public float CoolDown  => _coolDown;
    public GameObject GiftPrefab => _giftPrefab;
    public Transform ParentForPoolOfGifts { get => _parentForPoolOfGifts; set => _parentForPoolOfGifts = value; }
    public bool IsTestMode { get => _isTestMode; set => _isTestMode = value; }
    public bool DropAccountState  => _dropAccountState;
    public Transform UIEffectsContainer => _uiEffectsContainer;
    public GameObject[] PopupsSmilesPrefabs  => _popupsSmilesPrefabs;
    public GameObject[] HomesPrefab  => _homesPrefab; 
    public GameObject[] EnvirPrefab => _envirPrefab;
    public GameObject StonePrefab  => _stonePrefab; 
    public Texture2D GrayTexture  => _grayTexture;
    public Vector3 BaseGiftScal  => _baseGiftScal;
    public int BrickShowLevel  => _brickShowLevel; 
} 

