using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace VOrb
{
    public class RestartWindow : Window
    {
        const float AFTER_FULL_PAUSE = 1f;
        private readonly BitOperations _window_settings = new BitOperations();

        [SerializeField] private Animator _viewAnimator;
        [SerializeField] private ParticleSystem _newSkin;

        private DialogWindow _dialog_window;
        private UnityAction _closeCallback;

        [SerializeField] private GameObject _avatarContainer;

        [SerializeField] private GameObject _btnBonus;
        [SerializeField] private GameObject _btnHome;
        [SerializeField] private GameObject _btnX2;

        private Image _btnHomeImg;
        [SerializeField] private GameObject _fon;

        //----------------------------------
        private SpriteRenderer skin_img;
        private SpriteRenderer skin_outer;
        private TextMeshProUGUI coins_text;
        private List<SafeSkinData> AchivableCalcSkins;
        private List<SafeSkinData> RareCalcSkins;
        //private List<SafeSkinData> ShowedFullSkins;

        [SerializeField] private TextMeshProUGUI _uiHeaderText;
        private IEnumerator ImgFillCorout;
        private IEnumerator CalcAnimCorout;
        private IEnumerator GoOutCoroutine;
        private bool isSkipTap = false;

        public BitOperations Window_settings  => _window_settings; 
        public ParticleSystem NewSkin  => _newSkin;  
        public UnityAction CloseCallback { private get => _closeCallback; set => _closeCallback = value; }
        public GameObject AvatarContainer => _avatarContainer;
        public GameObject Fon => _fon;

        protected override void SelfClose()
        {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                CloseCallback?.Invoke();
            }      
        }
    

        protected override void SelfOpen()
        {
            UIWindowsManager.GetWindow<MainWindow>().Btn_timer.SetActive(false);
            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            mn.BonusFon.SetActive(false);
            mn.BonusText.gameObject.SetActive(false);

            _dialog_window = UIWindowsManager.GetWindow<DialogWindow>();

            CalcAnimCorout = null;
            

            GameService.Instance.ActiveLvlType = LvlType.simple;

            _uiHeaderText.gameObject.SetActive(false);
            gameObject.SetActive(true);
            
            //генерация заголовка окна
            GenerateHeader();

            if (AvatarContainer == null)
            {
                ContainerUpdate();
            }
            coins_text = AvatarContainer.transform.Find("COINS_TEXT").gameObject.GetComponent<TextMeshProUGUI>();
            skin_img = AvatarContainer.transform.Find("SKIN_IMG").gameObject.GetComponent<SpriteRenderer>();
            skin_outer = AvatarContainer.transform.Find("SKIN_OUTER").gameObject.GetComponent<SpriteRenderer>();
            _btnHomeImg = _btnHome.GetComponentInChildren<UIElement>().GetComponent<Image>();
            _btnHomeImg.fillAmount = 1f;
            //настройки видимости

            AvatarContainer.gameObject.SetActive(Window_settings.Contains(Restart_settings.AVATAR_CONTAINER));

            //настройка ивентов
            TapEvent.RemoveAllListeners();
            SvipeEvent.RemoveAllListeners();
            TapEvent.AddListener(WhenTapOrSvipe);
            SvipeEvent.AddListener(WhenTapOrSvipe);


            // CollectingSystem.Instance.Lvl_gainedcoins = 50;
            ShowAllButtons(false);


            bool isRewarded = GameService.Instance.challengeServer.CurrentDone;
            if (GameService.Instance.challengeServer.GetReward()>0 && !isRewarded)
            {                
                UIWindowsManager.ActiveNow = _dialog_window;               
                _dialog_window.Open(ChangeCurrentWindow);
                AudioServer.PlaySound(Sound.ChallengeDone);
            }
            else
            {
                ShowSkinFilling(5f);
            }
        }
      
        public void DropImgToFirst()
        {
            if (ImgFillCorout!=null)
            {
                StopCoroutine(ImgFillCorout);
            }
            var calc_sk = AchivableCalcSkins.FirstOrDefault();
            if (calc_sk.MaxParts==0)
            {
                AvatarContainer.SetActive(false);
                return;
            }
            skin_img.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == calc_sk.PrefabName + "_img");
            skin_outer.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == calc_sk.PrefabName + "_outer");
            skin_img.material.SetFloat("_xoffset", 0);
        }
        public void ShowSkinFilling(float sec)
        {
            _uiHeaderText.gameObject.SetActive(true);
            isSkipTap = false;
            AvatarContainer.gameObject.SetActive(true);
            Fon?.SetActive(true);
            if (UIWindowsManager.GetWindow<DialogWindow>().gameObject.activeInHierarchy)
            {
                UIWindowsManager.GetWindow<DialogWindow>().gameObject.SetActive(false);
            }

            var mn = UIWindowsManager.GetWindow<MainWindow>();
            mn.BonusFon.SetActive(false);
            mn.BonusText.gameObject.SetActive(false);

            GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins); 
            GameService.Instance.PartsTextMesh.text = (CollectingSystem.Instance.Lvl_gainedparts + CollectingSystem.Instance.Lvl_bonusparts).ToString();
            mn.BonusText.text = "0";
            //CollectingSystem.Instance.AddGained(20,5);

            CollectingSystem.Instance.StoreLvlResult();

            RecalcRareSkins(CollectingSystem.Instance.GetRecievedFromCollection().parts+CollectingSystem.Instance.Lvl_gainedparts+ CollectingSystem.Instance.Lvl_bonusparts);
            RecalcAchivableSkins(CollectingSystem.Instance.Lvl_gainedcoins);
            
            DropImgToFirst();
            if (CalcAnimCorout!=null)
            {
                StopCoroutine(CalcAnimCorout);
                if (GoOutCoroutine!=null)
                {
                    StopCoroutine(GoOutCoroutine);
                    _btnHomeImg.fillAmount = 1;
                }
            }
            CalcAnimCorout = Splash(sec);
            StartCoroutine(CalcAnimCorout);            
        }

        private void ContainerUpdate()
        {
            _avatarContainer = transform.Find("AVATAR_CONTAINER").gameObject;            
            _btnBonus = transform.Find("bonus_lvl").gameObject;
            _btnHome = transform.Find("btn_home").gameObject;
            _btnX2 = transform.Find("x2").gameObject;
        }
        private IEnumerator ImgFill(float cur_fill, float max_fill)
        {

            skin_img.material.SetFloat("_xoffset", cur_fill);
            while (cur_fill < max_fill && !isSkipTap)
            {
                cur_fill = Mathf.Lerp(cur_fill, max_fill + 0.1f, Time.fixedDeltaTime*2);
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                skin_img.material.SetFloat("_xoffset", cur_fill);
            }
            skin_img.material.SetFloat("_xoffset", max_fill);
        }

        private IEnumerator Splash(float sec)
        {
            yield return new WaitForFixedUpdate();
            StopCoroutine(CloseAfter(sec));
            int iteration_num;
            int iteration_one;
            float frecuence = SceneLoader.sceneSettings.Frecuence;

            
            RectTransform re_t = UIWindowsManager.GetWindow<MainWindow>().CoinText.gameObject.GetComponent<RectTransform>();
            RectTransform re_t_parts = UIWindowsManager.GetWindow<MainWindow>().PartText.gameObject.GetComponent<RectTransform>();
            RectTransform re_t_bonus_parts = UIWindowsManager.GetWindow<MainWindow>().BonusText.gameObject.GetComponent<RectTransform>();

            Vector3 WorldCoord_Target = coins_text.gameObject.transform.position;

            ImgFillCorout = null;

            var CollectionGainView = AchivableCalcSkins;
            CollectionGainView.AddRange(RareCalcSkins);
            bool firstRare = true;
            foreach (var calc_sk in CollectionGainView)
            {
               
                Vector3 WorldCoord_points = (calc_sk.rareType == (int)SkinType.achievable ? re_t.position : re_t_parts.position);
                int partsSum = CollectingSystem.Instance.Lvl_gainedparts + CollectingSystem.Instance.Lvl_bonusparts;
                int total_gained = (calc_sk.rareType == (int)SkinType.achievable ? (int)CollectingSystem.Instance.Lvl_gainedcoins : partsSum);
                GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
                GameService.Instance.PartsTextMesh.text = partsSum.ToString();

                //АНИМАЦИЯ
                if (calc_sk.rareType != (int)SkinType.rare)
                {
                    GameService.Instance.CoinsTextMesh.gameObject.GetComponent<Animator>().Play("SCORE_GAIN");
                }
                
                SafeSkinData current_origin = DataBaseManager.GetSkinInfo("", calc_sk.PlaySkin_Id).Data;
               
                int origin_collected = current_origin.CollectedParts;
                int origin_maxParts = current_origin.MaxParts;

                int cur_skin_collect = (int)calc_sk.CollectedParts - origin_collected;

                int score_result = Mathf.Clamp((calc_sk.rareType==(int)SkinType.achievable? (int)CollectingSystem.Instance.Lvl_gainedcoins : partsSum) - cur_skin_collect, 0, int.MaxValue);
                int av_coin_result = calc_sk.CollectedParts;

                skin_img.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == calc_sk.PrefabName + "_img");
                skin_outer.sprite = DataBaseManager.Instance.SpriteBase.Find(sp => sp.name == calc_sk.PrefabName + "_outer");

                iteration_num = (int)((sec * 0.5f) / frecuence);
                iteration_one = Mathf.RoundToInt(Mathf.Clamp(((float)cur_skin_collect / (float)iteration_num), 1, 1000));

                float timer = 0;

                coins_text.text = origin_collected.ToString() + "/" + origin_maxParts.ToString();

                float max_fill = (calc_sk.CollectedParts!= calc_sk.MaxParts) ? 
                     Mathf.Clamp(
                     Mathf.Lerp(0.2f, 0.83f, (float)calc_sk.CollectedParts / (float)calc_sk.MaxParts),
                     0f, 1f) 
                     : 1.2f;
                float cur_fill = Mathf.Clamp01(
                     Mathf.Lerp(0.2f, 0.83f, (float)origin_collected / (float)origin_maxParts   ));
                if (ImgFillCorout != null)
                {
                    SceneLoader.Instance.StopCoroutine(ImgFillCorout);
                }
                if (NewSkin.gameObject.activeInHierarchy)
                {
                    NewSkin.gameObject.SetActive(false);
                }
                ImgFillCorout = ImgFill(cur_fill, max_fill);
                skin_img.material.SetFloat("_xoffset", cur_fill);

                if (isSkipTap)
                {
                    yield return new WaitForSeconds(AFTER_FULL_PAUSE);
                }
               
                SceneLoader.Instance.StartCoroutine(ImgFillCorout);
                int av_coin_balance = origin_collected;
                while (timer < sec && !isSkipTap)
                {

                    if (total_gained > score_result)
                    {
                        total_gained = Mathf.Clamp(total_gained - iteration_one, score_result, 1000);
                        av_coin_balance = Mathf.Clamp(av_coin_balance + iteration_one, 0, origin_maxParts);
                        UIWindowsManager.GetWindow<MainWindow>().StartCoroutine(
                                         UIEffects.AnimateDecrScore(
                                               GameObjectPool.Instance.GetPooledObject(PooledObjectType.FlowMessage),
                                               WorldCoord_points, WorldCoord_Target, iteration_one.ToString()
                                         ));
                    }
                    else
                    {
                       
                        timer = sec;
                    }


                    yield return new WaitForSeconds(frecuence);
                    timer += frecuence;

                   

                    if (calc_sk.rareType == (int)SkinType.achievable)
                    {
                        GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(total_gained);
                    }
                    else
                    {
                        GameService.Instance.PartsTextMesh.text = total_gained.ToString();
                    }
                    coins_text.text = av_coin_balance.ToString() + "/" + origin_maxParts.ToString();


                }

                total_gained = score_result;
                av_coin_balance = av_coin_result;
                if (calc_sk.rareType == (int)SkinType.achievable)
                {
                    GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(total_gained);
                }
                else
                {
                    GameService.Instance.PartsTextMesh.text = total_gained.ToString();
                }

                coins_text.text = av_coin_balance.ToString() + "/" + origin_maxParts.ToString();

                if (firstRare && (calc_sk.rareType == (int)SkinType.rare))
                {
                    yield return new WaitForSeconds(0.5f);
                    firstRare = false;
                }
                else
                    yield return new WaitForSeconds(0.5f);
                if (av_coin_balance== origin_maxParts )
                {

                    if (calc_sk.rareType == (int)SkinType.rare)
                    {
                        AudioServer.PlaySound(Sound.ChallengeDone, 0.8f);                      
                    }
                    
                    AnimateNew();
                    float t = Time.fixedUnscaledTime;
                    yield return new WaitForSeconds(2f);
                }

                isSkipTap &= false;
               
            }


            ShowButtonsBySettings();
            var mn = UIWindowsManager.GetWindow<MainWindow>();

            mn.BonusFon.SetActive(GameService.Instance.challengeServer.CurrentDone);
            mn.BonusText.gameObject.SetActive(GameService.Instance.challengeServer.CurrentDone);
            
            
            GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
            GameService.Instance.PartsTextMesh.text = CollectingSystem.Instance.Lvl_gainedparts.ToString();
            mn.BonusText.text = "+" + CollectingSystem.Instance.Lvl_bonusparts;


            if (Window_settings.Contains(Restart_settings.X2_BTN))
            {
                GoOutCoroutine = CloseAfter(sec);
            }
            else
                GoOutCoroutine = CloseAfter(1);

            StartCoroutine(GoOutCoroutine);
            UIWindowsManager.GetWindow<MainWindow>().Splash_Timer.gameObject.SetActive(false);            
        }
        private void AnimateNew()
        {
            _viewAnimator.Play("Bounce");
            AudioServer.PlaySound(Sound.ChallengeDone, 0.8f);
            NewSkin.gameObject.SetActive(true,2f,this);
            NewSkin.Play();
        }

        private IEnumerator CloseAfter(float sec)
        {
            float timer = sec;
            float amount = 1f;            
            while (timer > 0)
            {
                _btnHomeImg.fillAmount = amount;
                yield return new WaitForFixedUpdate();
                amount -= (1 / sec) * Time.fixedDeltaTime;
                timer -= Time.fixedDeltaTime;
            }

            BtnHomeClick();            

        }


        public void BtnBonusClick()
        {
            AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
            // выключаем флаг 
            if (Window_settings.Contains(Restart_settings.BONUS_BTN))
            {
                Window_settings.Remove(Restart_settings.BONUS_BTN);
            }
            if (Window_settings.Contains(Restart_settings.X2_BTN))
            {
                Window_settings.Remove(Restart_settings.X2_BTN);

            }


            _btnX2.SetActive(false);
            _btnBonus.SetActive(false);

            GoOutCoroutine = CloseAfter(1);
        }

        public void Btnx2Click()
        {
            AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
            if (GoOutCoroutine != null)
            {
                StopCoroutine(GoOutCoroutine);
                _btnHomeImg.fillAmount = 1f;
            }

            // выключаем флаг 
            if (Window_settings.Contains(Restart_settings.X2_BTN))
            {
                Window_settings.Remove(Restart_settings.X2_BTN);

            }
            if (Window_settings.Contains(Restart_settings.BONUS_BTN))
            {
                Window_settings.Remove(Restart_settings.BONUS_BTN);
            }

            _btnX2.SetActive(false);
            _btnBonus.SetActive(false);
            GoOutCoroutine = CloseAfter(1);
        }

        public void BtnHomeClick()
        {
            NewSkin.gameObject.SetActive(false);
            AudioServer.PlaySound(Sound.ButtonClick, 0.5f);
            CollectingSystem.Instance.SaveCurrentGainToCollection();
            GameService.Instance.challengeServer.MoveNext();
            UIWindowsManager.GetWindow<MainWindow>().OpenStartWindow();
        }
       


        private void X2RewardCallback()
        {

            CollectingSystem.Instance.AddGained(CollectingSystem.Instance.Lvl_gainedcoins,
                                                CollectingSystem.Instance.Lvl_gainedparts);
            bool isRewarded = GameService.Instance.challengeServer.CurrentDone;
            if (GameService.Instance.challengeServer.GetReward() > 0 && !isRewarded)
            {
                UIWindowsManager.ActiveNow = _dialog_window;
                _dialog_window.Open(ChangeCurrentWindow);
                AudioServer.PlaySound(Sound.ChallengeDone);
            }
            else
            {
                ShowSkinFilling(5f);
            }

        }
        private void BonusRewardCallback()
        {
            GameService.Instance.ActiveLvlType = LvlType.bonus;

            //Сохраняем с флагом что ещё не закончили
            CollectingSystem.Instance.StoreLvlResult();

            CollectingSystem.Instance.GetPlayerAvatar(out SafeSkinData selectSkin);

            GameService.Instance.PutAvatarToGame(selectSkin);
            CloseCallback = null;
            Close();


            MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
            mn.MainMessage.gameObject.SetActive(true);
            GameService.Instance.CoinsTextMesh.text = GameService.Instance.challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
            UIWindowsManager.ActiveNow = mn;
        }

        private void RecalcRareSkins(int AllParts)
        {            
            RareCalcSkins = RareCalcSkins ?? new List<SafeSkinData>();
            RareCalcSkins.Clear();
            var IncrSkins = DataBaseManager.Instance.GetPlayerSkinsCollection().
                           Where(sk => (int)sk.Purchased == 0 &&
                           (int)sk.rareType == (int)SkinType.rare && sk.MaxParts<= AllParts && (int)sk.Enabled ==0).
                           OrderBy(sk => (int)sk.PlaySkin_Id).ToList();
            if (IncrSkins.Count > 0)
            {
                foreach (var sk in IncrSkins)
                {
                    SafeSkinData full_sk = new SafeSkinData(sk.PrefabName,sk.PlaySkin_Id, sk.MaxParts, sk.MaxParts, sk.matColor,sk.Purchased,sk.rareType);
                    RareCalcSkins.Add(full_sk);
                    if (!CollectingSystem.Instance.NewOpenSkinsId.Contains(full_sk.PlaySkin_Id))
                    {
                            CollectingSystem.Instance.NewOpenSkinsId.Add(full_sk.PlaySkin_Id);
                    }
                }
                
            }
            else return;
        }

        
        private void RecalcAchivableSkins(int new_balance)
        {
                        

            AchivableCalcSkins = AchivableCalcSkins ?? new List<SafeSkinData>();
            AchivableCalcSkins.Clear();

            int LastNotFilled = DataBaseManager.Instance.GetLastNotFilled();
            if (LastNotFilled<0)
            {
                return;
            }

            var SkinsIncrem = DataBaseManager.Instance.GetPlayerSkinsCollection().
                             Where(sk => (int)sk.PlaySkin_Id >= LastNotFilled && (int)sk.Purchased == 0 &&
                             (int)sk.rareType == (int)SkinType.achievable).
                             OrderBy(sk => (int)sk.PlaySkin_Id);

            int balance = new_balance;
            List<SafeSkinData> tmp_list = new List<SafeSkinData>();

            foreach (var skin in SkinsIncrem)
            {

                
                SafeSkinData curSkin = skin;
                

                curSkin.CollectedParts = Mathf.Clamp(skin.CollectedParts + balance, 0, (int)skin.MaxParts);

                if (curSkin.CollectedParts!= skin.CollectedParts && curSkin.CollectedParts== skin.MaxParts)
                {
                    if (!CollectingSystem.Instance.NewOpenSkinsId.Contains(curSkin.PlaySkin_Id))
                    {
                        CollectingSystem.Instance.NewOpenSkinsId.Add(curSkin.PlaySkin_Id);
                    }
                    
                }

                curSkin.Enabled = DataBaseManager.SkinEnabled(curSkin);
                tmp_list.Add(curSkin);
            }
            if (tmp_list.Count>0)
            {
                var RecalcSkins = tmp_list.Where(sk => (int)sk.CollectedParts == (int)sk.MaxParts)
                    .OrderBy(sk => (int)sk.PlaySkin_Id)
                    .ToList();
                var NotFilledSkin = tmp_list.Where(sk => (int)sk.CollectedParts > 0 && (int)sk.CollectedParts != (int)sk.MaxParts)
                    .OrderBy(sk => (int)sk.PlaySkin_Id).FirstOrDefault();
                if (NotFilledSkin.PlaySkin_Id!=0)
                {
                    RecalcSkins.Add(NotFilledSkin);
                }
                AchivableCalcSkins = RecalcSkins.OrderBy(sk => (int)sk.PlaySkin_Id).ToList();
            }

        

        }
        private string GenerateHeader()
        {
            
            if (_uiHeaderText == null)
            {
                _uiHeaderText = transform.Find("HEADER_CANVAS").Find("UI_HEADER_TEXT").GetComponent<TextMeshProUGUI>();
            }

            if (!Window_settings.Contains(Restart_settings.HEADER_TEXT))
            {
                return _uiHeaderText.text;
            }

            int lvlpointGained = CollectingSystem.Instance.Lvl_gainedcoins;
            string res_str = "";
            if (SceneLoader.sceneSettings.EndTextVariants.Count >= 3)
            {

                if (lvlpointGained <= 10)
                {
                    res_str = SceneLoader.sceneSettings.EndTextVariants[0];
                }
                else if (lvlpointGained > 10 && lvlpointGained < 30)
                {
                    res_str = SceneLoader.sceneSettings.EndTextVariants[1];
                }
                else
                    res_str = SceneLoader.sceneSettings.EndTextVariants[2];
            }

            string[] strLines = res_str.Split(' ');
            _uiHeaderText.text = strLines[0] + " \n ";
            for (int i = 1; i < strLines.Length; i++)
            {
                _uiHeaderText.text += strLines[i] + " ";
            }
            return res_str;
        }
        public void ShowAllButtons(bool value)
        {
            //btnExit.SetActive(value);
            _btnBonus.SetActive(value);
            _btnHome.SetActive(value);
            _btnX2.SetActive(value);
        }

        private void ShowButtonsBySettings()
        {
            _btnHome.SetActive(Window_settings.Contains(Restart_settings.HOME_BTN));
            _btnX2.SetActive(Window_settings.Contains(Restart_settings.X2_BTN));
            _btnBonus.SetActive(Window_settings.Contains(Restart_settings.BONUS_BTN));      

        }
        
        private void WhenTapOrSvipe()
        {
            isSkipTap = true;
          
        }

    }

    [Flags]
    public enum Restart_settings : uint
    {
        None = 0,
        //Buttons
        BONUS_BTN = 1,
        X2_BTN = 2,
        CLOSE_BTN = 4,
        HOME_BTN = 8,
        //CONTAINERS
        AVATAR_CONTAINER = 16,
        //texts
        HEADER_TEXT = 32,
        //Profiles
        isSPLASH = 64,
        CAN_SELECT = 128,
        ALL = ~None
    }
}