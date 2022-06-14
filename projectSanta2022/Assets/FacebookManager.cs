using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;


namespace VOrb
{

    public class FacebookManager : MonoBehaviour
    {
        // Awake function from Unity's MonoBehavior
        private void Awake()
        {

            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
        public void LevelEnded(int level)
        {
            var lvlParams = new Dictionary<string, object>();
            lvlParams["Level complete"] = level.ToString();

            FB.LogAppEvent(
               AppEventName.AchievedLevel,
               parameters: lvlParams
            );
        }

    }

}
