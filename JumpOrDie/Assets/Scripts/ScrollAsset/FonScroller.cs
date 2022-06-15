using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace VOrb
{
    public class FonScroller : MonoBehaviour
    {
        private const int FILLED_COST = 5000;
        private const int THEME_ADD_CNT = 500;
        [SerializeField]
        FonScrollView scrollView = null;
        [SerializeField]
        Button prevCellButton = null;
        [SerializeField]
        Button nextCellButton = null;
       // [SerializeField]
       // Text selectedItemInfo = null;
        public FonCellData Selected = null;
        public TextMeshProUGUI priceText = null;

        public Image Img_coin = null;

        public Image Img_part = null;
        private GameObject pointer;

        void Start()
        {

            pointer = UIWindowsManager.GetWindow<AvatarWindow>().points[0].transform.parent.Find("Pointer").gameObject;            
            prevCellButton?.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton?.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);
            List<FonInfo> Fons = DataBaseManager.Instance.AllFons;
            //SceneLoader.Debuger("Список загруженных фонов: " + Fons.Count);
            var cellData = Enumerable.Range(1, Fons.Count)
                .Select(i => new FonCellData
                {
                    fon = DataBaseManager.Instance.AllFons.FirstOrDefault((f_id) => f_id.Id == i),                    
                    fon_sprite =  DataBaseManager.Instance.SpriteBase.Find(
                        sp => sp.name == DataBaseManager.Instance.AllFons.FirstOrDefault((f_id) => f_id.Id == i).PrefabName + "_img"
                        )
                })
                .ToList();

            scrollView.UpdateData(cellData);
            scrollView.UpdateSelection(GameService.Instance.CurrentFonNumber - 1);
        }
        public void UpdateCellsData()
        {
            //List<FonInfo> Fons = DataBaseManager.Instance.AllFons;
            ////SceneLoader.Debuger("Список загруженных фонов: " + Fons.Count);
            //var cellData = Enumerable.Range(1, Fons.Count)
            //    .Select(i => new FonCellData
            //    {
            //        fon = DataBaseManager.Instance.AllFons.FirstOrDefault((f_id) => f_id.Id == i),
            //        fon_sprite = DataBaseManager.Instance.SpriteBase.Find(
            //            sp => sp.name == DataBaseManager.Instance.AllFons.FirstOrDefault((f_id) => f_id.Id == i).PrefabName + "_img"
            //            )
            //    })
            //    .ToList();

            //scrollView.UpdateData(cellData);
            if (Selected != null)
            {
                HandleSelectedIndexChanged(scrollView.GetCellData().fon.Id - 1);
            }
            priceText.text = DataBaseManager.Instance.PlayerProgress.ToString();
            UIWindowsManager.GetWindow<AvatarWindow>().GameProgressBar.fillAmount = Mathf.Clamp01((float)DataBaseManager.Instance.PlayerProgress / FILLED_COST);

            int max_opened = Mathf.CeilToInt(DataBaseManager.Instance.PlayerProgress / THEME_ADD_CNT) + 1;
            var point_arr = UIWindowsManager.GetWindow<AvatarWindow>().points;
            for (int i = 0; i < point_arr.Length; i++)
            {
                point_arr[i].SetActive(i<max_opened);
            }

            


        }


        void HandleSelectedIndexChanged(int index)
        {
             Selected = scrollView.GetCellData(index);
            
            pointer.transform.localPosition = new Vector2(UIWindowsManager.GetWindow<AvatarWindow>().
                             points[index].transform.localPosition.x,
                             pointer.transform.localPosition.y);

            //selectedItemInfo.text = Selected.fon.PrefabName;            

            if (Selected.fon.Id <= CollectingSystem.Instance.RateProgress(true))
            {
                // меняем фон
                UIWindowsManager.GetWindow<MainWindow>().SetFon(Selected.fon.Id);
                
            }

        }



    }
}
