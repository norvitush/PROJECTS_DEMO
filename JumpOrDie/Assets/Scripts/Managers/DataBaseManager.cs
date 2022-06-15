using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

namespace VOrb
{

    public class DataBaseManager : Singlton<DataBaseManager>
    {

        [SerializeField] private PLayerCollectionInfo collectionInfo;

                     
        public List<Challenge> _allChalenges = new List<Challenge>();        
        private Dictionary<int, string> _skinNames = new Dictionary<int, string>();

        public List<FonInfo> AllFons = new List<FonInfo>();
        public List<Item> ItemsDataBase = new List<Item>();
        public List<Sprite> SpriteBase = new List<Sprite>();

        public int PlayerProgress
        {
            get
            {
                return collectionInfo.progress;
            }
        }
        
        protected override void Init()
        {
            base.Init();
            collectionInfo = new PLayerCollectionInfo("");

            
            List<SkinView> _skinsDataBase = new List<SkinView>();
            LoadJsonFromString("MIRA2G134", _skinsDataBase);
            LoadJsonFromString("MIRA2G133", ItemsDataBase);
            LoadJsonFromString("MIRA2G135", AllFons);
            LoadJsonFromString("MIRA2G136", _allChalenges);

            foreach (var sk in _skinsDataBase)
           {
                collectionInfo.Skins.Add(sk.RareType != SkinType.basic 
                                            ? (SafeSkinData)sk
                                            : new SafeSkinData(sk.PrefabName,sk.Id,0,0,sk.MatColor,1));
                _skinNames.Add(sk.Id,sk.SkinName);
           }

            if (_skinsDataBase.Count(s=> s.Id>0) == 0)
            {
                collectionInfo.Skins.Add(new SafeSkinData("av_peng", 1, 0, 0, new Color32(175, 100, 255, 255), 1));
            }


            var sp = Resources.LoadAll("render",typeof(Sprite));
            foreach (var item in sp)
            {
                SpriteBase.Add((Sprite)item);
            }

         
        }

        private IEnumerable<TDat> LoadJsonFromString<TDat>(string file, List<TDat> output)
        {
            TextAsset mytxtData = (TextAsset)Resources.Load(file);
            string txt = mytxtData.text;

            //string realtxt2 = DataKeeper.UncryptStr(txt2);
            IEnumerable<TDat> forret = JsonConvert.DeserializeObject<TDat[]>(txt);
            foreach (var item in forret)
            {
                output.Add(item);
            }
            return forret;
        }




        //СОХРАНЕНИЕ И ЗАГРУЗКА СОЛЛЕКЦИИ ИГРОКА
        public bool IsPlayerSaved(string PlayerName)
        {
            return (int)DataKeeper.LoadParam(PlayerName + "_", 0) != 0;    // если есть val =1  иначе дефалт - значит не сохраняли  
        }



        public static int SkinEnabled(SafeSkinData skin)
        {
            return (skin.MaxParts == skin.CollectedParts) ? 1: 0;
        }
        
        public void LoadCollection(string PlayerName)
        {
            if (IsPlayerSaved(PlayerName))
            {
                CollectingSystem.Instance.NewOpenSkinsId.Clear();
                var Stored = collectionInfo.Skins.ToList();
                collectionInfo.Skins.Clear();
                
                collectionInfo.PlayerName = PlayerName;
                collectionInfo.progress = (int)DataKeeper.LoadParam(PlayerName + "_progress", 0);
                collectionInfo.coins = (int)DataKeeper.LoadParam(PlayerName + "_coins", 0);
                collectionInfo.parts = (int)DataKeeper.LoadParam(PlayerName + "_parts", 0);


                int balance = collectionInfo.coins;
                int balance_parts = collectionInfo.parts;

                var SkinsIncrem = Stored.
                                 OrderBy(sk => (int)sk.PlaySkin_Id);
                foreach (var skin in SkinsIncrem)
                {
                    
                    SafeSkinData curSkin = new SafeSkinData(skin.PrefabName, skin.PlaySkin_Id, skin.MaxParts,0,skin.matColor,0,skin.rareType);                    


                    switch ((SkinType)(int)skin.rareType)
                    {
                        case SkinType.basic:
                            curSkin.Purchased = 1;
                            curSkin.Enabled = 1;                            
                            break;
                        case SkinType.achievable:
                            curSkin.Purchased = (int)DataKeeper.LoadParam(PlayerName + "_" + skin.PlaySkin_Id + "_purchased", 0);
                            if (curSkin.Purchased != 1)
                            {
   
                                    if (curSkin.MaxParts <= balance)
                                    {
                                        curSkin.CollectedParts = curSkin.MaxParts;  // монет хватило - скин заполнен
                                    }
                                    else
                                    {
                                        curSkin.CollectedParts = balance; // не хватило
                                    }
     
                            }
                            else
                            {
                                curSkin.CollectedParts = curSkin.MaxParts;
                              
                            }
                            curSkin.Enabled = SkinEnabled(curSkin);
                            break;
                        case SkinType.rare:
                            

                            curSkin.Purchased = (int)DataKeeper.LoadParam(PlayerName + "_" + skin.PlaySkin_Id + "_purchased", 0);                           
                            if (curSkin.Purchased != 1)
                            {                                
                                if (curSkin.MaxParts <= balance_parts)
                                {
                                    curSkin.CollectedParts = curSkin.MaxParts;  // кусков хватило - скин заполнен
                                }
                                else
                                {
                                    curSkin.CollectedParts = balance_parts; // не хватило
                                }
                            }
                            else
                            {
                                curSkin.CollectedParts = curSkin.MaxParts;
                            }

                            curSkin.Enabled = SkinEnabled(curSkin);
                            break;
                        case SkinType.epic:
                            curSkin.Purchased = (int)DataKeeper.LoadParam(PlayerName + "_" + skin.PlaySkin_Id + "_purchased", 0);
                            curSkin.Enabled = curSkin.Purchased;
                            break;
                        default:
                            break;
                    }
                    collectionInfo.Skins.Add(curSkin);
                    if ((int)DataKeeper.LoadParam(PlayerName + "_" + skin.PlaySkin_Id + "_newskin", 0)==1)
                    {
                        CollectingSystem.Instance.NewOpenSkinsId.Add(curSkin.PlaySkin_Id);
                    }
                }
                Debug.Log(collectionInfo.ToString());
                RecalcLastNotFilled();
                
            }
        }
        public void SaveCollectionWithName(string Name, bool GainOnly = false)
        {
            PLayerCollectionInfo temp = collectionInfo;
            temp.PlayerName = Name;

            DataKeeper.SaveParam(Name + "_",1);            // самого игрока того..

            DataKeeper.SaveParam(Name + "_progress",temp.progress);
            DataKeeper.SaveParam(Name + "_coins", temp.coins);
            DataKeeper.SaveParam(Name + "_parts", temp.parts);
            if (!GainOnly)
            {
                foreach (var skin in temp.Skins)
                {
                    if (CollectingSystem.Instance.NewOpenSkinsId.Contains(skin.PlaySkin_Id) && skin.Enabled==0)
                    {
                        CollectingSystem.Instance.NewOpenSkinsId.Remove(skin.PlaySkin_Id);
                        DataKeeper.SaveParam(Name + "_" + skin.PlaySkin_Id + "_newskin", 0);
                    }
                    else
                    {
                        DataKeeper.SaveParam(Name + "_" + skin.PlaySkin_Id + "_newskin",
                                             CollectingSystem.Instance.NewOpenSkinsId.Contains(skin.PlaySkin_Id) ? 1 : 0);
                    }
                    DataKeeper.SaveParam(Name + "_" + skin.PlaySkin_Id + "_collected", skin.CollectedParts);
                    DataKeeper.SaveParam(Name + "_" + skin.PlaySkin_Id + "_max", skin.MaxParts);
                    DataKeeper.SaveParam(Name + "_" + skin.PlaySkin_Id + "_purchased", skin.Purchased);

                }
            }
            

        }
        public void DropSavedPlayerInfo(string PlayerName)
        {
            if (IsPlayerSaved(PlayerName))
            {
                DataKeeper.DeleteKey(PlayerName + "_progress");
                DataKeeper.DeleteKey(PlayerName + "_coins");
                DataKeeper.DeleteKey(PlayerName + "_parts");


                foreach (var skin in collectionInfo.Skins)
                {
                    DataKeeper.DeleteKey(PlayerName + "_" + skin.PlaySkin_Id + "_collected");
                    DataKeeper.DeleteKey(PlayerName + "_" + skin.PlaySkin_Id + "_max");
                    DataKeeper.DeleteKey(PlayerName + "_" + skin.PlaySkin_Id + "_purchased");
                    DataKeeper.DeleteKey(PlayerName + "_" + skin.PlaySkin_Id + "_newskin");
                }
               
                DataKeeper.DeleteKey(PlayerName + "_");            // и самого игрока того..
            }
            DataKeeper.DeleteKey(PlayerName + "_Lvl_gainedcoins");
            DataKeeper.DeleteKey(PlayerName  + "_Lvl_gainedparts");            
            DataKeeper.DeleteKey(PlayerName + "_Lvl_complete");
            DataKeeper.DeleteKey(PlayerName + "_avatar");
            DataKeeper.DeleteKey("easymode");
            DataKeeper.DeleteKey("tutor");
            DataKeeper.DeleteKey(PlayerName + "_CurrentFonNumber");
            DataKeeper.DeleteKey(PlayerName + "_challenge");
            DataKeeper.DeleteKey(PlayerName + "_BestTime");
        }

        internal CollectingSystem.Recieved GetCoinsAndParts()
        {
            var forReturn = new CollectingSystem.Recieved
            {
                coins = Instance.collectionInfo.coins,
                parts = Instance.collectionInfo.parts
            };
            return forReturn;
        }

        public static void DebugCollection()
        {
            Debug.Log(Instance.collectionInfo.ToString() + " " + Time.time);
        }
        //                 ОБНОВЛЕНИЕ ДАННЫХ КОЛЛЕКЦИИ ИГРОКА
        public List<SafeSkinData> GetPlayerSkinsCollection()
        {
            return collectionInfo.Skins;
        }
        public int GetLastNotFilled()
        {
            return collectionInfo.LastNotFilled_id;
        }

        public void AddToCollection(int coins, int parts)
        {
            collectionInfo.coins += coins;
            collectionInfo.progress += (coins+parts);
            collectionInfo.parts += parts;
        }

        public void SubstructFromCollection(int coins, int parts)
        {
            collectionInfo.coins -= coins;            
            collectionInfo.parts -= parts;
        }
       
        public void SetOwnerCollection(string owner)
        {
            collectionInfo.PlayerName = owner;
            RecalcLastNotFilled();
        }
        public void RecalcLastNotFilled()
        {
            int balance = collectionInfo.coins;
            int newLast = Instance.collectionInfo.Skins.
                                     Where(sk => sk.rareType != (int)SkinType.epic 
                                     && sk.rareType != (int)SkinType.basic 
                                     && sk.MaxParts > balance
                                     && sk.Purchased!=1).
                                     OrderBy(sk => (int)sk.PlaySkin_Id).FirstOrDefault().PlaySkin_Id;
            collectionInfo.LastNotFilled_id = newLast==0 ? -1 : newLast;            
        }


        //=================================================================

        internal Challenge FindChallenge(int searchId)
        {
            return _allChalenges.FirstOrDefault(ch => ch.id == searchId);
        }
        public static SkinInfo GetSkinInfo(string name, int id)
        {
            Predicate<SafeSkinData> equalNameOrId = (cur) => { return (cur.PrefabName == name) || (cur.PlaySkin_Id == id); };
            SafeSkinData forret = Instance.collectionInfo.Skins.Find(equalNameOrId);
            string outpStr = "";
            Instance._skinNames.TryGetValue(id, out outpStr);
            return new SkinInfo(forret, outpStr);
        }
        public static SafeSkinData FindByRareType(SkinType rare)
        {            
            Func<SafeSkinData, bool> equalRare = (cur) => { return cur.rareType == (int)rare; };
            SafeSkinData forret = Instance.collectionInfo.Skins.FirstOrDefault(equalRare);
          
            return forret;
        }
        public static Item FindItem(string name, int id)
        {
            Predicate<Item> equalNameOrId = (cur) => { return (cur.Name == name) || (cur.Id == id); };
            return Instance.ItemsDataBase.Find(equalNameOrId);
        }
        public string Serialize<T>(T _object)
        {
            string tmp = JsonConvert.SerializeObject(_object);
           // Debug.Log(tmp);
            return tmp;
        }

       
        public void AddItem(Item itm)
        {
            ItemsDataBase.Add(itm);
        }

    }


    [System.Serializable]
    public class SkinView
    {
        [Tooltip("Имя для поиска в префабах Resources")] 
        public string PrefabName;
        public int Id;
        public int FullSkinParts;
        public Color32 MatColor;
        public SkinType RareType;
        public string SkinName;

        public SkinView() { }

    }

    public class SkinInfo
    {        
        public SafeSkinData Data;
        public string name;

        public SkinInfo(SafeSkinData skinData, string name)
        {
            this.Data = skinData;
            this.name = name;
        }
    }

    [System.Serializable]
    public class FonInfo
    {
        public string PrefabName;
        public int Id;

        public FonInfo(int id, string prefab)
        {
            this.PrefabName = prefab;
            Id = id;
        }
    }
}
