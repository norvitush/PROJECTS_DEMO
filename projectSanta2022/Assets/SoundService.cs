using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VOrb.CubesWar
{
    public enum Sound
    {
     ButtonClick = 0,
     SantaThrow = 1,
     ChimneySuccsess = 2,
     ChimneyFailed = 3,
     StoneBang = 4,
     StarShow = 5,
     StarsWindowShow = 6,
     Congratulation = 7,
     Music = 9,
     PauseBackgroundMusic = 10,
     Coin = 11

}

    public class SoundService : Singlton<SoundService>
    {
        [SerializeField]
        private float _volumeIncreaseSpeed;
        public AudioSource ButtonClick;
        public AudioSource SantaThrow;
        public AudioSource ChimneySuccsess;
        public AudioSource ChimneyFailed;
        public AudioSource StoneBang;
        public AudioSource StarShow;
        public AudioSource StarsWindowShow;
        public AudioSource Congratulation;
        public AudioSource Coin;
        public AudioSource Music;
        public AudioSource PauseBackgroundMusic;

        public static void AttestMusic()
        {
            SetMusic(GameService.Instance.Sounds && GameService.Instance.GameStarted);
        }

        public static void SetMusic(bool value) 
        {
            if (value)
            {
                Instance.Music.Play();
                Instance.PauseBackgroundMusic.Stop();
                Instance.Music.volume = 0;
                Instance.StartCoroutine(Instance.SoundVolumeIncreasing(Instance.Music,0.3f));
            }
            else 
            {
                
                Instance.StopAllCoroutines();
                Instance.Music.Stop();
            }
        }

        public static void PauseMusic(bool value)
        {
            if (GameService.Instance.Sounds)
            {
                if (value)
                    Instance.Music.Pause();
                else
                    Instance.Music.UnPause();
            }
            else
                Instance.Music.Stop();
        }

        public static void PlayPauseBackground(bool value, bool withIncrease = true)
        {
            if (GameService.Instance.Sounds && value)
            {
                if (value)
                {
                    Instance.PauseBackgroundMusic.Play();
                    if (withIncrease)
                    {
                        Instance.PauseBackgroundMusic.volume = 0;
                        Instance.StartCoroutine(Instance.SoundVolumeIncreasing(Instance.PauseBackgroundMusic, 0.7f));
                    }
                    else
                        Instance.PauseBackgroundMusic.volume = 0.7f;
                                    }
            }
            else
            {
                if (withIncrease)
                {
                    Instance.StopAllCoroutines();
                }
                Instance.PauseBackgroundMusic.Stop();

            }

        }

        private IEnumerator SoundVolumeIncreasing(AudioSource source,float maxValume = 1f)
        {
            float volume = 0;
            while (volume< maxValume)
            {
                volume += Instance._volumeIncreaseSpeed * Time.deltaTime;
                yield return null;
                source.volume = volume;
            }
            
        }

        public static void PlaySound(Sound snd, float volumeScale = 1)
        {
            if (GameService.Instance.Sounds)
            {
                switch (snd)
                {
                    case Sound.ButtonClick:
                        Instance.ButtonClick.PlayOneShot(Instance.ButtonClick.clip, 0.8f);
                        break;
                    case Sound.SantaThrow:
                        Instance.SantaThrow.PlayOneShot(Instance.SantaThrow.clip, 1.2f);
                        break;
                    case Sound.ChimneySuccsess:
                        Instance.ChimneySuccsess.PlayOneShot(Instance.ChimneySuccsess.clip, 1f);
                        break;
                    case Sound.ChimneyFailed:
                        Instance.ChimneyFailed.PlayOneShot(Instance.ChimneyFailed.clip, 1.3f);
                        break;
                    case Sound.StoneBang:
                        Instance.StoneBang.PlayOneShot(Instance.StoneBang.clip, 1.2f);
                        break;
                    case Sound.StarShow:
                        Instance.StarShow.PlayOneShot(Instance.StarShow.clip, 1f);
                        break;
                    case Sound.StarsWindowShow:
                        Instance.StarsWindowShow.PlayOneShot(Instance.StarsWindowShow.clip, 1f);
                        break;
                    case Sound.Congratulation:
                        Instance.Congratulation.PlayOneShot(Instance.Congratulation.clip, 1f);
                        break;
                    case Sound.Coin:
                        Instance.Coin.PlayOneShot(Instance.Coin.clip, 1f);
                        break;
                    default:
                        break;
                }

               
                

            }

        }

    }
}

