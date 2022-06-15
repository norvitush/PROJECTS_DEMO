
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Timers;
using UnityEngine.UI;
using System.Collections;

namespace VOrb
{

    public class GameService : Singlton<GameService>
    {
        const int LAST_SESSION_NOT_COMPLETE = -10;
        
        private bool _gameStarted;

        //Это игрок        
        private Player _currentPlayer = new Player();

        private int _currentFonNumber = 1;
        private int _gameSpeedTimer = 0;
        private LvlType _activeLvl_type = LvlType.simple;
        private float _timeCount = 0;
        private SafeSkinData _currentSkin;
        private Rigidbody _avatarRig = null;
        private float _fullSizeVolume = 0f;

        public bool PlayServicesConnected = false;
        public float BestTime { get; set; }
        public bool Sounds = true;
        public float yTopCoordForBonus = 0f;
        public TouchRegistrator Sensor;
        public bool TutorialEnded => _currentPlayer.TutorialEnded;
        public ChallengeServer challengeServer;

        public LvlType ActiveLvlType
        {
            get
            {
                return _activeLvl_type;
            }
            set
            {
                var mn = UIWindowsManager.GetWindow<MainWindow>();
                if (mn!=null)
                {
                    if (value == LvlType.bonus)
                    {
                        mn.MainMessage.text = "Bonus lvl" + " \n " + "All Gain x2 !!!";
                    }
                    else
                    {
                        mn.MainMessage.text = SceneLoader.sceneSettings.BeginText;
                    }
                }
                _activeLvl_type = value;
                

            }
        }

        private TimersManager _timeManager;
        private Animator _jumpie_anim;
        private AvatarController _avatarControl;
        private PhysicMaterial _current_physicMaterial;
        private Material _sliceMaterial;
        private TextMeshProUGUI _coinsTextMesh;
        private TextMeshProUGUI _partsTextMesh;
        private GameObject _fonContainer;
        private GameObject _avatarContainer;
        private GameObject _ropeContainer;
        private GameObject _groundContainer;
        private GameObject _itemsContainer;
        private GameObject _lowItemContainer;
        private GameObject _uIContainer;
        private GameObject _tutorialContainer;

        public ItemsGenerator itemGenerator = new ItemsGenerator();

        
        public Player PlayerState => _currentPlayer;
        public string PlayerName => _currentPlayer.name;
        public int CurrentFonNumber { get => _currentFonNumber; set => _currentFonNumber = value; }

        //признак начала игры
        public bool GameStarted {
            get => _gameStarted;
            set 
            {
                _gameStarted = value;
                _currentPlayer.CanPlay = value;
            }
        }

        public float FullSizeVolume { get => _fullSizeVolume; set => _fullSizeVolume = value; }

        //признак что игрок не может действовать        
        public bool PlayerIsStunned { get=>!_currentPlayer.CanPlay; set=> _currentPlayer.CanPlay=!value; }
        //контейнеры

        public TimersManager TimeManager { get => _timeManager; set => _timeManager = value; }
        public Animator Jumpie_anim { get => _jumpie_anim; set => _jumpie_anim = value; }
        public AvatarController AvatarControl { get => _avatarControl; set => _avatarControl = value; }
        public PhysicMaterial Current_physicMaterial { get => _current_physicMaterial; set => _current_physicMaterial = value; }
        public Material SliceMaterial { get => _sliceMaterial; set => _sliceMaterial = value; }
        public TextMeshProUGUI CoinsTextMesh { get => _coinsTextMesh; set => _coinsTextMesh = value; }
        public TextMeshProUGUI PartsTextMesh { get => _partsTextMesh; set => _partsTextMesh = value; }
        public GameObject FonContainer { get => _fonContainer; set => _fonContainer = value; }
        public GameObject AvatarContainer { get => _avatarContainer; set => _avatarContainer = value; }
        public GameObject RopeContainer { get => _ropeContainer; set => _ropeContainer = value; }
        public GameObject GroundContainer { get => _groundContainer; set => _groundContainer = value; }
        public GameObject ItemsContainer { get => _itemsContainer; set => _itemsContainer = value; }
        public GameObject LowItemContainer { get => _lowItemContainer; set => _lowItemContainer = value; }
        public GameObject UIContainer { get => _uIContainer; set => _uIContainer = value; }
        public GameObject TutorialContainer { get => _tutorialContainer; set => _tutorialContainer = value; }

        protected override void Init()
        {
            base.Init();

            //Инициализация игрока
            _currentPlayer.name = "FirstPlayer";
            _currentPlayer.ID = 0;
            _currentPlayer.TutorialEnded = false;
            _currentPlayer.EasyMode = true;

            //Сброс дефалтов
            Instance.GameStarted = false;
            _currentSkin.PlaySkin_Id = 0;
        }
        public void GameSessionEnd(int Count)
        {
            UIWindowsManager.GetWindow<MainWindow>().Btn_pause.SetActive(false);
            
            if (Count <= 0 && !PlayerIsStunned)
            {
                SetActiveLvlTimer(false);
                PlayerIsStunned = true;
                MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();

                    //показываем рестарт
                    RestartWindow restart_wind = UIWindowsManager.GetWindow<RestartWindow>();

                    //преднастройки окна:  //нужен бы фасад
                    //    CAN_SELECT = 128,isSPLASH = 64,HEADER_TEXT = 32, AVATAR_CONTAINER = 16,
                    //    HOME_BTN = 8, CLOSE_BTN = 4,X2_BTN = 2,  BONUS_BTN = 1,   //  0b00000000
                
                    //нужен бы фасад!
                    var new_settings = UIWindowsManager.GetWindow<RestartWindow>().Window_settings;
                    
                    switch (ActiveLvlType)
                    {
                        case LvlType.simple:
                            new_settings.Set(0b01111011);
                            break;
                        case LvlType.bonus:
                            new_settings.Set(0b01111000);
                            break;
                        case LvlType.extrahard:
                            new_settings.Set(0b00111000);
                            break;
                    }

                    restart_wind.CloseCallback = UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow;

                if (Count == LAST_SESSION_NOT_COMPLETE)
                {
                    UIWindowsManager.GetWindow<MainWindow>().OpenRestartWindow(false);
                }
                else
                {
                    mn.HealthBar.SetProgress(0);
                    mn.TapEvent.RemoveAllListeners();
                    mn.SvipeEvent.RemoveAllListeners();
                    UIWindowsManager.ActiveNow = UIWindowsManager.GetWindow<RestartWindow>();
                    if (BestTime < _timeCount)
                    {
                        UIWindowsManager.GetWindow<MainWindow>().Splash_Best.gameObject.SetActive(true, 2.25f, this);
                        UIWindowsManager.GetWindow<MainWindow>().Splash_Timer.gameObject.SetActive(true);                        
                        UIWindowsManager.GetWindow<MainWindow>().Splash_Timer.Play();
                        BestTime = _timeCount;
                        DataKeeper.SaveParam(PlayerName + "_BestTime", BestTime);
                        AudioServer.PlaySound(Sound.ChallengeDone);
                    }
                    UIWindowsManager.GetWindow<MainWindow>().DieEffect.gameObject.SetActive(true);
                    UIWindowsManager.GetWindow<MainWindow>().DieEffect.StartAnimation(() => { UIWindowsManager.GetWindow<MainWindow>().OpenRestartWindow(false); });
                    AvatarControl.MoveRigDown();

                }
  
                StopGame();
            }
        }


        private void Start()
        {
            //проверка игрока
            if (SceneLoader.sceneSettings.DropAccountState)
            {
                CollectingSystem.Instance.DeletePlayerInfo();                
            }

        }

        internal void SetPlayerPrefs(bool tutor, bool easyMode)
        {
            _currentPlayer.TutorialEnded = tutor;
            _currentPlayer.EasyMode = easyMode;
        }

        public void StopGame()
        {
            Instance.GameStarted = false;
            FreezScene();            
        }
        public void CompleteLastSession()
        {

                CollectingSystem.Instance.RestoreLvlResults();
                CoinsTextMesh.text = challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
                PartsTextMesh.text = CollectingSystem.Instance.Lvl_gainedparts.ToString();
                UIWindowsManager.GetWindow<MainWindow>().CurrentWindow.Close();
                Instance.ActiveLvlType = LvlType.bonus;
                GameSessionEnd(LAST_SESSION_NOT_COMPLETE);

        }

        public void StartGame()
        {

            
            Canvas cnv = UIContainer.GetComponent<Canvas>();
           
            Vector3 pt = new Vector2(cnv.pixelRect.xMax * 0.5f, cnv.pixelRect.yMax * SceneLoader.sceneSettings.YTopBonusOffset);
           
            Instance.yTopCoordForBonus = pt.ConvertPixelCoordinatesToWorld(cnv, 0).y;

            GameStarted = true;
            Jumpie_anim.speed = 1;
            _gameSpeedTimer = 0;
            UnFreezeScene();
            CoinsTextMesh.text = challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
            
            PartsTextMesh.text = CollectingSystem.Instance.Lvl_gainedparts.ToString();
            UIContainer.SetActive(true);
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            mn.Btn_pause.SetActive(true);
            mn.Btn_timer.SetActive(true);

            SetActiveLvlTimer(true,true);           
            mn.BonusFon.SetActive(false);
            mn.BonusText.gameObject.SetActive(false);

            if (challengeServer.GetCurrent().CoinsTarget < 0)
            {
                mn.Btn_info.SetActive(false);
            }

            if (!IsItemInScene())
            {
                itemGenerator.GenerateNext();
                CollectingSystem.Instance.OnCollect(new Item { ItemAction = ItemAction.none });
            }
            
        }

        public void Pause()
        {
            Debug.Log("Time.timeScale    до " + Time.timeScale);
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            //GameStarted = Time.timeScale!=0 ? false : true;
        }

        public void ReastartLvl()
        {
            Instance.GameStarted = false;
            UIWindowsManager.GetWindow<MainWindow>().CurrentWindow.Close(); // теперь события обрабатвает Main окно
        }

        public void FreezScene()
        {
            if (_avatarRig!= null)
            {
                
                Jumpie_anim.SetBool("CanRotate", false);               
                
                _avatarRig.constraints = RigidbodyConstraints.FreezePosition;
            }
            else
            {
                Debug.Log("Фриз сцены Аватара то нет!");
            }

        }
        public void UnFreezeScene()
        {
            if (_avatarRig != null && Jumpie_anim!= null)
            {
                Jumpie_anim.Play("Ready");
                Jumpie_anim.SetBool("CanRotate", true);

                _avatarRig.constraints = RigidbodyConstraints.None;
                _avatarRig.constraints |= RigidbodyConstraints.FreezePositionX;
                _avatarRig.constraints |= RigidbodyConstraints.FreezePositionZ;
                if (GameService.Instance.PlayerState.EasyMode)
                {
                    _avatarRig.constraints |= RigidbodyConstraints.FreezeRotationX;
                   // AvatarRig.constraints |= RigidbodyConstraints.FreezeRotationY;
                    _avatarRig.constraints |= RigidbodyConstraints.FreezeRotationZ;
                }
            }
        }

        public GameObject GetAvatarObj()
        {
            if (_avatarRig != null)
            {
                return _avatarRig.gameObject;
            }
            else
                return null;
        }

        public void ReceiveContainers()
        {
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            if (mn!=null)
            {                
                UIContainer = mn.transform.GetComponentInChildren<main_window_ui>(true).gameObject;                
                FonContainer = mn.transform.Find("Fon_container").gameObject ;
                AvatarContainer = mn.transform.Find("Avatar_container").gameObject;
                RopeContainer = mn.transform.Find("Rope_Container").gameObject;
                GroundContainer = mn.transform.Find("Ground_container").gameObject ;
                ItemsContainer = mn.transform.Find("Item_container").gameObject;
                LowItemContainer = mn.transform.Find("LowItem_container").gameObject;                
                TutorialContainer = UIContainer.transform.Find("TUTORIAL").gameObject;
                TutorialContainer.SetActive(false);
            }
        }

        //Базовая инициализация полей менеджера
        public void LoadFields()
        {

           Jumpie_anim = RopeContainer != null ? RopeContainer.transform.GetChild(0)?.GetComponent<Animator>(): null;
          
           Current_physicMaterial = (PhysicMaterial)Resources.Load("materials/Bounced");
           SliceMaterial = (Material)Resources.Load("materials/BaseMat");

            AvatarControl = AvatarContainer?.GetComponent<AvatarController>();
            AvatarControl.parentWindow = UIWindowsManager.GetWindow<MainWindow>();
            CoinsTextMesh = UIWindowsManager.GetWindow<MainWindow>().CoinText;
            PartsTextMesh = UIWindowsManager.GetWindow<MainWindow>().PartText;

            CurrentFonNumber = (int)DataKeeper.LoadParam(PlayerName + "_CurrentFonNumber", 1);
            BestTime = (float)DataKeeper.LoadParam(PlayerName + "_BestTime", 0f);

            SetPlayerPrefs(tutor: (bool)(SafeInt)(int)DataKeeper.LoadParam("tutor", 0),
                                        easyMode: (bool)(SafeInt)(int)DataKeeper.LoadParam("easymode", 1));

            Sounds = (bool)(SafeInt)(int)DataKeeper.LoadParam("Sounds", 0);

            challengeServer = new ChallengeServer(CoinsTextMesh.transform.parent.Find("ChallengeBar").GetComponent<Image>());
            UIWindowsManager.GetWindow<MainWindow>().SetFon(CurrentFonNumber);
        }
        public static bool IsItemInScene()
        {
            return Instance.ItemsContainer.transform.childCount>0;
        }
        public void PutItemToGame(Item addingItem)
        {
            if (addingItem==null)
            {
                CleanContainer(ItemsContainer);
                return;
            }
            if (ItemsContainer == null)
            {
                Instance.ReceiveContainers();
            }
            if (ItemsContainer.transform.childCount > 0)
            {
                CleanContainer(ItemsContainer);
            }

            string prefPath = "prefabs/" + addingItem.PrefabName;

            GameObject newItem = Instantiate(Resources.Load(prefPath, typeof(GameObject)), ItemsContainer.transform) as GameObject;
            newItem.transform.localPosition = Vector3.zero;
            newItem.transform.localRotation.eulerAngles.Set(0, 0, 0);
            newItem.GetOrAddComponent<GameElem>().ID = addingItem.Id;
        }
        
        public void PutAvatarToGame(SafeSkinData newSkin)
        {
            if (AvatarContainer==null)
            {
                ReceiveContainers();
            }
            if (AvatarContainer.transform.childCount>0)
            {
                CleanContainer(AvatarContainer);
            }
           
            _currentSkin = newSkin;
            CollectingSystem.Instance.SetPlayerAvatar(newSkin.PlaySkin_Id);
            
            string prefPath = "prefabs/" + newSkin.PrefabName;

            GameObject newAvatar = Instantiate(Resources.Load(prefPath, typeof(GameObject)), AvatarContainer.transform) as GameObject;
            GameObject blood = Instantiate(Resources.Load("prefabs/Blood", typeof(GameObject)), AvatarContainer.transform) as GameObject;
            newAvatar.transform.localPosition = Vector3.zero;
            newAvatar.transform.localRotation.eulerAngles.Set(0, 0, 0);
            SliceMaterial.SetColor("_BaseColor", newSkin.matColor);
            
            _avatarRig = newAvatar.GetComponent<Rigidbody>();
            AvatarControl.Rig = _avatarRig;
            AvatarControl.BloodEffect = blood;
            FullSizeVolume = newAvatar.GetComponent<MeshFilter>().sharedMesh.VolumeOfMesh();

            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();            
            mn.HealthBar.SetProgress(100f);            
            mn.TapEvent.RemoveAllListeners();            
            mn.TapEvent.AddListener(()=> { AvatarControl.TapEvent.Invoke(); });           
            mn.SvipeEvent.RemoveAllListeners();
            mn.SvipeEvent.AddListener(() => { AvatarControl.SvipeEvent.Invoke(); });            

        }
        public void UpdateAvatarObject(GameObject newAvatarobject)
        {
            _avatarRig = newAvatarobject?.GetComponent<Rigidbody>();
            AvatarControl.Rig = _avatarRig;
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
        public void SpeedTimer()
        {
            _gameSpeedTimer++;
            if (_gameSpeedTimer % 10 == 0)
            {
                Jumpie_anim.speed *= SceneLoader.sceneSettings.JumpieVelocity;
            }
        }
        public void OnTimerTick()
        {
            _timeCount += 0.1f;
            UIWindowsManager.GetWindow<MainWindow>().Timer.text = _timeCount.ToString( "0.00");           
        }
        public void SetActiveLvlTimer( bool value, bool drop_count=false)
        {
            TimersManager.SetPaused(OnTimerTick, !value);
            TimersManager.SetPaused(SpeedTimer, !value);
            _timeCount = drop_count? 0: _timeCount;
            _gameSpeedTimer = drop_count ? 0 : _gameSpeedTimer;
            UIWindowsManager.GetWindow<MainWindow>().Timer.text = _timeCount.ToString("0.00");
        }


        public void ShowScoreContainer(bool value)
        {
            if (!GameStarted)
            {
                UIContainer.SetActive(value);
            }            
        }
    }

}

