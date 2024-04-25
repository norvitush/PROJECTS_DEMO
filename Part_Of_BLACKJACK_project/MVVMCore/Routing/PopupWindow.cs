using GoldenSoft.UI.MVVM;
using System.Collections;
using UnityEngine;

namespace GoldenSoft
{
    public class PopupWindow : Window
    {
        private Coroutine _timerHandle;
        private float _openTimer;

        protected override void SelfClose()
        {
            TryBreakCoroutine();
            _openTimer = 0;

            try
            {
                if (gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(false);

                    Destroy(gameObject);
                }
            }
            catch (System.Exception)
            {
                //maybe gameObject is already destroyed                
            }
           
        }

        protected override void SelfOpen()  { 
        gameObject.SetActive(true);
        }

        public void Popup(float time)
        {
            TryBreakCoroutine();
            _openTimer = time;
            Open(null);
            _timerHandle = StartCoroutine(WaitingForClose(_openTimer));
        }
        public virtual void Popup()
        {
            TryBreakCoroutine();
            Open(null);
        }
        private IEnumerator WaitingForClose(float time)
        {
            while (_openTimer > 0)
            {
                yield return null;
                _openTimer -= Time.deltaTime;
            }
            _timerHandle = null;
            
            Close();
        }
        private void TryBreakCoroutine() 
        {
            if (_timerHandle != null)
            {
                StopCoroutine(_timerHandle);
                _timerHandle = null;
            }
        }

        public override void Init(IViewModel viewModel)
        {
            throw new System.NotImplementedException();
        }

        public override void SetContentVisible(bool val)
        {
            throw new System.NotImplementedException();
        }
    }

}

