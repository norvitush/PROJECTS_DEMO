using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VOrb.CubesWar
{
    public class GiftsDriver
    {
        private Coroutine _waitForLevelEnd = null;
        private bool _nextGiftReady = true;
        private PooledObject _newGift;
        private int _throwCount = 0;
        private SafeGiftItem NextCubeContent { get => DataBaseManager.GetCubeItemData("", 1); }

        public List<Vector3> SpawnPoints;
        public GameObject NewCube { get => _nextGiftReady ? _newGift.GameObject : null; }
        public float FALL_LIMIT = 12f;
        public int ThrowCount => _throwCount;

        public GiftsDriver()
        {
            EventPublisher.onShoot.Subscribe(OnThrow);
            SpawnPoints = new List<Vector3>();
        }

        public List<GiftBehaviour> GetAllNotThrowed()
        {
            List<GiftBehaviour> deactivatedCubes = new List<GiftBehaviour>();
            deactivatedCubes = ObjectPoolManager.GetActivePool(PooledObjectType.Gift)
                               .Where(cb => cb.GetComponent<GiftBehaviour>()!=null)
                               .Select(cb => cb.GetComponent<GiftBehaviour>())
                               .Where(cb => !(cb.State is StartState) && !(cb.State is ThrowState))
                               .ToList();
            return deactivatedCubes;
        }

       

      
      
     
       

        public void CleanState()
        {
            _throwCount = 0;
            if (_waitForLevelEnd!=null)
            {
                GameService.Instance.StopCoroutine(_waitForLevelEnd);
                _waitForLevelEnd = null;
            }
        }

        public List<GiftBehaviour> GetCubesInRadius(GiftBehaviour excludeCube, Vector3 WorldPosition, float radius, bool same = true)
        {
            List<GiftBehaviour> output = new List<GiftBehaviour>();
            Collider[] hitColliders = Physics.OverlapSphere(WorldPosition, radius);
            List<Collider> hited = hitColliders.Where(c => c.gameObject != excludeCube.gameObject).ToList();
            SafeGiftItem baseCubeData = excludeCube.Data;
            foreach (Collider hitCollider in hited)
            {
                GiftBehaviour hitedControl = hitCollider.GetComponent<GiftBehaviour>();
                if (hitedControl != null)
                {                    
                    if (same)
                    {
                        if (hitedControl.Data == baseCubeData)
                        {
                            output.Add(hitedControl);
                        }
                    }
                    else
                    {
                        output.Add(hitedControl);
                    }
                }


            }
            return output;
        }
        public Vector3 GetDirectionToSame(GiftBehaviour cubeControl, float radius)
        {
            float minDistance = 100;
            Vector3 output = Vector3.zero;
            Vector3 basePosition = cubeControl.gameObject.transform.position;
            var hited = GetCubesInRadius(cubeControl, cubeControl.transform.position, radius);

            foreach (var hitCollider in hited)
            {
                Vector3 DeltaVector = hitCollider.transform.position - basePosition;
                float Delta = DeltaVector.magnitude;


                    if (Delta < minDistance)
                    {
                        minDistance = Delta;
                        output = DeltaVector;
                    }

            }

            return output;

        }
        public void OnThrow(GameObject giftObject, int id)
        {
            if (GameService.Instance.GameStarted)
            {

                if (_nextGiftReady)
                {
                    _nextGiftReady = false;
                    _waitForLevelEnd = GameService.Instance.StartCoroutine(CoolDown());
                    if (giftObject != null)
                    {
                        GiftBehaviour BulletGift = giftObject.GetComponent<GiftBehaviour>();
                        if (BulletGift!=null)
                        {
                            BulletGift.State = new ThrowState();
                            BulletGift.Play(GiftAnimations.Throw);
                            _throwCount++;
                            UIWindowsManager.GetWindow<MainWindow>().SetGiftsInfo(GameService.Instance.currentLevel.Giftscount - _throwCount);
                        }
                        
                    }
                }
            }

        }

        IEnumerator CoolDown()
        {
            yield return new WaitForSeconds(SceneLoader.sceneSettings.coolDown);
            if (_throwCount<GameService.Instance.currentLevel.Giftscount)
            {
               SpawnGift(); 
            }
            else
            {
                //подарки кончились  - стоп игра
                yield return new WaitForSeconds(2f);
                GameService.Instance.StopGame();
            }
            _waitForLevelEnd = null;
        }

        public void SpawnGift()
        {
            _newGift = GetGift();
            if (_newGift.GameObject != null)
            {
                //_newCube.GameObject.transform.position = GameService.Instance.GunController.GunSpawnPoint.transform.position;

                _newGift.GameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                _newGift.GameObject.SetActive(true);                                              
                GiftBehaviour cubeControl = _newGift.GameObject.GetComponent<GiftBehaviour>();
                GameService.Instance.GunController.Santa.HookTheGift(_newGift);
                cubeControl?.Play(GiftAnimations.NewOne);
                
            }
            _nextGiftReady = _newGift.GameObject != null;
        }
       

        private PooledObject GetGift(IGiftState state  = null)
        {
            if (state==null)
            {
                state = new StartState();
            }
            var stoneCheck = _newGift.GameObject?.GetComponent<StoneBehaviour>();
            PooledObject gift;
            if (GameService.Instance.currentLevel.LevelNumber > SceneLoader.sceneSettings.StoneShowLevel && stoneCheck ==null && ThrowCount>1 )
            {
                int _coinDrop = UnityEngine.Random.Range(0, 10);
                if (_coinDrop<2)
                {
                    gift = ObjectPoolManager.Instance.GetPooledObject(PooledObjectType.Brick);
                }
                else
                    gift = ObjectPoolManager.Instance.GetPooledObject(PooledObjectType.Gift);
            }
            else
                gift = ObjectPoolManager.Instance.GetPooledObject(PooledObjectType.Gift);



            if (gift.GameObject != null)
            {
                    
                GiftBehaviour GiftControl = gift.GameObject.GetComponent<GiftBehaviour>();
                    
                if (GiftControl!=null)
                {
                    //CubControl.State = null;
                    
                    
                    GiftControl.UpdateContent(state, NextCubeContent);
                    gift.GameObject.transform.localScale = SceneLoader.sceneSettings.baseCubeScal;
                }
            }

            return gift;
        }

        public void DeactivateFalled()
        {
            Debuger.layoutThis("DeactivateFalled");
            var allActive = ObjectPoolManager.GetActivePool(PooledObjectType.Gift);
            foreach (var gift in allActive)
            {
                if (gift.transform.position.y < (GameService.Instance.GunController.gameObject.transform.position.y - FALL_LIMIT))
                {                    
                    gift.transform.position = GameService.Instance.GunController.GunSpawnPoint.transform.position;
                    gift.transform.rotation = Quaternion.identity;
                    gift.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gift.ReleaseToPool(PooledObjectType.Gift);
                }
            }
        }

       

        
    }

}