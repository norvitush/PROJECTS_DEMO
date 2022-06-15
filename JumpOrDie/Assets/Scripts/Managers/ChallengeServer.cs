using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VOrb
{

    public class ChallengeServer
    {        
        private Image _outputImg;
        private Challenge _currentTarget =  null;
        private bool _currentDone = false;

        public bool CurrentDone { get => _currentDone; set => _currentDone = value; }

        public ChallengeServer(Image progressImage)
        {
                _outputImg = progressImage;
                _currentTarget = DataBaseManager.Instance.FindChallenge(
                    (int)DataKeeper.LoadParam(GameService.Instance.PlayerName+"_challenge",1));
        }

        public string GetChallengeString(int val)
        {
            if (_currentTarget.CoinsTarget>0)
            {
                if (_outputImg != null)
                {

                    _outputImg.fillAmount = Mathf.Clamp01((float)val / (_currentTarget.CoinsTarget > 0 ? (float)_currentTarget.CoinsTarget : (float)_currentTarget.PartsTarget));
                }
                else Debug.Log("NULL PROGRESS COIN");
                return val + "/" + (_currentTarget.CoinsTarget > 0 ? _currentTarget.CoinsTarget : _currentTarget.PartsTarget);
            }
            else
            {
                if (_outputImg != null)
                {

                    _outputImg.fillAmount = 1f;
                }
                return val.ToString();
            }
            
        }

        public int GetReward()
        {
            if (_currentTarget.CoinsTarget>0)
            {
                if (CollectingSystem.Instance.Lvl_gainedcoins >= _currentTarget.CoinsTarget
                   && CollectingSystem.Instance.Lvl_gainedparts >= _currentTarget.PartsTarget)
                {
                    CurrentDone = true;
                    return _currentTarget.Reward;
                }
                else
                {
                    CurrentDone = false;
                    return 0;
                }
            }
            else
                return 0;
               

        }
        public void MoveNext(bool save = true)
        {
            if (CurrentDone)
            {
                if (_currentTarget == null)
                {
                    return;
                }
                Challenge next = DataBaseManager.Instance.FindChallenge(_currentTarget.id + 1);

                _currentTarget = next;
                CurrentDone = false;

                if (save)
                {
                    DataKeeper.SaveParam(GameService.Instance.PlayerName + "_challenge", next.id);
                }
                
            }

        }
        public Challenge GetCurrent()
        {
            return _currentTarget;
        }
 
    }



}
