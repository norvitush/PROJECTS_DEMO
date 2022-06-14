using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace VOrb
{

    public struct AdShowInfo
    {
        public bool NeedToReward;
        public bool WasShowed;
        public bool ReadyForShow;
    }




    public class IronSourceManager : MonoBehaviour
    {
        public AdShowInfo Rewarded_result;
        public AdShowInfo Interstitial_result;

        public void Start()
        {
            Rewarded_result = new AdShowInfo { NeedToReward = false, WasShowed = false, ReadyForShow = false };
            Interstitial_result = new AdShowInfo { NeedToReward = false, WasShowed = false, ReadyForShow = false };

#if UNITY_ANDROID && !UNITY_EDITOR
                    string appKey = "125137df5";
#elif UNITY_IPHONE
                    string appKey = "125137df5";
#else
            string appKey = "unexpected_platform";
#endif

            IronSource.Agent.validateIntegration();

            IronSource.Agent.shouldTrackNetworkState(true);



            // SDK init            
            IronSource.Agent.init(appKey);
            Debuger.AddToLayout("|*IS* InitKey: " + appKey);

            


        }

        void OnEnable()
        {

            ////Add Rewarded Video Events
            //IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            //IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            //IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            //IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;


            //// Add Interstitial Events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

            #region commented_for_future
            //IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            //IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            //IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            //IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;


            ////Add Rewarded Video DemandOnly Events
            //IronSourceEvents.onRewardedVideoAdOpenedDemandOnlyEvent += RewardedVideoAdOpenedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdClosedDemandOnlyEvent += RewardedVideoAdClosedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdLoadedDemandOnlyEvent += RewardedVideoAdLoadedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdRewardedDemandOnlyEvent += RewardedVideoAdRewardedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdShowFailedDemandOnlyEvent += RewardedVideoAdShowFailedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdClickedDemandOnlyEvent += RewardedVideoAdClickedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdLoadFailedDemandOnlyEvent += RewardedVideoAdLoadFailedDemandOnlyEvent;


            //// Add Offerwall Events
            //IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
            //IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
            //IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
            //IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
            //IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
            //IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;


            //// Add Interstitial Events
            //IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            //IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            //IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            //IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            //IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            //IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            //IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;



            //// Add Interstitial DemandOnly Events
            //IronSourceEvents.onInterstitialAdReadyDemandOnlyEvent += InterstitialAdReadyDemandOnlyEvent;
            //IronSourceEvents.onInterstitialAdLoadFailedDemandOnlyEvent += InterstitialAdLoadFailedDemandOnlyEvent;
            //IronSourceEvents.onInterstitialAdShowFailedDemandOnlyEvent += InterstitialAdShowFailedDemandOnlyEvent;
            //IronSourceEvents.onInterstitialAdClickedDemandOnlyEvent += InterstitialAdClickedDemandOnlyEvent;
            //IronSourceEvents.onInterstitialAdOpenedDemandOnlyEvent += InterstitialAdOpenedDemandOnlyEvent;
            //IronSourceEvents.onInterstitialAdClosedDemandOnlyEvent += InterstitialAdClosedDemandOnlyEvent;


            //// Add Banner Events
            //IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            //IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            //IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            //IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            //IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            //IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
            #endregion
            //Add ImpressionSuccess Event
            IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;


            

        }

        void OnApplicationPause(bool isPaused)
        {
            //Debuger.AddToLayout("unity-script: OnApplicationPause = " + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

        #region sample_code
        //public void OnGUI()
        //{

        //    //GUI.backgroundColor = Color.blue;
        //    //GUI.skin.button.fontSize = (int)(0.035f * Screen.width);





        //    //Rect showRewardedVideoButton = new Rect(0.10f * Screen.width, 0.15f * Screen.height, 0.80f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(showRewardedVideoButton, "Show Rewarded Video"))
        //    //{
        //    //    Debug.Log("unity-script: ShowRewardedVideoButtonClicked");
        //    //    if (IronSource.Agent.isRewardedVideoAvailable())
        //    //    {
        //    //        IronSource.Agent.showRewardedVideo();
        //    //    }
        //    //    else
        //    //    {
        //    //        Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        //    //    }
        //    //}



        //    //Rect showOfferwallButton = new Rect(0.10f * Screen.width, 0.25f * Screen.height, 0.80f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(showOfferwallButton, "Show Offerwall"))
        //    //{
        //    //    if (IronSource.Agent.isOfferwallAvailable())
        //    //    {
        //    //        IronSource.Agent.showOfferwall();
        //    //    }
        //    //    else
        //    //    {
        //    //        Debug.Log("IronSource.Agent.isOfferwallAvailable - False");
        //    //    }
        //    //}

        //    //Rect loadInterstitialButton = new Rect(0.10f * Screen.width, 0.35f * Screen.height, 0.35f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(loadInterstitialButton, "Load Interstitial"))
        //    //{
        //    //    Debug.Log("unity-script: LoadInterstitialButtonClicked");
        //    //    IronSource.Agent.loadInterstitial();
        //    //}

        //    //Rect showInterstitialButton = new Rect(0.55f * Screen.width, 0.35f * Screen.height, 0.35f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(showInterstitialButton, "Show Interstitial"))
        //    //{
        //    //    Debug.Log("unity-script: ShowInterstitialButtonClicked");
        //    //    if (IronSource.Agent.isInterstitialReady())
        //    //    {
        //    //        IronSource.Agent.showInterstitial();
        //    //    }
        //    //    else
        //    //    {
        //    //        Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
        //    //    }
        //    //}

        //    //Rect loadBannerButton = new Rect(0.10f * Screen.width, 0.45f * Screen.height, 0.35f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(loadBannerButton, "Load Banner"))
        //    //{
        //    //    Debug.Log("unity-script: loadBannerButtonClicked");
        //    //    IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
        //    //}

        //    //Rect destroyBannerButton = new Rect(0.55f * Screen.width, 0.45f * Screen.height, 0.35f * Screen.width, 0.08f * Screen.height);
        //    //if (GUI.Button(destroyBannerButton, "Destroy Banner"))
        //    //{
        //    //    Debug.Log("unity-script: loadBannerButtonClicked");
        //    //    IronSource.Agent.destroyBanner();
        //    //}




        //}
        #endregion

        //#region RewardedAd callback handlers

        //void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
        //{
        //    Debug.Log(" I got AvailabilityChangedEvent, value = " + canShowAd);
        //    Rewarded_result.ReadyForShow = canShowAd;
        //}

        //void RewardedVideoAdOpenedEvent()
        //{
        //    Debug.Log("I got VideoAdOpenedEvent");
        //    Rewarded_result.NeedToReward = false;
        //    Rewarded_result.WasShowed = false;
        //}

        //void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
        //{
        //    Debug.Log("I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        //    Rewarded_result.NeedToReward = true;
        //}

        //void RewardedVideoAdClosedEvent()
        //{
        //    Debug.Log("I got RewardedVideoAdClosedEvent");
        //    Rewarded_result.WasShowed = true;
        //}

        //void RewardedVideoAdStartedEvent()
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
        //}

        //void RewardedVideoAdEndedEvent()
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
        //}

        //void RewardedVideoAdShowFailedEvent(IronSourceError error)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
        //}

        /************* RewardedVideo DemandOnly Delegates *************/

        //void RewardedVideoAdLoadedDemandOnlyEvent(string instanceId)
        //{

        //    Debug.Log("unity-script: I got RewardedVideoAdLoadedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void RewardedVideoAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        //{

        //    Debug.Log("unity-script: I got RewardedVideoAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void RewardedVideoAdOpenedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdOpenedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void RewardedVideoAdClosedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdClosedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void RewardedVideoAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void RewardedVideoAdClickedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got RewardedVideoAdClickedDemandOnlyEvent for instance: " + instanceId);
        //}


       // #endregion



        //#region Interstitial callback handlers

        void InterstitialAdReadyEvent()
        {
            Debuger.AddToLayout("*IS: InterstitialAdReadyEvent");
            Interstitial_result.ReadyForShow = true;
        }

        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            Debuger.AddToLayout("*IS: InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
            Interstitial_result.NeedToReward = true;
            Interstitial_result.WasShowed = true;
        }

        void InterstitialAdShowSucceededEvent()
        {
            Debuger.AddToLayout("*IS: InterstitialAdShowSucceededEvent");
            Interstitial_result.NeedToReward = true;
        }

        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            Debuger.AddToLayout("*IS: InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
            Interstitial_result.NeedToReward = true;
        }

        void InterstitialAdClickedEvent()
        {
            Debuger.AddToLayout("*IS: InterstitialAdClickedEvent");

            Interstitial_result.NeedToReward = true;
        }

        void InterstitialAdOpenedEvent()
        {
            Debuger.AddToLayout("*IS:  InterstitialAdOpenedEvent");

            Interstitial_result.NeedToReward = false;
            Interstitial_result.WasShowed = false;
        }

        void InterstitialAdClosedEvent()
        {
            //Debug.Log("*IS: InterstitialAdClosedEvent -  WasShowed = true");
            Debuger.AddToLayout("*IS: InterstitialAdClosedEvent -  WasShowed = true");
            Interstitial_result.WasShowed = true;
            IronSource.Agent.loadInterstitial();
        }

        ///************* Interstitial DemandOnly Delegates *************/

        //void InterstitialAdReadyDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdReadyDemandOnlyEvent for instance: " + instanceId);
        //}

        //void InterstitialAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", error code: " + error.getCode() + ",error description : " + error.getDescription());
        //}

        //void InterstitialAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", error code :  " + error.getCode() + ",error description : " + error.getDescription());
        //}

        //void InterstitialAdClickedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdClickedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void InterstitialAdOpenedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdOpenedDemandOnlyEvent for instance: " + instanceId);
        //}

        //void InterstitialAdClosedDemandOnlyEvent(string instanceId)
        //{
        //    Debug.Log("unity-script: I got InterstitialAdClosedDemandOnlyEvent for instance: " + instanceId);
        //}




        //#endregion

        //#region Banner callback handlers

        //void BannerAdLoadedEvent()
        //{
        //    Debug.Log("unity-script: I got BannerAdLoadedEvent");
        //}

        //void BannerAdLoadFailedEvent(IronSourceError error)
        //{
        //    Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void BannerAdClickedEvent()
        //{
        //    Debug.Log("unity-script: I got BannerAdClickedEvent");
        //}

        //void BannerAdScreenPresentedEvent()
        //{
        //    Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
        //}

        //void BannerAdScreenDismissedEvent()
        //{
        //    Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
        //}

        //void BannerAdLeftApplicationEvent()
        //{
        //    Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
        //}

        //#endregion


        //#region Offerwall callback handlers

        //void OfferwallOpenedEvent()
        //{
        //    Debug.Log("I got OfferwallOpenedEvent");
        //}

        //void OfferwallClosedEvent()
        //{
        //    Debug.Log("I got OfferwallClosedEvent");
        //}

        //void OfferwallShowFailedEvent(IronSourceError error)
        //{
        //    Debug.Log("I got OfferwallShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void OfferwallAdCreditedEvent(Dictionary<string, object> dict)
        //{
        //    Debug.Log("I got OfferwallAdCreditedEvent, current credits = " + dict["credits"] + " totalCredits = " + dict["totalCredits"]);

        //}

        //void GetOfferwallCreditsFailedEvent(IronSourceError error)
        //{
        //    Debug.Log("I got GetOfferwallCreditsFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
        //}

        //void OfferwallAvailableEvent(bool canShowOfferwal)
        //{
        //    Debug.Log("I got OfferwallAvailableEvent, value = " + canShowOfferwal);

        //}

        //#endregion

        #region ImpressionSuccess callback handler

        void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                //Firebase.Analytics.Parameter[] AdParameters = {

                // new Firebase.Analytics.Parameter("ad_platform", "ironSource"),

                //  new Firebase.Analytics.Parameter("ad_source", impressionData.adNetwork),

                //  new Firebase.Analytics.Parameter("ad_unit_name", impressionData.adUnit),

                //  new Firebase.Analytics.Parameter("ad_format", impressionData.instanceName),

                //  new Firebase.Analytics.Parameter("currency", "USD"),

                //  new Firebase.Analytics.Parameter("value", (double)impressionData.revenue)

                //};

                //Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", AdParameters);

            }
        }

        #endregion





    }
}

