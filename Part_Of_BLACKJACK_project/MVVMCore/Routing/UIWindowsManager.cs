using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using GoldenSoft.UI.MVVM;

namespace GoldenSoft.UI
{
    public class UIWindowsManager : Singlton<UIWindowsManager>
    {
        private readonly Dictionary<Type, (Type ViewModel, Type Model)> _viewsDependenses = new Dictionary<Type, (Type ViewModel, Type Model)>()
        {
           {typeof(StartWindow), (ViewModel: typeof(StartWindowViewModel), Model: typeof(StartWindowModel)) },
           
            {typeof(OriginStartWindow), (ViewModel: typeof(StartWindowViewModel), Model: typeof(StartWindowModel)) },

           {typeof(InfoPopup), (ViewModel: typeof(InfoPopupViewModel), Model: typeof(InfoPopupModel)) },

           {typeof(DepositPanel), (ViewModel: typeof(DepositPanelViewModel), Model: typeof(DepositPanelModel)) },

           {typeof(DoubleDepositePanel), (ViewModel: typeof(DepositPanelViewModel), Model: typeof(DepositPanelModel)) },

           {typeof(WaitDepositPanel), (ViewModel: typeof(DepositPanelViewModel), Model: typeof(DepositPanelModel)) },

           {typeof(WaitDoubleDepositPanel), (ViewModel: typeof(DepositPanelViewModel), Model: typeof(DepositPanelModel)) },

           {typeof(OriginPvpWaitWindow), (ViewModel: typeof(WaitWindowViewModel), Model: typeof(WaitWindowModel)) },    
           
            {typeof(WaitWindow), (ViewModel: typeof(WaitWindowViewModel), Model: typeof(WaitWindowModel)) },

           {typeof(PlayWindow), (ViewModel: typeof(PlayViewModel), Model: typeof(PlayModel)) },
           
            {typeof(PvpPlayWindow), (ViewModel: typeof(PvpViewModel), Model: typeof(PlayModel)) },
            
            {typeof(FortuneWheelWindow), (ViewModel: typeof(FortuneWheelViewModel), Model: typeof(FortuneWheelModel)) },

            {typeof(LooseResultWindow), (ViewModel: typeof(ResultViewModel), Model: typeof(ResultWindowModel)) },
            
            {typeof(WinResultWindow), (ViewModel: typeof(ResultViewModel), Model: typeof(ResultWindowModel)) },

            {typeof(TournamentWindow), (ViewModel: typeof(TournamentWindowViewModel), Model: typeof(TournamentWindowModel)) },
 
            {typeof(HistoryWindow), (ViewModel: typeof(HistoryWindowViewModel), Model: typeof(HistoryWindowModel)) },
            
            {typeof(SettingsWindow), (ViewModel: typeof(SettingsWindowViewModel), Model: typeof(SettingsWindowModel)) },
            
            {typeof(BattleResultWindow), (ViewModel: typeof(BattleResultWindowViewModel), Model: typeof(BattleResultWindowModel)) },

        };

        private readonly List<IUIModel> _activeModels = new List<IUIModel>();

        [SerializeField]
        private Transform _uiWindowsContainer, _uiOverlayElementsContainer;

        [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();


        private readonly Dictionary<Type, GameObject> _registeredPrefabs = new Dictionary<Type, GameObject>();

        [SerializeField] private List<Window> _windows = new List<Window>();


        private static Window _activeNow;
        public static Window ActiveWindow => _activeNow;

        private void RegisterPrefab(GameObject prefab)
        {
            if (prefab == null) return;

            if (prefab.TryGetComponent(out Window window) && !_registeredPrefabs.ContainsKey(window.GetType()))
            {
                Type windowType = window.GetType();
                _registeredPrefabs.Add(windowType, prefab);

                if (!_viewsDependenses.TryGetValue(window.GetType(), out (Type ViewModel, Type Model) value)) 
                    Debug.LogWarning($"*Registering prefabs* UIWindowManager: Dependenses  for window type {windowType} are not set! Add it to the UIWindowManager class.");
                
            }
        }
        protected override void Init()
        {
            base.Init();

            foreach (var item in _prefabs)
            {
                RegisterPrefab(item);
            }

        }

        private IUIModel GetNewModel<T>()
        {
            IUIModel output = null;

            if (_viewsDependenses.TryGetValue(typeof(T), out (Type VM, Type M) pair))
            {
                output = (IUIModel)Activator.CreateInstance(pair.M);
            }

            return output;
        }

        private GameObject GetPrefab<T>()
        {
            GameObject output = null;

            if (_registeredPrefabs.TryGetValue(typeof(T), out GameObject prefab))
            {
                output = prefab;
            }

            return output;
        }

        void Start()
        {
            EventPublisher.GameComponentStarted?.Publish(typeof(UIWindowsManager));
        }

        public  static MVVMElement<T> OpenAsSingle<T>(bool needDestroyModelAfterClose = true, bool needCleareOverlay = false) where T : Window
        {
            _activeNow = null;

            return CreateUIElement<T>(asSingle: true, needDestroyModelAfterClose, needCleareOverlay);
        }

        public static MVVMElement<T> OpenAsOverlayPanel<T>(bool needDestroyModelAfterClose = false, bool needCleareOverlay = false) where T : Window
        {
            return CreateUIElement<T>(asSingle: false, needDestroyModelAfterClose, needCleareOverlay);
        }

        private static MVVMElement<T> CreateUIElement<T>(bool asSingle, bool needDestroyModelAfterClose,bool needCleareOverlay = false) where T : Window
        {
            T view = null;
            IUIModel model = null;
            IViewModel ViewModel = null;

            if (!Instance._registeredPrefabs.ContainsKey(typeof(T))) 
            {
                Debug.LogWarning($"*Registering prefabs* UIWindowManager: Prefab for window type {typeof(T)} are not registered! Add it to the UIWindowManager class.");
                return default; 
            }

            if (needCleareOverlay)
                Instance.ClearePanelUIContainer();

            if (asSingle)
                Instance.CleareUIContainer();


            if (Instance._viewsDependenses.TryGetValue(typeof(T), out (Type VM, Type M) pair))
            {
                model = Instance._activeModels.FirstOrDefault(el => el.GetType() == pair.M);

                if (model == null)
                {
                    model = Instance.GetNewModel<T>();

                    if (model == null) return default;

                    Instance._activeModels.Add(model);
                }

                ViewModel = (IViewModel)Activator.CreateInstance(pair.VM, model);

                var prefab = Instance.GetPrefab<T>();

                if (prefab == null || ViewModel == null) return default;
                
                var gameObject = GameObject.Instantiate(prefab, asSingle ? Instance._uiWindowsContainer : Instance._uiOverlayElementsContainer);

                view = gameObject.GetComponent<T>();

                if (view == null)
                {
                    GameObject.Destroy(gameObject);
                    
                    if(model != null)    Instance._activeModels.Remove(model);

                    return default;
                }

                if (asSingle)
                {
                    _activeNow = view;
                }

                view.Init(ViewModel);
                view.Open(null);

                view.OnDestroyWindow += () => { Instance.Release<T>(needDestroyModelAfterClose); };

                Instance._windows.Add(view);

                return (model,view, ViewModel);
                
            }

            return (model, view, ViewModel);

        }

        private void Release<T>(bool isModelDestroable) where T : Window
        {

            if (isModelDestroable && Instance._viewsDependenses.TryGetValue(typeof(T), out (Type VM, Type M) pair))
            {
                var model = Instance._activeModels.FirstOrDefault(el => el.GetType() == pair.M);
                if(model != null)
                    Instance._activeModels.Remove(model);
            }

            var forDelete = Instance._windows.Where(w => w is T).ToList();
            foreach (var window in forDelete)
            {
                Instance._windows.Remove(window);
            }

            if (ActiveWindow is T) _activeNow = null;
        }

        public T ShowPopupWindow<T>(string text, string title, Action closeAction, string buttonTitle = "") where T : PopupWindow
        {
            var mvvmPopup = OpenAsOverlayPanel<InfoPopup>(true);            
            ((InfoPopupModel)mvvmPopup.data).SetInfoText(text, title, buttonTitle);
            ((InfoPopupViewModel)mvvmPopup.viewModel).SetCloseAction(closeAction);

            return mvvmPopup.view as T;
        }

        public void PrintState()
        {
            string models = "";

            foreach (var item in _activeModels)
            {
                models += $"{item}|";
            }

            string windows = "";

            foreach (var w in _windows)
            {
                windows += $"{w.GetType()}|";
            }

            print($"windows: {windows} ___    activeModels {models}    __    ActiveNow {_activeNow?.GetType()}");
        }

        private void CleareUIContainer()
        {
            _uiWindowsContainer.Clear();

        }

        private void ClearePanelUIContainer()
        {
            _uiOverlayElementsContainer.Clear();
        }

        public static T GetActiveModel<T>() where T : IUIModel
        {
            return (T)Instance._activeModels.FirstOrDefault(m => m.GetType() == typeof(T));
        } 

        public static T GetOpenedWindow<T>() where T : Window => (T)Instance._windows.FirstOrDefault(m => m.GetType() == typeof(T));

    }

    public struct MVVMElement<T> where T : Window
    {
        public IUIModel data;
        public T view;
        public IViewModel viewModel;

        public MVVMElement(IUIModel data, T view, IViewModel viewModel)
        {
            this.data = data;
            this.view = view;
            this.viewModel = viewModel;
        }

        public void Deconstruct(out IUIModel data, out T view, out IViewModel viewModel)
        {
            data = this.data;
            view = this.view;
            viewModel = this.viewModel;
        }

        public static implicit operator (IUIModel data, T view, IViewModel viewModel)(MVVMElement<T> value)
        {
            return (value.data, value.view, value.viewModel);
        }

        public static implicit operator MVVMElement<T>((IUIModel data, T view, IViewModel viewModel) value)
        {
            return new MVVMElement<T>(value.data, value.view, value.viewModel);
        }


    }
}