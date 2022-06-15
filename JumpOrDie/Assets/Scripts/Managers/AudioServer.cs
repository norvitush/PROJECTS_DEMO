using UnityEngine;

namespace VOrb
{
    public enum Sound
    {
        Blood = 0 ,
        ButtonClick = 1,
        Tap = 2,
        Svipe = 3,
        CoinCollect = 4,
        ShieldCollect = 5,
        PartCollect = 6,
        FallEnded = 7,
        ChallengeDone = 8,
        BankCounting =9,
        Purchase =10,
        PullUp = 11,
        Preview = 12,
        Die = 13
    }


    public class AudioServer : Singlton<AudioServer>
    {
        [SerializeField] private AudioSource _buttonClick;
        [SerializeField] private AudioSource _coinCollected;
        [SerializeField] private AudioSource _shieldCollected;
        [SerializeField] private AudioSource _partCollected;
        [SerializeField] private AudioSource _onTapJump;
        [SerializeField] private AudioSource _onLanding;
        [SerializeField] private AudioSource _onSvipeDown;
        [SerializeField] private AudioSource _onBankCounting;
        [SerializeField] private AudioSource _challengeDone;
        [SerializeField] private AudioSource _onBlood;
        [SerializeField] private AudioSource _onPullUp;
        [SerializeField] private AudioSource _onPreview;
        [SerializeField] private AudioSource _onDie;

        public static void PlaySound(Sound snd, float volumeScale = 1 )
        {
           
            if (GameService.Instance.Sounds)
            {
                switch (snd)
                {
                    case Sound.Blood:
                        Instance._buttonClick.PlayOneShot(Instance._onBlood.clip, volumeScale);                        
                        break;
                    case Sound.ButtonClick:
                        Instance._buttonClick.PlayOneShot(Instance._buttonClick.clip, 0.5f);
                        break;
                    case Sound.Tap:
                        //рандомизация высоты звука
                        Instance._onTapJump.outputAudioMixerGroup.audioMixer.SetFloat("TapPitch",Random.Range(0.8f,1.3f));
                        Instance._onTapJump.PlayOneShot(Instance._onTapJump.clip, volumeScale);
                        break;
                    case Sound.Svipe:
                        Instance._buttonClick.PlayOneShot(Instance._onSvipeDown.clip, volumeScale);
                        break;
                    case Sound.CoinCollect:
                        Instance._buttonClick.PlayOneShot(Instance._coinCollected.clip, volumeScale);
                        break;
                    case Sound.ShieldCollect:
                        Instance._buttonClick.PlayOneShot(Instance._shieldCollected.clip, volumeScale);
                        break;
                    case Sound.PartCollect:
                        Instance._buttonClick.PlayOneShot(Instance._partCollected.clip, volumeScale);
                        break;
                    case Sound.FallEnded:
                        Instance._buttonClick.PlayOneShot(Instance._onLanding.clip, volumeScale);
                        break;
                    case Sound.ChallengeDone:
                        Instance._buttonClick.PlayOneShot(Instance._challengeDone.clip, volumeScale);
                        break;
                    case Sound.BankCounting:
                        Instance._buttonClick.PlayOneShot(Instance._onBankCounting.clip, volumeScale);
                        break;
                    case Sound.Purchase:
                        Instance._buttonClick.PlayOneShot(Instance._coinCollected.clip, volumeScale);
                        break;
                    case Sound.PullUp:
                        Instance._buttonClick.PlayOneShot(Instance._onDie.clip, 0.2f);                        
                        break;
                    case Sound.Preview:
                        Instance._buttonClick.PlayOneShot(Instance._onPreview.clip, volumeScale);
                        break;
                    case Sound.Die:
                        Instance._buttonClick.PlayOneShot(Instance._onDie.clip, volumeScale);
                        break;
                    default:
                        break;
                }

            }
           
        }
    }

}

