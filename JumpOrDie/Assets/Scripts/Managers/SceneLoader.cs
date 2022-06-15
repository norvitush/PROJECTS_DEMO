
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Timers;
using VOrb;

public partial class SceneLoader : Singlton<SceneLoader>
{

    public static SceneSettings sceneSettings;
    
    [SerializeField]private Image _fadeScreen;
    private UIWindowsManager _windowController;     
    
    protected override void Init()
    {
        base.Init();
        
        //Получение игровых настроек
        sceneSettings = GetComponent<SceneSettings>();        

        //Менеджер окон
        _windowController = GetComponent<UIWindowsManager>();

        //Создание игрового менеджера
        GameObject GameManager = Instantiate(Resources.Load("prefabs/GLOB_GameService", typeof(GameObject))) as GameObject;             
        GameManager.SetActive(true);

        //проброс зависимости сенсора экрана игровому менеджеру
        GameService.Instance.Sensor = sceneSettings.Sensor;
        
        //Создаем локальную базу данных
        GameObject DataManager = Instantiate(Resources.Load("prefabs/GLOB_DBManager", typeof(GameObject))) as GameObject;        
        DataManager.SetActive(true);
        DataManager.name = "GLOB_DataBase";

        //Создаем систему работы с игровыми предметами
        GameObject CollectingSystem = Instantiate(Resources.Load("prefabs/GLOB_CollectingSystem", typeof(GameObject))) as GameObject;
        CollectingSystem.SetActive(true);
        CollectingSystem.name = "GLOB_CollectingSystem";

        //Пул объектов
        GameObject objPool1 = Instantiate(Resources.Load("prefabs/GLOB_ObjectsPOOL", typeof(GameObject))) as GameObject;           
        GameObjectPool PoolManager = objPool1.GetComponent<GameObjectPool>();

        PoolManager.AllPools.Add(new NamedPoolList("SplashText", sceneSettings.PooledSplashText_object, 5, 
                              sceneSettings.Parent_forPoolSplashes));
        PoolManager.AllPools.Add(new NamedPoolList("ScoreFlowText", sceneSettings.PooledScoreText_object, 15, 
                              sceneSettings.Parent_forPoolScore));
        PoolManager.AllPools.Add(new NamedPoolList("FlowUpText", sceneSettings.PooledFlowUpText_object, 5,
                      sceneSettings.Parent_forFlowUp));

        PoolManager.readyCallback.AddListener(InitAfterCreatePool);

        objPool1.SetActive(true);
        objPool1.name = "GLOB_ObjectPool";
      
    }

    
    public void InitAfterCreatePool()
    {        
        StartCoroutine(nameof(FadeOut));

        //Продолжаем загрузку после инициализации пока анимируется FadeOut
        GameService.Instance.ReceiveContainers();
        GameService.Instance.LoadFields();              
        _windowController.enabled = true;
        Destroy(sceneSettings.PooledSplashText_object);
        Destroy(sceneSettings.PooledScoreText_object);
        Destroy(sceneSettings.PooledFlowUpText_object);
        GameObject TimerManager = new GameObject("GLOB_GameTimer", typeof(TimersManager));
        GameService.Instance.TimeManager = TimerManager.GetComponent<TimersManager>();
        TimersManager.SetTimer(TimerManager, 0.1f, uint.MaxValue, GameService.Instance.OnTimerTick);
        TimersManager.SetTimer(TimerManager, 1f, uint.MaxValue, GameService.Instance.SpeedTimer);
        GameService.Instance.SetActiveLvlTimer(false);

        //Сид рандома
        System.DateTime date1 = System.DateTime.Now;
        string cSeed = date1.Day.ToString().PadLeft(2, '0') + date1.Hour.ToString().PadLeft(2, '0') + date1.Minute.ToString().PadLeft(2, '0') + date1.Millisecond.ToString().PadLeft(4, '0');
        long lSeed = long.Parse(cSeed)/Mathf.Clamp(date1.Minute,3,24);
        Random.InitState((int)lSeed);

        //Инициализация Коллекции игрока
        CollectingSystem.Instance.InitPlayerCollection(sceneSettings.PlayerName);        
    }

    IEnumerator FadeOut()
    {
        _fadeScreen.color = new Color(_fadeScreen.color.r, _fadeScreen.color.g, _fadeScreen.color.b, 1f);
        while (_fadeScreen.color.a>0.05f)
        {
            yield return new WaitForFixedUpdate();
            _fadeScreen.color = new Color(_fadeScreen.color.r, _fadeScreen.color.g, _fadeScreen.color.b, _fadeScreen.color.a - Time.deltaTime);
        }
        _fadeScreen.transform.parent.gameObject.SetActive(false);
    }
   

}

