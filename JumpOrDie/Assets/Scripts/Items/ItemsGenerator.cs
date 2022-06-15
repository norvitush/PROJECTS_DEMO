using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace VOrb
{ 
    public class ItemsGenerator
    {
        private Item _lastItem = null;
        private int _itemSequenceExeption = 2000;

        private Item _nextItem = null;
        public Item NextItem { get => _nextItem; set => _nextItem = value; }

        public void PullUpCoin()
        {
            string tmp = DataBaseManager.Instance.ItemsDataBase.Where(it => it.Id == 2004).FirstOrDefault().PrefabName;            
            string prefPath = "prefabs/" + tmp;
            GameObject newItem = GameObject.Instantiate(Resources.Load(prefPath, typeof(GameObject)), GameService.Instance.LowItemContainer.transform) as GameObject;
        }

        public void GenerateNext()
        {
            Item NextIt = null;
            NextIt = GetRandomItem();
            //Проверка на исключение и повтор
            if (GameService.Instance.ActiveLvlType == LvlType.bonus && NextIt.Id == _itemSequenceExeption)
            {
                NextIt = DataBaseManager.FindItem("Coin", 0);
            }
            else
            {
                if (_lastItem != null)
                {
                    NextIt = (_lastItem.Id == NextIt.Id) && (NextIt.Id == _itemSequenceExeption) ? GetRandomItem() : NextIt;
                }
            }

            _lastItem = NextIt;
            NextItem = NextIt;
            //nextItem= DataBaseManager.FindItem("Shield", 0);
        }

        public void SpawnItem()
        {

            if (GameService.IsItemInScene())
            {
                return;
            }
            if (NextItem != null)
            {
                GameService.Instance.PutItemToGame(NextItem);            
            }
            else
            {
                GenerateNext();                
                GameService.Instance.PutItemToGame(NextItem);
            }

            GenerateNext();            

        }

        public Item GetRandomItem()
        {
            int Skip_id = 2002;
            if (GameService.Instance.ActiveLvlType==LvlType.bonus)
            {
                Skip_id = 0;
            }            
            int Skip_id2 = 2004;

            float sums = DataBaseManager.Instance.ItemsDataBase.
                                 Where(it => it.Id != Skip_id && it.Id != Skip_id2).
                                 Sum(it => it.Chance1_100 * 1000);

            int randItem = Random.Range(0, Mathf.RoundToInt(sums) * 1000) / 1000;

            var ItemsDesc = DataBaseManager.Instance.ItemsDataBase.
                                 Where(it => it.Id != Skip_id && it.Id!= Skip_id2).
                                 OrderByDescending(it => it.Chance1_100);
            var NumeredList =
                from it in ItemsDesc
                select new
                {
                    recno = ItemsDesc.ToList().IndexOf(it) + 1,
                    item = it
                };

            var result = from rec in (
                               from it in NumeredList
                               select new
                               {
                                   it.recno,
                                   it.item,
                                   startPoint = NumeredList.
                                                Where(itn => (itn.item.Id != it.item.Id) && (itn.recno < it.recno)).
                                                Sum((itn1) => itn1.item.Chance1_100) * 1000,
                                   endPoint = (NumeredList.
                                                Where(itn => (itn.item.Id != it.item.Id) && (itn.recno < it.recno)).
                                                Sum((itn1) => itn1.item.Chance1_100) + it.item.Chance1_100) * 1000,
                               }
                         )
                         where (rec.startPoint <= randItem) && (randItem <= rec.endPoint)
                         select rec.item;
            return result.FirstOrDefault(it => it.Id > 0);
        }
    }
}

