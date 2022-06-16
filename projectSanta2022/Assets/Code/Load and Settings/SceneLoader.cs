
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timers;
using VOrb.CubesWar;
using VOrb;
using System;
//using GooglePlayGames;
//using UnityEngine.SocialPlatforms;
//using GooglePlayGames.BasicApi;

public class SceneLoader : Singlton<SceneLoader>
{

    private static SceneSettings _sceneSettings;

    private UIWindowsManager _windowController;
    [SerializeField] private Image _fadeScreen;

    public static SceneSettings SceneSettings { get => _sceneSettings; private set => _sceneSettings = value; }
    public Image FadeScreen => _fadeScreen; 

    protected override void Init()
    {
        base.Init();        

        //Получение игровых настроек
        SceneSettings = GetComponent<SceneSettings>();
      
        //Менеджер окон
        _windowController = GetComponent<UIWindowsManager>();

        //Создание игрового менеджера для получения всех объектов
        GameObject GameManager = Instantiate(Resources.Load("prefabs/GLOB_GameService", typeof(GameObject))) as GameObject;             
        GameManager.SetActive(true);
        GameObject sensor = Instantiate(Resources.Load("prefabs/UITouchPanel", typeof(GameObject)),
                            SceneSettings.TopUISensor.transform) as GameObject; 

        //Создаем и записываем Игровому менеджеру инфу о сенсоре экрана
        GameService.Instance.Sensor = sensor.GetComponent<TouchRegistrator>();
        GameService.Instance.Sensor.SetDependences(GameService.Instance);
        GameManager.name = "GLOB_GameService";

        //Создаем локальную базу данных
        GameObject DataManager = Instantiate(Resources.Load("prefabs/GLOB_DBManager", typeof(GameObject))) as GameObject;
        DataManager.SetActive(true);
        DataManager.name = "GLOB_DataBase";

        ////Пул объектов
        GameObject PoolManagerObject = Instantiate(Resources.Load("prefabs/GLOB_ObjectsPOOL", typeof(GameObject))) as GameObject;           
        ObjectPoolManager PoolManager = PoolManagerObject.GetComponent<ObjectPoolManager>();


        PoolManager.funcForInvoke.AddListener(InitAfterCreatePool);

        PoolManager.AllPools.Add(new ObjectPool(PooledObjectType.Gift, SceneSettings.GiftPrefab, 10, 
                              SceneSettings.ParentForPoolOfGifts));

        PoolManager.AllPools.Add(new ObjectPool(PooledObjectType.Brick, SceneSettings.StonePrefab, 5,
                      SceneSettings.ParentForPoolOfGifts));


        int minTypeNum = (int)PooledObjectType.NumberPopup1_smile;
        for (int i = 0; i < SceneSettings.PopupsSmilesPrefabs.Length; i++)
        {
            PoolManager.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, SceneSettings.PopupsSmilesPrefabs[i], 5,
                      SceneSettings.UIEffectsContainer));
        }

        minTypeNum = (int)PooledObjectType.Homes1;
        for (int i = 0; i < SceneSettings.HomesPrefab.Length; i++)
        {
            PoolManager.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, SceneSettings.HomesPrefab[i], 5,
                      SceneSettings.ParentForPoolOfGifts));
        }
        minTypeNum = (int)PooledObjectType.Envir1;
        for (int i = 0; i < SceneSettings.EnvirPrefab.Length; i++)
        {
            PoolManager.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, SceneSettings.EnvirPrefab[i], 10,
                      SceneSettings.ParentForPoolOfGifts));
        }


        PoolManagerObject.SetActive(true);
        PoolManagerObject.name = "GLOB_ObjectPool";        

    }

    public void InitAfterCreatePool()
    {
        StartCoroutine(nameof(FadeOut));

        ////Продолжаем загрузку после инициализации
        GameService.Instance.ReceiveContainers();
        GameService.Instance.LoadFields();              
        _windowController.enabled = true;

        Destroy(SceneSettings.GiftPrefab);
   

        GameObject TimerManager = new GameObject("GLOB_GameTimer", typeof(TimersManager));
        TimersManager.SetTimer(TimerManager, 1f, uint.MaxValue, GameService.Instance.OnTimerTick);
        GameService.Instance.SetActiveLvlTimer(false);


        ////Сид рандома

        DateTime date = DateTime.Now;
        string cSeed = date.Day.ToString().PadLeft(2, '0') + date.Hour.ToString().PadLeft(2, '0') + date.Minute.ToString().PadLeft(2, '0') + date.Millisecond.ToString().PadLeft(4, '0');
        long lSeed = long.Parse(cSeed) / Mathf.Clamp(date.Minute, 3, 24);
        UnityEngine.Random.InitState((int)lSeed);
        ////Инициализация Коллекции игрока
        ///
        GameService.Instance.GlobalScore = 0;

        if (SceneSettings.DropAccountState)
        {
            GameStorageOperator.DropSavedPlayerInfo();
            Debug.Log("Данные игрока очищены!");
            UIWindowsManager.GetWindow<MainWindow>().StarsPath.DropAllProgress();
        }
        else
        {
            Debug.Log(SceneSettings.PlayerName + SystemInfo.deviceUniqueIdentifier);
            string player = SceneSettings.PlayerName + SystemInfo.deviceUniqueIdentifier;
            string loaded = (string)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Player, "");
            if (loaded == "")
            {
                Debug.Log("НЕ СОХРАНЁН");

                //СОХРАНЯЕМ
                GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Player, player);
                UIWindowsManager.GetWindow<MainWindow>().StarsPath.DropAllProgress();
            }
            else
            {

                if (loaded == player)
                {
                    //ЗАГРУЖАЕМ
                    UpdateFromStorage();
                }
                else
                {
                    Debug.Log("!!! НЕ совпадает с текущим ");
                    GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Player, player);
                    UIWindowsManager.GetWindow<MainWindow>().StarsPath.DropAllProgress();
                }
            }
        }
        



    }
    public static void UpdateFromStorage()
    {
        GameService.Instance.NoAds =(int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Noads, 0);

        GameService.Instance.Sounds = (bool)(SafeInt)(int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Sound, 1);
       
        GameService.Instance.GlobalScore = (int)GameStorageOperator.GetFromDevice(GameStorageOperator.PlayerParamNames.Smiles, 0);
        UIWindowsManager.GetWindow<MainWindow>().StarsPath.UpdateAllLevelsFromStorage();
        UIWindowsManager.GetWindow<StartWindow>().NoAdsShow = (GameService.Instance.NoAds == 0);
    }
    IEnumerator FadeOut()
    {
        FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, 1f);
        while (FadeScreen.color.a>0.05f)
        {
            yield return new WaitForFixedUpdate();
            FadeScreen.color = new Color(FadeScreen.color.r, FadeScreen.color.g, FadeScreen.color.b, FadeScreen.color.a - Time.deltaTime);
        }
        FadeScreen.transform.gameObject.SetActive(false);
    }
   


}

