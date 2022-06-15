using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace VOrb
{
    public class AvatarScroller : MonoBehaviour
    {
        public Animator PurchaseAnimator;
        [SerializeField]
        AvatarScrollView scrollView = null;
        [SerializeField]
        Button prevCellButton = null;
        [SerializeField]
        Button nextCellButton = null;
        [SerializeField]
        Text selectedItemInfo = null;
        public SkinCellData Selected = null;
        public Text priceText = null;

        public Image Img_coin = null;

        public Image Img_part = null;

        void Start()
        {
            prevCellButton?.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton?.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);
            List<SafeSkinData> AllSkins = DataBaseManager.Instance.GetPlayerSkinsCollection();         
           // SceneLoader.Debuger("Список загруженных скинов: " + AllSkins.Count);
            var cellData = Enumerable.Range(1, AllSkins.Count)
                .Select(i => new SkinCellData {
                    skinInfo = DataBaseManager.GetSkinInfo("", i),
                    av_sprite = DataBaseManager.Instance.SpriteBase.Find(
                        sp => sp.name == DataBaseManager.GetSkinInfo("", i).Data.PrefabName + "_img"
                        ),
                    glow_sprite = DataBaseManager.Instance.SpriteBase.Find(
                        sp => sp.name == DataBaseManager.GetSkinInfo("", i).Data.PrefabName + "_outer"
                        )
                })
                .ToList();

            scrollView.UpdateData(cellData);
            SafeSkinData current;
            CollectingSystem.Instance.GetPlayerAvatar(out current);
            scrollView.UpdateSelection(current.PlaySkin_Id-1);
           // Debug.Log("GameService.Instance.CurrentSkin.PlaySkin_Id :  " + current.PlaySkin_Id);
        }
        public void UpdateCellsData()
        {
            List<SafeSkinData> AllSkins = DataBaseManager.Instance.GetPlayerSkinsCollection();
            var cellData = Enumerable.Range(1, AllSkins.Count)
                .Select(i => new SkinCellData
                {
                    skinInfo = DataBaseManager.GetSkinInfo("", i),
                    av_sprite = DataBaseManager.Instance.SpriteBase.Find(
                        sp => sp.name == DataBaseManager.GetSkinInfo("", i).Data.PrefabName + "_img"
                        ),
                    glow_sprite = DataBaseManager.Instance.SpriteBase.Find(
                        sp => sp.name == DataBaseManager.GetSkinInfo("", i).Data.PrefabName + "_outer"
                        )
                })
                .ToList();

            scrollView.UpdateData(cellData);
            if (Selected!=null)
            {
                HandleSelectedIndexChanged(scrollView.GetCellData().skinInfo.Data.PlaySkin_Id - 1);
            }
            
        }


        void HandleSelectedIndexChanged(int index)
        {
            Selected = scrollView.GetCellData(index);
            Img_part.gameObject.SetActive((int)Selected.skinInfo.Data.rareType != (int)SkinType.achievable);
            Img_coin.gameObject.SetActive((int)Selected.skinInfo.Data.rareType == (int)SkinType.achievable);
            switch ((SkinType)(int)Selected.skinInfo.Data.rareType)
            {
                case SkinType.basic:
                    priceText.text = "";
                    Img_part.gameObject.SetActive(false);
                    Img_coin.gameObject.SetActive(false);
                    break;
                case SkinType.achievable:
                    priceText.text = Selected.skinInfo.Data.CollectedParts + " / " + Selected.skinInfo.Data.MaxParts;
                    break;
                case SkinType.rare:
                    priceText.text = Selected.skinInfo.Data.CollectedParts + " / " + Selected.skinInfo.Data.MaxParts;
                    break;
                case SkinType.epic:
                    break;
                default:
                    break;
            }

            selectedItemInfo.text = Selected.skinInfo.name;

            scrollView.BtnPurchasedEnable(Selected.skinInfo.Data.MaxParts <= CollectingSystem.Instance.GetRecievedFromCollection(Selected.skinInfo.Data),
                                          Selected.skinInfo.Data.Purchased == 1);

            if (CollectingSystem.Instance.NewOpenSkinsId.Contains(Selected.skinInfo.Data.PlaySkin_Id))
            {                
                CollectingSystem.Instance.NewOpenSkinsId.Remove(Selected.skinInfo.Data.PlaySkin_Id);
                DataKeeper.DeleteKey(GameService.Instance.PlayerName + "_" + Selected.skinInfo.Data.PlaySkin_Id + "_newskin");
            }
        }


      
    }
}
