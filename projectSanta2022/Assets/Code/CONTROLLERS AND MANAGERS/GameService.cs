
using UnityEngine;
using System.Collections.Generic;
using Timers;
using VOrb.CubesWar.VFX;
using VOrb.CubesWar.Levels;
using System;

namespace VOrb.CubesWar
{

    public enum LvlType
    {
        simple = 0,
        bonus = 1,
        extrahard = 2
    }

    public struct Player
    {
        public string name;
        public int ID;
        public bool inPlay;
    }

    public interface  IGameManager
    {
        public ITouchSensetive ActiveNow { get; set; }
    }


    public class GameService : Singlton<GameService>, IGameManager
    {
        [Header("Общие настройки")]

        public bool PlayServicesConnected = false;
        public bool Sounds = true;
        public SafeInt NoAds;
        public int GlobalScore;

        [Header("Объект панели сенсора")]
        public TouchRegistrator Sensor;

        public int ScoreGiftsCount = 0;       
        public int RightTargetCount= 0;
        public int multiplyX = 1;

        public int Current_level
        {
            get
            {
                if (currentLevel != null)
                {
                    return currentLevel.LevelNumber;
                }
                else
                    return 0;
            }
        }
        public LevelInfo currentLevel;




        [Header("Игрок")]
        //  Игрок
        private Player CurPlayer = new Player();
        public Player PlayerState => CurPlayer;
        public string PlayerName => CurPlayer.name;



        [Header("Время")]
        // ВРЕМЯ
        public int TimeCount = 0;
        public TimersManager timeManager;



        [Header("игровые объекты/контейнеры")]
        private static ITouchSensetive _activeNow;
        public ITouchSensetive ActiveNow { get => _activeNow; set => _activeNow = value; }

        public GameObject fonContainer;

        public static Canvas MainCanvas;
        public static GameObject UIMainWindowContainer;
        public GameObject GameElements;
        public GameObject GunContainer;
        private GameObject TopUIContainer;

        [Header("Игровые контроллеры")]

        public static GiftsDriver CubsDriver;
        public static VFX_Manager VFXDriver;
        //ПУШКА
        public Joystik GameJoystik;
        public SantaDriver GunController;


        // ИГРОВОЕ СОСТОЯНИЕ
        public bool GameStarted
        {
            get => _gameStarted;
            set
            {
                _gameStarted = value;
                CurPlayer.inPlay = value;
            }
        }
                

        private bool _gameStarted;


        protected override void Init()
        {
            base.Init();

            //Инициализация структуры игрока
            CurPlayer.name = "";
            CurPlayer.ID = 0;


            //Сброс дефалтов
            Instance.GameStarted = false;
        }


        public void StartGame(LevelInfo level)
        {
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();

            if (Time.timeScale == 0) //снимаем с паузы
            {
                Pause();
                SetActiveLvlTimer(true, false);
            }
            else
            {
#if UNITY_ANDROID && !UNITY_EDITOR
             if (!IronSource.Agent.isInterstitialReady())
             {
                IronSource.Agent.loadInterstitial();
             }                           
#endif
                if (level.Contains<NumberedHouseChimney>())
                {
                    level.GenerateNumbersList(3);
                }
                RightTargetCount = 0;
                ScoreGiftsCount = 0;

                currentLevel = level;

                CubsDriver.CleanState();
                VFXDriver.StartSnow();

                mn.SetProgressSlider(0.1f);
                mn.SetGiftsInfo(level.Giftscount);
                mn.SetSmilesInfo(0);
                mn.SetTargetsInfo(0);

                ObjectPoolManager.DeactivatePool(PooledObjectType.Gift);
                DeactivateHomes();

                GunController.ReArm();
                SetActiveLvlTimer(true, true);


                GameStarted = true;

                mn.OpenLevelInfoPopup();

                SoundService.AttestMusic();

                GameElements.GetComponentInChildren<HomesLife>(true).gameObject.SetActive(true);

            }
            mn.SetSettingsByDefaults();
        }

        public void StopGame(bool WithoutCalc = false)
        {
            GameStarted = false;
            Time.timeScale = 1;
            SoundService.AttestMusic();
            
            
            SetActiveLvlTimer(false);
            ObjectPoolManager.DeactivatePool(PooledObjectType.Brick);
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            if (WithoutCalc)
            {
                ObjectPoolManager.DeactivatePool(PooledObjectType.Gift);
                DeactivateHomes();
                mn.OpenStartWindow();
                currentLevel = null;
            }
            else
            {
                int lastVal = GlobalScore;
                GlobalScore += ScoreGiftsCount;
                GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Smiles, GlobalScore);
                mn.TryOpenLevelResultsPopup(
                () =>
                {
                   ObjectPoolManager.DeactivatePool(PooledObjectType.Gift);
                   DeactivateHomes();
                   mn.OpenStartWindow();
                   currentLevel = null;
                   if (GlobalScore >= 2022 && lastVal < 2022)
                   {
                       UIWindowsManager.GetWindow<StartWindow>().ShowWinnerScreen();
                   }

                });

                
            }
            CubsDriver.CleanState();

        }

        private void DeactivateHomes()
        {
            HomesLife mover = GameElements.GetComponentInChildren<HomesLife>(true);
            int minTypeNum = (int)PooledObjectType.Homes1;
            for (int i = 0; i < SceneLoader.SceneSettings.HomesPrefab.Length; i++)
            {
                var homes = ObjectPoolManager.GetActivePoolWithType((PooledObjectType)minTypeNum + i, withAsked: true);

                foreach (var home in homes)
                {
                    home.ReleaseToPool();
                }
                
            }
            mover?.CleareEnviropment();
            mover?.gameObject.SetActive(false);
        }

        public void Pause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            SoundService.PauseMusic(Time.timeScale == 0);
        }

        public void ReceiveContainers()
        {
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            if (mn != null)
            {
                UIMainWindowContainer = mn.UIContainer;
                VFXDriver = mn.VFXContainer.AddComponent<VFX_Manager>();

                GameElements = mn.GameElements;
                GunContainer = mn.GunContainer;
                
            }

            TopUIContainer = SceneLoader.SceneSettings.TopUISensor;

        }

        //Обязяательно нужно вызвать перед работой с классом
        public void LoadFields()
        {

            JoystikContainer JoystikParent = SceneLoader.SceneSettings.TopUISensor.GetComponentInChildren<JoystikContainer>();
            GameJoystik = JoystikParent.gameObject.transform.GetComponentInChildren<Joystik>();
            GameJoystik?.Init();
            GunController = GunContainer.GetComponentInChildren<SantaDriver>();
            CurPlayer.name = SceneLoader.SceneSettings.PlayerName;
            CubsDriver = new GiftsDriver();

            MainCanvas = TopUIContainer.GetComponent<Canvas>();


            ////фэйк прохождение уровней

            //foreach (var level in DataBaseManager.Instance.LevelsInfo)
            //{
            //    if (level.LevelNumber<12)
            //    {
            //        UIWindowsManager.GetWindow<MainWindow>().StarsPath.SetLevelStars(level.LevelNumber, 3);
            //    }
            //    UIWindowsManager.GetWindow<MainWindow>().StarsPath.SetLevelStars(12, 2);
            //    UIWindowsManager.GetWindow<MainWindow>().StarsPath.SetLevelStars(13, 1);
            //    UIWindowsManager.GetWindow<MainWindow>().StarsPath.SetLevelStars(14, 1);


            //}
        }


        public void CleanContainer(GameObject container)
        {
            if (container == null)
            {
                Instance.ReceiveContainers();
            }
            int cnt = container.transform.childCount;
            for (int i = 0; i < cnt; i++)
            {

                Destroy(container.transform.GetChild(i).gameObject);
            }
        }

        public List<GameObject> GetAllIn(GameObject container)
        {
            List<GameObject> allObjects = new List<GameObject>();
            if (container == null)
            {
                Instance.ReceiveContainers();
            }
            int cnt = container.transform.childCount;
            for (int i = 0; i < cnt; i++)
            {
                allObjects.Add(container.transform.GetChild(i).gameObject);
            }
            return allObjects;
        }

        public void OnTimerTick()
        {
            TimeCount++;
        }
        public void SetActiveLvlTimer(bool value, bool drop_count = false)
        {
            TimersManager.SetPaused(OnTimerTick, !value);
            TimeCount = drop_count ? 0 : TimeCount;
        }


        public int CalculateStarsResult()
        {
            int starscount = 3;
            float procent = (float)RightTargetCount / currentLevel.Giftscount;
            if (procent < 0.5f)
            {
                starscount = 0;              
            }
            else if (procent >= 0.5f && procent < 0.8f)
            {
                starscount = 1;
            }
            else if (procent >= 0.8f && procent < 1f)
            {
                starscount = 2;
            }

            return starscount;

        }




    }



}