using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VOrb.CubesWar.Levels;

namespace VOrb.CubesWar
{

    public class DataBaseManager : Singlton<DataBaseManager>
    {

   
        [SerializeField] private List<SafeGiftItem> _giftsData = new List<SafeGiftItem>();
        [SerializeField] private List<Sprite> _giftsTextures = new List<Sprite>();
        [SerializeField] private List<Sprite> _homesTextures = new List<Sprite>();

        public List<LevelInfo> LevelsInfo = new List<LevelInfo>();
        public List<Sprite> SpritesDataBase = new List<Sprite>();


        protected override void Init()
        {
            base.Init();

            _giftsData = GameStorageOperator.LoadJsonFromString<GiftItem>("pow33").Select(g => (SafeGiftItem)g).ToList();

            List<Level> Levels =   GameStorageOperator.LoadJsonFromString<Level>("pow333");
            
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
                    _giftsTextures.Add((Sprite)item);
                    continue;
                }
                if (item.name.Contains("ho_color"))
                {
                    _homesTextures.Add((Sprite)item);
                    continue;
                }
                SpritesDataBase.Add((Sprite)item);

            }

        
        }
        //=================================================================

        public Texture2D GetRandomGiftsTexture() => _giftsTextures[UnityEngine.Random.Range(0, _giftsTextures.Count)].texture;
        public Texture2D GetRandomHousesTexture() => _homesTextures[UnityEngine.Random.Range(0, _homesTextures.Count)].texture;


        public static GiftItem GetCubeItemData(string prefname, int id)
        {
            Predicate<SafeGiftItem> equalNameOrId = (cur) => { return (cur.PrefabName == prefname) || (cur.Id == id); };
            var forret = Instance._giftsData.Find(equalNameOrId);
            return forret;
        }

    }

    
}
