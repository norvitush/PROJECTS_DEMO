using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

public class SceneSettings : MonoBehaviour
{
    [Header("ИГРОВЫЕ УСТАНОВКИ")]
    public GameObject TopUIContainer;
    //кулдаун появления следующего кубика
    public float coolDown = 1.5f;

    public GameObject BulletCubePrefab;
    public Transform parent_forPoolOfCubs;

    [Tooltip("Test mode!")]
    public bool IsTestMode = true;

    [Tooltip("Сброс сохранённых данных")]
    public bool DropAccountState = false;

    [Tooltip("Имя игрока")]
    public string PlayerName = "";

    public GameObject PopupTextPrefab;
    public Transform parent_forPopupTextPrefab;

    public GameObject[] NubersPopupTextPrefabs;
    

    public GameObject[] HomesPrefab;
    public GameObject[] EnvirPrefab;
    public GameObject StonePrefab;
    public Texture2D GrayTexture;


    [Header("поведение кубика")]
    public Vector3 baseCubeRotation = new Vector3(3.16f, -19.46f, -19.21f);
    public Vector3 baseCubeScal = new Vector3(1.85f, 1.85f, 1.85f);
    public Vector3 baseHomeScal = new Vector3(22f, 22f, 22f);

    public int StoneShowLevel = 5;
} 

