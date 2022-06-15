using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VOrb
{
    public class FonScrollView : FancyScrollView<FonCellData, FonScrollViewContext>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController = null;
        [SerializeField]
        Window parent = null;
        [SerializeField]
        Button PurchaseButton = null;
       


        public bool BtnPurchasedEnable
        {
            get
            {
                return PurchaseButton == null
                       ? false : PurchaseButton.interactable;
            }
            set
            {
                if (PurchaseButton != null)
                {
                    PurchaseButton.interactable = value;
                }               
            }
        }
        public bool BtnPurchasedVisible
        {
            get
            {
                return PurchaseButton == null
                       ? false : PurchaseButton.gameObject.activeInHierarchy;
            }
            set
            {
                if (PurchaseButton != null)
                {
                    PurchaseButton.gameObject.SetActive(value);
                }
            }
        }
        Action<int> onSelectedIndexChanged;

        void Awake()
        {
            scrollPositionController.OnUpdatePosition(p => UpdatePosition(p));
            scrollPositionController.OnItemSelected(HandleItemSelected);
            

            SetContext(new FonScrollViewContext
            {
                OnPressedCell = OnPressedCell,
                OnSelectedIndexChanged = index =>
                {
                    onSelectedIndexChanged?.Invoke(index);
                }
            });
        }
        public FonCellData GetCellData(int index)
        {
            return cellData.Find(fnd => fnd.fon.Id == index + 1);
        }

        public FonCellData GetCellData()
        {
            return cellData.Find(fnd => fnd.fon.Id == Context.SelectedIndex + 1);
        }

        public void UpdateData(List<FonCellData> data )
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);
            UpdateContents();
            
            
        }

      
        public void UpdateSelection(int index)
        {            
            if (index < 0 || index >= cellData.Count)
            {
                return;
            }

            scrollPositionController.ScrollTo(index, 0.4f);
            Context.SelectedIndex = index;
            UpdateContents();
        }

        public void OnSelectedIndexChanged(Action<int> onSelectedIndexChanged)
        {
            this.onSelectedIndexChanged = onSelectedIndexChanged;
        }

        public void SelectNextCell()
        {
            UpdateSelection(Context.SelectedIndex + 1);
        }

        public void SelectPrevCell()
        {
            UpdateSelection(Context.SelectedIndex - 1);
        }

        void HandleItemSelected(int selectedItemIndex)
        {
            Context.SelectedIndex = selectedItemIndex;
            UpdateContents();
        }

        void OnPressedCell(FonCellView cell)
        {

            UpdateSelection(cell.DataIndex);

            var tapedFon = GetCellData().fon;
            UIElement Goal = cell.gameObject.GetOrAddComponent<UIElement>();
            Goal.ID = tapedFon.Id;
            Goal.type = UIElementType.UISkin;
            GameService.Instance.Sensor.TapGoal = Goal;

            if (tapedFon.Id <= CollectingSystem.Instance.RateProgress(true))
            {             
                // меняем фон
                Debug.Log("Fon is enabled! Change it!");
                UIWindowsManager.GetWindow<MainWindow>().SetFon(tapedFon.Id);
            }

           

        }

        
    }
}
