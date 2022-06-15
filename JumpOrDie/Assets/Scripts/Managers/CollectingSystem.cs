using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace VOrb
{

    class CollectingSystem : Singlton<CollectingSystem>
    {
        public struct Recieved
        {
            public SafeInt coins;
            public SafeInt parts;
            public SafeInt bonus;
        }

        public SafeInt Lvl_gainedcoins
        {
            get
            {
                return Lvl_Gained.coins;
            }
            private set            
            {
                Lvl_Gained.coins = value;
            }
        }
        public SafeInt Lvl_gainedparts
        {
            get
            {
                return Lvl_Gained.parts;
            }
            private set
            {
                Lvl_Gained.parts = value;
            }
        }
        public SafeInt Lvl_bonusparts
        {
            get
            {
                return Lvl_Gained.bonus;
            }
            private set
            {
                Lvl_Gained.bonus = value;
            }
        }
       

        [SerializeField]private Recieved Lvl_Gained;

        public List<int> NewOpenSkinsId = new List<int>();
        public int NewSkinsCount => NewOpenSkinsId.Count;

        public static ItemActionState[] OnItemCollect =
        {
            new ItemActionState(OnCollectCoin),
            new ItemActionState(OnCollectShield),
            new ItemActionState(OnCollectPart),
            new ItemActionState(()=>{}),
            new ItemActionState(OnCollectBonusCoin)
        };

        private static void OnCollectBonusCoin()
        {
            GameService.Instance.CleanContainer(GameService.Instance.LowItemContainer);
            switch (GameService.Instance.ActiveLvlType)
            {
                case LvlType.simple:
                    Instance.Lvl_gainedcoins += Instance.RateProgress();
                    UIEffects.SplashGainCoin("x" + Instance.RateProgress());

                    break;
                case LvlType.bonus:
                    Instance.Lvl_gainedcoins += Instance.RateProgress() * 2;
                    // UIEffects.SplashMainScreen("WoW X2!");
                    UIEffects.SplashGainCoin("x"+(Instance.RateProgress() * 2));


                    break;
                case LvlType.extrahard:
                    break;
            }
            GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(Instance.Lvl_gainedcoins);
            AudioServer.PlaySound(Sound.CoinCollect, 0.5f);
        }

        protected override void Init()
        {
            base.Init();           
        }

        private void Start()
        {
            Lvl_Gained = new Recieved();
            //Lvl_gainedcoins = 0;            
        }

        //==========  DataKeeper with DataBase
        public void SetPlayerAvatar(int id)
        {
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_avatar", id);
        }
        public bool GetPlayerAvatar( out SafeSkinData skin)
        {
            bool IsSkinFound = true;
            int av_id =0;

            if (DataBaseManager.Instance.IsPlayerSaved(GameService.Instance.PlayerName))
            {
                av_id= (int)DataKeeper.LoadParam(GameService.Instance.PlayerName + "_avatar", 0);
                if (av_id == 0)
                    IsSkinFound = false;
            }
            else
            {
                IsSkinFound = false;
            }           
            int cur_skinId = IsSkinFound ? av_id : SceneLoader.sceneSettings.DefaultSkin.Id;
            skin = DataBaseManager.GetSkinInfo("", cur_skinId).Data;
            return (skin.PlaySkin_Id!=0);

        }

        public void StoreLvlResult(bool lvlComplete = false)
        {
            
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_Lvl_complete", lvlComplete ? 1 : 0);
            if (lvlComplete)
            {
                Lvl_gainedcoins = 0;
                Lvl_gainedparts = 0;
                Lvl_bonusparts = 0;
            }
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_Lvl_gainedcoins", (int)Lvl_gainedcoins);
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_Lvl_gainedparts", (int)Lvl_gainedparts);
        }
        public bool CheckLvlComplete()
        {
            
            return (int)DataKeeper.LoadParam(GameService.Instance.PlayerName + "_Lvl_complete", 1) == 1;
        }

        public void RestoreLvlResults()
        {
            Lvl_gainedcoins = (int)DataKeeper.LoadParam(GameService.Instance.PlayerName + "_Lvl_gainedcoins", 0 );
            Lvl_gainedparts = (int)DataKeeper.LoadParam(GameService.Instance.PlayerName + "_Lvl_gainedparts", 0 );
            DataKeeper.SaveParam(GameService.Instance.PlayerName + "_Lvl_complete", 0); // уровень не закончен
        }

 
        public void DeletePlayerInfo()
        {
            DataBaseManager.Instance.DropSavedPlayerInfo(SceneLoader.sceneSettings.PlayerName);
        }

        public void InitPlayerCollection(string player)
        {



            if (SceneLoader.sceneSettings.IsTestMode)
            {                
                DataBaseManager.Instance.AddToCollection(20000 , 20000);
                DataBaseManager.Instance.SaveCollectionWithName( GameService.Instance.PlayerName,true);
            }


            if (DataBaseManager.Instance.IsPlayerSaved(player))
            {
                
                DataBaseManager.Instance.LoadCollection(player);
            }
            else
            {
                DataBaseManager.Instance.SetOwnerCollection(player);
            }
            
        }


        // ========================


        public void AddGained(int AddCoins, int AddParts, int bonusParts=0)
        {
            Lvl_gainedcoins += AddCoins;
            Lvl_gainedparts += AddParts;
            Lvl_bonusparts += bonusParts;
        }

        public void OnCollect(Item itm)
        {

                OnItemCollect[(int)itm.ItemAction].action.Invoke();

                if (itm.ItemAction != ItemAction.BonusCoinCollect)
                {
                    GameService.Instance.PutItemToGame(null);
                    if (GameService.Instance.ActiveLvlType == LvlType.simple)
                    {
                        StartCoroutine(PutNewItem(SceneLoader.sceneSettings.NewItemDelay));
                    }
                    else
                    {
                        StartCoroutine(PutNewItem(2f));
                    }

                }      
           
        }
        //==================================================
        public int RateProgress(bool forFons = false)
        {
            int cur_progress = DataBaseManager.Instance.PlayerProgress;
            if (forFons)
            {
                return Mathf.CeilToInt(cur_progress / 500)+1;
            }
            else
            {
                if (cur_progress < 1000)
                {
                    return 1;
                }
                else if (cur_progress >= 1000 && cur_progress < 5000)
                {
                    return 2;
                }
                else if (cur_progress >= 5000)
                {
                    return 3;
                }
            }           
            
            return 0;
        }
        private static void OnCollectCoin()
        {
            AudioServer.PlaySound(Sound.CoinCollect, 0.8f);
            //DataBaseManager.Instance.AddToCollection(1,0);
            int AddRated = Instance.RateProgress();
            switch (GameService.Instance.ActiveLvlType)
            {
                case LvlType.simple:

                    Instance.Lvl_gainedcoins +=  AddRated;
                    UIEffects.SplashGainCoin("x"+AddRated);
                    break;
                case LvlType.bonus:
                    Instance.Lvl_gainedcoins+= Instance.RateProgress()*2;
                    UIEffects.SplashGainCoin("x" + AddRated*2);


                    break;
                case LvlType.extrahard:
                    break;                     
            }
            
            GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(Instance.Lvl_gainedcoins);
            GameService.Instance.PartsTextMesh.text = Instance.Lvl_gainedparts.ToString();
            //GameService.Instance.CoinsTextMesh.gameObject.GetComponent<Animator>().SetBool("new_gain",true);

        }
        private static void OnCollectShield()
        {
            if (!GameService.Instance.TutorialEnded)
            {
                GameService.Instance.TutorialContainer.SetActive(true);
                GameService.Instance.StartCoroutine(GameService.Instance.AvatarControl.WaitNearestShake());
            }
            AudioServer.PlaySound(Sound.ShieldCollect);
            GameService.Instance.AvatarControl.ShieldEffect.SetActive(false);
            CutScript Cutter =  GameService.Instance.AvatarControl.Cutter;
            GameService.Instance.AvatarControl.ShieldEffect.SetActive(true);
            Cutter.MakeCuttedAgain();
            
            Cutter.SetUncuttedForTime(SceneLoader.sceneSettings.ShieldTime);
           
            //GameObject avatar = GameService.Instance.GetAvatarObj();
            // GameObject newItem = Instantiate(Resources.Load("prefabs/VFX/shield_particle", typeof(GameObject)), avatar.transform) as GameObject;
        }

        private static void OnCollectPart()
        {
            AudioServer.PlaySound(Sound.PartCollect, 0.5f);
            // DataBaseManager.Instance.AddToCollection(0, 1);
            switch (GameService.Instance.ActiveLvlType)
            {
                case LvlType.simple:
                    Instance.Lvl_gainedparts++;
                    break;
                case LvlType.bonus:
                    Instance.Lvl_gainedparts+=2;
                    UIEffects.SplashMainScreen("WoW X2!");
                    break;
                case LvlType.extrahard:
                    break;
            }
            

            GameService.Instance.PartsTextMesh.text = Instance.Lvl_gainedparts.ToString();

        }
        IEnumerator PutNewItem(float delay)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(delay,delay*2)-0.8f);
            yield return StartCoroutine(nameof(PlayItemPreView));
            GameService.Instance.itemGenerator.SpawnItem();
            UIWindowsManager.GetWindow<MainWindow>().ItemPreViewEffect.gameObject.SetActive(false);
        }

        IEnumerator PlayItemPreView()
        {
            Color trailscolor;
            if (GameService.Instance.itemGenerator.NextItem!=null)
            {
                trailscolor = GameService.Instance.itemGenerator.NextItem.PreViewColor;
            }
            else
            {
                trailscolor = new Color32(255, 165, 0, 255);
            }

            ParticleSystem baseParticles = UIWindowsManager.GetWindow<MainWindow>().ItemPreViewEffect;            
            ParticleSystem subParticles = baseParticles.transform.Find("boom").GetComponent<ParticleSystem>();

            //base
            var base_myTrail = baseParticles.trails;
            var base_main = baseParticles.main;

            base_main.startColor = trailscolor;           

            base_myTrail.colorOverLifetime = trailscolor;


            //subs
            var myTrail = subParticles.trails;            
            var main = subParticles.main;

            main.startColor = trailscolor;

            Gradient gradient = new Gradient();

            gradient.SetKeys(
               new GradientColorKey[] { new GradientColorKey(trailscolor, 0.0f), new GradientColorKey(trailscolor, 1.0f) },
                myTrail.colorOverLifetime.gradient.alphaKeys 
           );

            myTrail.colorOverLifetime = gradient;



            //localeffect.GetComponent<ParticleSystem>().trails
            AudioServer.PlaySound(Sound.Preview);
            baseParticles.gameObject.SetActive(true);



            yield return new WaitForSeconds(1f);          
        }

            public void SpendBalance(int value, SafeSkinData targetSkin)
        {
            Color BaseStartscolor = new Color32(144,204,241,255); 
            Color SubStartscolor = new Color32(38, 129, 185, 255); 

            switch ((SkinType)(int)targetSkin.rareType)
            {
                case SkinType.basic:
                    break;
                case SkinType.achievable:
                    DataBaseManager.Instance.SubstructFromCollection(value, 0);
                     BaseStartscolor = new Color32(241, 235, 144, 255); 
                     SubStartscolor = new Color32(149, 60, 0, 255); 
                    break;
                case SkinType.rare:
                    DataBaseManager.Instance.SubstructFromCollection(0, value);
                    BaseStartscolor = new Color32(144, 241, 165, 255); 
                    SubStartscolor = new Color32(10, 164, 44, 255);
                    break;
                case SkinType.epic:
                    break;
                default:
                    break;
            }

            ParticleSystem baseParticles = UIWindowsManager.GetWindow<RestartWindow>().NewSkin;
            ParticleSystem subParticles = baseParticles.transform.Find("Light_circle").GetComponent<ParticleSystem>();

            //base
            var base_main = baseParticles.main;
            var base_sub = subParticles.main;

            base_main.startColor = BaseStartscolor;
            base_sub.startColor = SubStartscolor;

            

            baseParticles.gameObject.SetActive(false);
           baseParticles.gameObject.SetActive(true, 2f, this, ()=> {
                Color Startscolor = new Color32(144, 204, 241, 255);
                Color Subcolor = new Color32(38, 129, 185, 255);
                ParticleSystem baseP = UIWindowsManager.GetWindow<RestartWindow>().NewSkin;
                ParticleSystem subP = baseParticles.transform.Find("Light_circle").GetComponent<ParticleSystem>();

                //base
                var basemain = baseP.main;
                var basesub = subP.main;

                basemain.startColor = Startscolor;
                basesub.startColor = Subcolor;
            });
            baseParticles.Play();

        }
        public void UpdateAllSkinsCollection()
        {
           var originSkinBase = DataBaseManager.Instance.GetPlayerSkinsCollection();

            var calculated = originSkinBase.
                 Where(sk => (int)sk.Purchased == 0 && ((int)sk.rareType == (int)SkinType.achievable || (int)sk.rareType == (int)SkinType.rare)).
                 Select(nsk =>
                 new SafeSkinData(
                        nsk.PrefabName, nsk.PlaySkin_Id, nsk.MaxParts,
                        Mathf.Clamp(GetRecievedFromCollection(nsk), 0, nsk.MaxParts)
                        , nsk.matColor, 0, nsk.rareType)
                 ).
                 OrderBy(sk => (int)sk.PlaySkin_Id).ToList();

            foreach (var calc_skin in calculated)
            {
                UpdateCollectionSkin(originSkinBase, calc_skin);
            }
            originSkinBase.Sort();

            DataBaseManager.Instance.RecalcLastNotFilled();
        }
        internal void SaveCurrentGainToCollection()
        {

            int sum_tickets = Lvl_gainedparts + Lvl_bonusparts;

            //ОБНОВЛЕНИЕ КОЛЛЕКЦИИ ИЗ ПОСЧИТАННЫХ СКИНОВ И ПОЛУЧЕНННЫХ ОЧКОВ
            DataBaseManager.Instance.AddToCollection(Lvl_gainedcoins, sum_tickets);
            UpdateAllSkinsCollection();
            //-------------------------------------------------------------------
            DataBaseManager.Instance.SaveCollectionWithName(SceneLoader.sceneSettings.PlayerName);

           
            #region PlayServices
            if (GameService.Instance.PlayServicesConnected)
            {                
                if (Lvl_gainedcoins == 1 )
                {
                    //ACHIVMENTS
                }
                if (Lvl_gainedcoins == 777)
                {
                    //ACHIVMENTS
                }
                if (RateProgress(forFons: true) > 9)
                {
                    //ACHIVMENTS
                }
                if (Lvl_gainedcoins >=100)
                {
                    SafeSkinData current;
                    GetPlayerAvatar(out current);
                    if (current.PrefabName == "av_frog")
                    {
                        //ACHIVMENTS
                    }
                }
               
            }
            #endregion
            //--------------------------------------------------------------------
            StoreLvlResult(true);
            GameService.Instance.ActiveLvlType = LvlType.simple;
            UIWindowsManager.GetWindow<AvatarWindow>().UpdateScrollWiew(); //обновление данных скинов в карусели
        }
    

        public void UpdateCollectionSkin(List<SafeSkinData> OriginSkinBase, SafeSkinData newSkin)
        {
            if (OriginSkinBase==null)
            {
                OriginSkinBase = DataBaseManager.Instance.GetPlayerSkinsCollection();
            }
            SafeSkinData upd_skin = DataBaseManager.GetSkinInfo("", newSkin.PlaySkin_Id).Data;
            if (upd_skin.PlaySkin_Id != 0) // есть что апдейтить
            {
                OriginSkinBase.RemoveAll(sk => { return sk.PlaySkin_Id == upd_skin.PlaySkin_Id; });
                OriginSkinBase.Add(newSkin);
            }
        }
        public Recieved GetRecievedFromCollection()
        {
           return DataBaseManager.Instance.GetCoinsAndParts();
        }
        public int GetRecievedFromCollection(SafeSkinData testSkin)
        {
            var GainFormCollection = DataBaseManager.Instance.GetCoinsAndParts();
            switch ((SkinType)(int)testSkin.rareType)
            {
                case SkinType.basic:
                    return GainFormCollection.coins;
               
                case SkinType.achievable:
                    return GainFormCollection.coins;
           
                case SkinType.rare:
                    return GainFormCollection.parts;
             
                case SkinType.epic:
                    break;
                default:
                    break;
            }
            return 0;            
        }


    }
        public class ItemActionState
        {
            public delegate void OnCollect();
            public OnCollect action;

            public ItemActionState(OnCollect param)
            {
                action = param;
            }
        }
}
