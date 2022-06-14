
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timers;
using VOrb.CubesWar;
using VOrb;
using System.Linq;
using System;
//using GooglePlayGames;
//using UnityEngine.SocialPlatforms;
//using GooglePlayGames.BasicApi;

public class SceneLoader : Singlton<SceneLoader>
{

    public FacebookManager FB_manager = null;
    public static SceneSettings sceneSettings;
    
    private UIWindowsManager WindowController;     
    public Image FadeScreen;

    protected override void Init()
    {
        base.Init();        

        //Получение игровых настроек
        sceneSettings = GetComponent<SceneSettings>();
      
        //Менеджер окон
        WindowController = GetComponent<UIWindowsManager>();

        //Создание игрового менеджера для получения всех объектов
        GameObject GameManager = Instantiate(Resources.Load("prefabs/GLOB_GameService", typeof(GameObject))) as GameObject;             
        GameManager.SetActive(true);
        GameObject sensor = Instantiate(Resources.Load("prefabs/UITouchPanel", typeof(GameObject)),
                            sceneSettings.TopUIContainer.transform) as GameObject; 

        //Создаем и записываем Игровому менеджеру инфу о сенсоре экрана
        GameService.Instance.Sensor = sensor.GetComponent<TouchRegistrator>();
        GameManager.name = "GLOB_GameService";

        //Создаем локальную базу данных
        GameObject DataManager = Instantiate(Resources.Load("prefabs/GLOB_DBManager", typeof(GameObject))) as GameObject;
        DataManager.SetActive(true);
        DataManager.name = "GLOB_DataBase";

        ////Пул объектов
        GameObject objPool1 = Instantiate(Resources.Load("prefabs/GLOB_ObjectsPOOL", typeof(GameObject))) as GameObject;           
        ObjectPoolManager CubsPool = objPool1.GetComponent<ObjectPoolManager>();


        CubsPool.funcForInvoke.AddListener(InitAfterCreatePool);

        CubsPool.AllPools.Add(new ObjectPool(PooledObjectType.Gift, sceneSettings.BulletCubePrefab, 10, 
                              sceneSettings.parent_forPoolOfCubs));

        CubsPool.AllPools.Add(new ObjectPool(PooledObjectType.Brick, sceneSettings.StonePrefab, 5,
                      sceneSettings.parent_forPoolOfCubs));


        int minTypeNum = (int)PooledObjectType.NumberPopup1_smile;
        for (int i = 0; i < sceneSettings.NubersPopupTextPrefabs.Length; i++)
        {
            CubsPool.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, sceneSettings.NubersPopupTextPrefabs[i], 5,
                      sceneSettings.parent_forPopupTextPrefab));
        }

        minTypeNum = (int)PooledObjectType.Homes1;
        for (int i = 0; i < sceneSettings.HomesPrefab.Length; i++)
        {
            CubsPool.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, sceneSettings.HomesPrefab[i], 5,
                      sceneSettings.parent_forPoolOfCubs));
        }
        minTypeNum = (int)PooledObjectType.Envir1;
        for (int i = 0; i < sceneSettings.EnvirPrefab.Length; i++)
        {
            CubsPool.AllPools.Add(new ObjectPool((PooledObjectType)minTypeNum + i, sceneSettings.EnvirPrefab[i], 10,
                      sceneSettings.parent_forPoolOfCubs));
        }


        objPool1.SetActive(true);
        objPool1.name = "GLOB_ObjectPool";        

    }

    public void InitAfterCreatePool()
    {
        StartCoroutine(nameof(FadeOut));

        ////Продолжаем загрузку после инициализации
        GameService.Instance.ReceiveContainers();
        GameService.Instance.LoadFields();              
        WindowController.enabled = true;

        Destroy(sceneSettings.BulletCubePrefab);
   

        GameObject TimerManager = new GameObject("GLOB_GameTimer", typeof(TimersManager));
        GameService.Instance.timeManager = TimerManager.GetComponent<TimersManager>();
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

        if (sceneSettings.DropAccountState)
        {
            GameStorageOperator.DropSavedPlayerInfo();
            Debug.Log("Данные игрока очищены!");
            UIWindowsManager.GetWindow<MainWindow>().StarsPath.DropAllProgress();
        }
        else
        {
            Debug.Log(sceneSettings.PlayerName + SystemInfo.deviceUniqueIdentifier);
            string player = sceneSettings.PlayerName + SystemInfo.deviceUniqueIdentifier;
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

