using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;
public class SceneSettings : MonoBehaviour
{
    [Header("ИГРОВЫЕ УСТАНОВКИ")]

    [Tooltip("DEBUG mode!")]
    [SerializeField]private bool _isDebugMode = false;
    
    [Tooltip("Test mode!")]
    [SerializeField] private bool _isTestMode = false;
   
    [Tooltip("Сброс сохранённых данных")]
    [SerializeField] private bool _dropAccountState = false;

    [Tooltip("Имя игрока")]
    [SerializeField] private string _playerName = "FirstPlayer";

    [SerializeField] private SkinView _defaultSkin;

    [Tooltip("Задержка удаления обрубка"), Range(0, 10)]
    [SerializeField] private float _destroyTime = 6f;
    

    [Tooltip("Через сколько можно снова порезаться"), Range(0, 5)]
    [SerializeField] private float _delayTime = 1.31f;

    [Tooltip("Предельный размер проигрыша в %"), Range(0f, 100f)]
    [SerializeField] private float _sizeLim = 63.9f;


    [Tooltip("Максимум прыжков")]
    [SerializeField] private int _jumpLimit = 2;

    [Tooltip("Время щита")]
    [SerializeField] private float _shieldTime = 10f;

    [Header("Надписи при Старте:")]
    [SerializeField] private string _beginText = "TAP FOR JUMP OR DIE !!!";

    [SerializeField] private List<string> _endTextVariants = new List<string>();

    [Header("Объекты для создания массива")]   
    
    [SerializeField] private GameObject _pooledSplashText_object;
    [SerializeField] private GameObject _pooledScoreText_object;
    [SerializeField] private Transform _parent_forPoolSplashes;
    [SerializeField] private Transform _parent_forPoolScore;
    [SerializeField] private GameObject _pooledFlowUpText_object;
    [SerializeField] private Transform _parent_forFlowUp;
    [SerializeField] private TouchRegistrator _sensor; 

    [Header("Точные настройки")]

    [Tooltip("Частота появления перелетающего текста при подсчёте очков"), Range(0.1f, 0.5f)]
    [SerializeField] private float _frecuence = 0.1f;

    [Tooltip("На этот коэф умножается высота экрана (0.5 - половина экрана)"), Range(0.4f, 0.99f)]
    [SerializeField] private float _yTopBonusOffset = 0.706f;

    [Tooltip("Время до минимального появления нового предмета"), Range(1f, 10f)]
    [SerializeField] private float _newItemDelay = 4.72f;

    [Tooltip("Ускорение скакалки"), Range(1f, 3f)]
    [SerializeField] private float _jumpieVelocity = 1.063f;

    public bool IsDebugMode => _isDebugMode;
    public bool IsTestMode => _isTestMode;
    public bool DropAccountState => _dropAccountState;
    public string PlayerName => _playerName;
    public SkinView DefaultSkin => _defaultSkin;
    public float DestroyTime => _destroyTime;
    public float DelayTime => _delayTime;
    public float SizeLim => _sizeLim;
    public int JumpLimit => _jumpLimit;
    public float ShieldTime => _shieldTime;
    public string BeginText { get => _beginText; set => _beginText = value; }
    public List<string> EndTextVariants => _endTextVariants;
    public GameObject PooledSplashText_object => _pooledSplashText_object;
    public GameObject PooledScoreText_object => _pooledScoreText_object;
    public Transform Parent_forPoolSplashes => _parent_forPoolSplashes;
    public Transform Parent_forPoolScore => _parent_forPoolScore;
    public GameObject PooledFlowUpText_object => _pooledFlowUpText_object;
    public Transform Parent_forFlowUp => _parent_forFlowUp;
    public TouchRegistrator Sensor => _sensor;
    public float Frecuence => _frecuence;
    public float YTopBonusOffset => _yTopBonusOffset;
    public float NewItemDelay => _newItemDelay;
    public float JumpieVelocity => _jumpieVelocity;
}
