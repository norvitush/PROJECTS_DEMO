using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VOrb
{
    public class AvatarScrollView : FancyScrollView<SkinCellData, AvatarScrollViewContext>
    {
        [SerializeField]
        ScrollPositionController scrollPositionController = null;
        [SerializeField]
        Window parent = null;
        [SerializeField]
        Button PurchaseButton = null;
       


        public void BtnPurchasedEnable(bool value, bool purchased)
        {

            if (purchased)
            {
                PurchaseButton.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_green");
                Image lable = PurchaseButton.transform.Find("Image").GetComponent<Image>();
                lable.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_P_yes");
            }
            else
            {
                if (value)
                {
                    PurchaseButton.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_orange");
                    Image lable = PurchaseButton.transform.Find("Image").GetComponent<Image>();
                    lable.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_P_no");
                }
                else
                {
                    PurchaseButton.GetComponent<Image>().sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_gray");
                    Image lable = PurchaseButton.transform.Find("Image").GetComponent<Image>();
                    lable.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == "btn_P_no");
                }

            }
            
                            


        }

        Action<int> onSelectedIndexChanged;

        void Awake()
        {
            scrollPositionController.OnUpdatePosition(p => UpdatePosition(p));
            scrollPositionController.OnItemSelected(HandleItemSelected);
//            PurchaseButton?.onClick.AddListener(OnPurchaseClick);

            SetContext(new AvatarScrollViewContext
            {
                OnPressedCell = OnPressedCell,
                OnSelectedIndexChanged = index =>
                {
                    onSelectedIndexChanged?.Invoke(index);
                }
            });
        }
        public SkinCellData GetCellData(int index)
        {
            return cellData.Find(skd => skd.skinInfo.Data.PlaySkin_Id == index+1);
        }

        public SkinCellData GetCellData()
        {
            return cellData.Find(skd => skd.skinInfo.Data.PlaySkin_Id == Context.SelectedIndex + 1);
        }

        public void UpdateData(List<SkinCellData> data )
        {
            cellData = data;
            scrollPositionController.SetDataCount(cellData.Count);

            UpdateContents();
            
            
        }

        //При выборе по щелчку/тапу
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

        void OnPressedCell(AvatarCellView cell)
        {
            
            UpdateSelection(cell.DataIndex);
            var tapedSkin = GetCellData().skinInfo.Data;            
            UIElement Goal = cell.gameObject.GetOrAddComponent<UIElement>();
            Goal.ID = tapedSkin.PlaySkin_Id;
            Goal.type = UIElementType.UISkin;
            GameService.Instance.Sensor.TapGoal = Goal;
           
            if (tapedSkin.PlaySkin_Id != 0 && tapedSkin.Enabled==1 && tapedSkin.Purchased==1)
            {
                UIWindowsManager.GetWindow<AvatarWindow>().OnPressAvatar(tapedSkin);
            }

        }
        //private void OnPurchaseClick()
        //{
        //    CollectingSystem.Instance.BuySkin(GetCellData().skinInfo.Data.PlaySkin_Id);
        //}
    }
}
