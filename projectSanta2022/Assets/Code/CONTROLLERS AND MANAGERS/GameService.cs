
using UnityEngine;
using System.Collections.Generic;
using Timers;
using VOrb.SantaJam.VFX;
using VOrb.SantaJam.Levels;
using System;

namespace VOrb.SantaJam
{

    public interface  IGameManager
    {
        public ITouchSensetive ActiveNow { get; set; }
    }

    public class GameService : Singlton<GameService>, IGameManager
    {

        private bool _sounds = true;
        private SafeInt _noAds;
        private int _globalScore;
        private TouchRegistrator _sensor;
        private int _smilesScoreCount = 0;
        private int _hitedTargetsCount = 0;
        private int _scoreMultiply = 1;
        private LevelInfo _currentLevel;
        private int TimeCount = 0;
        private static ITouchSensetive _activeNow;
        public ITouchSensetive ActiveNow { get => _activeNow; set => _activeNow = value; }

        private GameObject _gameElements;
        private GameObject _santaContainer;

        private static GiftsDriver _giftsController;
        private static VFX_Manager _vFXDriver;
        private ThrowDriver _santaController;
        private Joystik _gameJoystik;
        private bool _gameStarted;

        public bool GameStarted
        {
            get => _gameStarted;
            set
            {
                _gameStarted = value;
            }
        }

        public bool Sounds { get => _sounds; set => _sounds = value; }
        public SafeInt NoAds { get => _noAds; set => _noAds = value; }
        public int GlobalScore { get => _globalScore; set => _globalScore = value; }
        public TouchRegistrator Sensor { get => _sensor; set => _sensor = value; }
        public int SmilesScore { get => _smilesScoreCount; set => _smilesScoreCount = value; }
        public int HitedTargetsCount { get => _hitedTargetsCount; set => _hitedTargetsCount = value; }

        public int Current_level
        {
            get
            {
                if (CurrentLevel != null)
                {
                    return CurrentLevel.LevelNumber;
                }
                else
                    return 0;
            }
        }

        public int ScoreMultiply  => _scoreMultiply; 
        public LevelInfo CurrentLevel { get => _currentLevel; private set => _currentLevel = value; }
        public string PlayerName => SceneLoader.SceneSettings.PlayerName;

        public GameObject GameElements { get => _gameElements; private set => _gameElements = value; }
        public GameObject SantaContainer { get => _santaContainer; private  set => _santaContainer = value; }
        public static GiftsDriver GiftsController { get => _giftsController; private set => _giftsController = value; }
        public static VFX_Manager VFXDriver { get => _vFXDriver; private set => _vFXDriver = value; }
        public Joystik GameJoystik { get => _gameJoystik; private set => _gameJoystik = value; }
        public ThrowDriver SantaController { get => _santaController; private set => _santaController = value; }

        protected override void Init()
        {
            base.Init();
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
                if (level.Contains<NumberedHouseChimney>())
                {
                    level.GenerateNumbersList(3);
                }
                HitedTargetsCount = 0;
                SmilesScore = 0;

                CurrentLevel = level;

                GiftsController.CleanState();
                VFXDriver.StartSnow();

                mn.SetProgressSlider(0.1f);
                mn.SetGiftsCountInfo(level.Giftscount);
                mn.SetSmilesInfo(0);
                mn.SetTargetsInfo(0);

                ObjectPoolManager.DeactivatePool(PooledObjectType.Gift);
                DeactivateHomes();

                SantaController.ReArm();
                SetActiveLvlTimer(value: true, drop_counter: true);


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
                CurrentLevel = null;
            }
            else
            {
                int lastVal = GlobalScore;
                GlobalScore += SmilesScore;
                GameStorageOperator.PutToDevice(GameStorageOperator.PlayerParamNames.Smiles, GlobalScore);
                mn.TryOpenLevelResultsPopup(
                () =>
                {
                   ObjectPoolManager.DeactivatePool(PooledObjectType.Gift);
                   DeactivateHomes();
                   mn.OpenStartWindow();
                   CurrentLevel = null;
                   if (GlobalScore >= 2022 && lastVal < 2022)
                   {
                       UIWindowsManager.GetWindow<StartWindow>().ShowWinnerScreen();
                   }

                });

                
            }
            GiftsController.CleanState();

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
                VFXDriver = mn.VFXContainer.AddComponent<VFX_Manager>();
                GameElements = mn.GameElements;
                SantaContainer = mn.SantaContainer;
            }
        }

        //Обязяательно нужно вызвать перед работой с классом
        public void LoadFields()
        {
            JoystikContainer JoystikParent = SceneLoader.SceneSettings.TopUISensor.GetComponentInChildren<JoystikContainer>();
            GameJoystik = JoystikParent.gameObject.transform.GetComponentInChildren<Joystik>();
            GameJoystik?.Init();
            SantaController = SantaContainer.GetComponentInChildren<ThrowDriver>();
            GiftsController = new GiftsDriver();
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
        public void SetActiveLvlTimer(bool value, bool drop_counter = false)
        {
            TimersManager.SetPaused(OnTimerTick, !value);
            TimeCount = drop_counter ? 0 : TimeCount;
        }


        public int CalculateStarsResult()
        {
            int starscount = 3;
            float procent = (float)HitedTargetsCount / CurrentLevel.Giftscount;
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