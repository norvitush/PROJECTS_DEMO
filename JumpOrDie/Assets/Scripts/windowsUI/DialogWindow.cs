using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VOrb;
using UnityEngine.UI;


public class DialogWindow : Window
{
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private GameObject _spawn_point;
    private GameObject _textObject = null;

    protected override void SelfClose()
    {
       
        if (gameObject.activeInHierarchy)
        {
            UIWindowsManager.ActiveNow = UIWindowsManager.GetWindow<RestartWindow>();
            UIWindowsManager.GetWindow<RestartWindow>().ShowSkinFilling(5f);
        }
        
            _textObject?.SetActive(false);
        
        gameObject.SetActive(false);

    }

    protected override void SelfOpen()
    {
        UIWindowsManager.GetWindow<RestartWindow>().AvatarContainer.gameObject.SetActive(false);
        UIWindowsManager.GetWindow<RestartWindow>().Fon?.SetActive(false);
        UIWindowsManager.GetWindow<RestartWindow>().ShowAllButtons(false);
        //настройка ивентов
        TapEvent.RemoveAllListeners();
        SvipeEvent.RemoveAllListeners();
        TapEvent.AddListener(() => {  });
        SvipeEvent.AddListener(() => {  });

        Challenge current = GameService.Instance.challengeServer.GetCurrent();
        var mn = UIWindowsManager.GetWindow<MainWindow>();

        //обмновим ка поля
        mn.BonusText.text = "+" + CollectingSystem.Instance.Lvl_bonusparts.ToString();
        mn.CoinText.text = GameService.Instance.challengeServer.GetChallengeString(CollectingSystem.Instance.Lvl_gainedcoins);
        mn.PartText.text = CollectingSystem.Instance.Lvl_gainedparts.ToString();


        int reward = GameService.Instance.challengeServer.GetReward();

            _coinText.text = (current.CoinsTarget > 0 ? current.CoinsTarget: current.PartsTarget) + "+";
            _rewardText.text = current.Reward.ToString();
            CollectingSystem.Instance.AddGained(0, 0,reward);
          
        

        gameObject.SetActive(true);
            StartCoroutine(nameof(WaitForAnimation));
        

        

    }

    private IEnumerator WaitForAnimation()
    {
        MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
        TextMeshProUGUI target = mn.BonusText;

        mn.BonusFon.SetActive(true);
        target.gameObject.SetActive(true); // mn.BonusText

        yield return new WaitForSeconds(1.5f);
        
        
        Vector3 WorldCoord_Target = target.transform.position;
        _textObject = GameObjectPool.Instance.GetPooledObject(PooledObjectType.FlowMessage);
        

        StartCoroutine(
             UIEffects.AnimateDecrScore(_textObject, _spawn_point.transform.position, WorldCoord_Target, ("+" + CollectingSystem.Instance.Lvl_bonusparts),false,2)
        );

        TapEvent.AddListener(() => { Close(); });
        SvipeEvent.AddListener(() => { Close(); });

        float beginTime = Time.unscaledTime;

        yield return new WaitUntil(()=>_textObject.transform.position.isNearToDestiny(WorldCoord_Target, 2f)||(Time.unscaledTime-beginTime > 4));

        //mn.Splash_Timer.gameObject.SetActive(true);
        //mn.Splash_Timer.Play();
        
        target.text = "+" + CollectingSystem.Instance.Lvl_bonusparts.ToString();
        target.GetComponent<Animator>().Play("gain_animation");
    }


}
