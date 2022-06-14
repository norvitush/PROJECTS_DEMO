using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VOrb.CubesWar.Levels;

namespace VOrb.CubesWar
{

    public class DataBaseManager : Singlton<DataBaseManager>
    {
        public enum ListType { ItemCube = 1  };

        [SerializeField] private List<GiftItem> CubesDataBuffer = new List<GiftItem>();
        
        public List<LevelInfo> LevelsInfo = new List<LevelInfo>();
        
        private List<SafeGiftItem> _itemCubesData = new List<SafeGiftItem>();
        public List<Sprite> SpritesDataBase = new List<Sprite>();

        [SerializeField] private List<Sprite> _GiftsTextures = new List<Sprite>();
        [SerializeField] private List<Sprite> _HomesTextures = new List<Sprite>();



        public List<SafeGiftItem> SelectAllGameCubesWhere(Func<SafeGiftItem,bool> match, ListType baseType = ListType.ItemCube)
        {
            switch (baseType)
            {                  
                case ListType.ItemCube:
                    return _itemCubesData.Where(match).ToList();                    
                default:
                    break;
            }
            return null;
            
        }

        protected override void Init()
        {
            base.Init();

            GameStorageOperator.LoadJsonFromString("pow33", CubesDataBuffer);
            foreach (var cb in CubesDataBuffer)
            {
                _itemCubesData.Add(cb);
            }
            

            List<Level> Levels = new List<Level>();
            GameStorageOperator.LoadJsonFromString("pow333", Levels);
            
            foreach (var lvl in Levels)
            {
                LevelInfo loadedLevel = new LevelInfo(new List<ILevelFicha>(), lvl.Speed, lvl.LevelNumber, lvl.Giftscount);
                foreach (var strFich in lvl.Fiches)
                {
                    ILevelFicha forAdd = null;
                    switch (Enum.Parse(typeof(ChimneyState), strFich, ignoreCase: true))
                    {
                        case ChimneyState.LightOff:
                            break;
                        case ChimneyState.LightOn:
                            forAdd = new LightHouseChimney();
                            break;
                        case ChimneyState.Smoked:
                            forAdd = new SmokedChimney();
                            break;
                        case ChimneyState.Numered:
                            forAdd = new NumberedHouseChimney();
                            break;
                        default:
                            break;
                    }
                    if (forAdd != null)
                    {
                        loadedLevel.AddFich(forAdd);
                    }
                }
                
                LevelsInfo.Add(loadedLevel);
            }


            var sp = Resources.LoadAll("render", typeof(Sprite));
            foreach (var item in sp)
            {

                if (item.name.Contains("pres_t"))
                {
                    _GiftsTextures.Add((Sprite)item);
                    continue;
                }
                if (item.name.Contains("ho_color"))
                {
                    _HomesTextures.Add((Sprite)item);
                    continue;
                }
                SpritesDataBase.Add((Sprite)item);

            }

        
        }
        //=================================================================

        public Texture2D GetRandomGiftsTexture() => _GiftsTextures[UnityEngine.Random.Range(0, _GiftsTextures.Count)].texture;
        public Texture2D GetRandomHousesTexture() => _HomesTextures[UnityEngine.Random.Range(0, _HomesTextures.Count)].texture;


        public static GiftItem GetCubeItemData(string prefname, int id)
        {
            Predicate<SafeGiftItem> equalNameOrId = (cur) => { return (cur.PrefabName == prefname) || (cur.Id == id); };
            var forret = Instance._itemCubesData.Find(equalNameOrId);
            return forret;
        }

    }

    
}
