using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VOrb.SantaJam;
using VOrb.SantaJam.Levels;

namespace VOrb.SantaJam
{

    public class HomesLife : MonoBehaviour
    {
        private GameObject _lastHome;
        private float _lenght;
        private Coroutine _mover = null;
        private List<PooledObject> _movableObjects = new List<PooledObject>();
        private List<PooledObject> _enviropment = new List<PooledObject>();

        [Header("-- Базовые настройки --")]
        [SerializeField] private float _initStartZ = 25f;
        [SerializeField] private float _leftXBound = -3.38f;
        [SerializeField] private float _rightXBound = 3.78f;
        [SerializeField,
         Tooltip("Во сколько количество домов больше числа подарков")]
        private int _GiftsMultiply = 2;
        [SerializeField,
         Tooltip("Для рандома растояния между рядами домов - единичное растояние на этот коэф.")]
        private float _rowMultiply = 2.5f;
        [SerializeField] private float _roadBeginOffset = 20f;
        //расстояние между домами
        [SerializeField] private float spaceBtwHZ = 3f;
        [SerializeField] private float basespeed = 2f;
        
        [Header("--Онлайн info --")]
        [SerializeField] private float startX = -2.803698f;
        [SerializeField] private float startY = -12.85f;
        [SerializeField] private float currentZ = 65f;

        public void PushToMoveList(PooledObject forAdd) => _movableObjects.Add(forAdd);


        private void OnEnable()
        {
            currentZ = _initStartZ;

            _mover = StartCoroutine(HomeMover());

        }
        private void OnDisable()
        {
            if (_mover!=null)
            {
                StopCoroutine(_mover);
                _mover = null;
            }
            _lastHome = null;
            _lenght = 0;
        }
        public void DeleteFromMove(GameObject gameObject) 
        {
            var obj = _movableObjects.FirstOrDefault(p => p.GameObject == gameObject);
            _movableObjects.Remove(obj);            
        }
        public void CleareEnviropment()
        {
            foreach (var item in _enviropment)
            {
                item.ReleaseToPool();
            }
            _enviropment.Clear();
        }
        public void ReleseMovebleObject(GameObject gameObject)
        {
            var obj = _movableObjects.FirstOrDefault(p => p.GameObject == gameObject);
            if (obj.GameObject!=null)
            {
                _movableObjects.Remove(obj);
                obj.ReleaseToPool();
            }   

        }
        public void CleareHomeEnviropment(Home home)
        { 
            
            if (home != null)
            {
                foreach (var point in home.Points)
                {
                    if (point.transform.childCount>0)
                    {
                        var obj = _enviropment.FirstOrDefault(p => p.GameObject == point.transform.GetChild(0).gameObject);
                        if (obj.GameObject  != null)
                        {
                            obj.ReleaseToPool();
                        }
                    }
                }
            }
        }


        private IEnumerator HomeMover()
        {
            yield return null;
            _movableObjects.Clear();
            bool isLevelNumered = GameService.Instance.CurrentLevel.Contains<NumberedHouseChimney>();
            SpawnHomes();

            float santaZ = GameService.Instance.SantaContainer.transform.position.z;
            float pathProgress = 0;
            //пул окружения
            

            while (GameService.Instance.GameStarted)
            {
                yield return new WaitForFixedUpdate();

                var MoveList = _movableObjects.Where(o => o.GameObject != null).ToList(); 

                if (_lastHome.transform.position.z <= santaZ || pathProgress>=0.99f || MoveList.Count == 0)
                {
                    Debug.Log("доехал до конца pathProgress=" + pathProgress);
                    //доехал до конца
                    GameService.Instance.StopGame();
                    break;
                }
                
                foreach (var movable in MoveList)
                {
                    var homeCntrl = movable.GameObject.GetComponent<Home>();
                    if (!movable.GameObject.activeInHierarchy)
                    {
                        if (homeCntrl!= null
                            &&movable.GameObject.transform.position.z <= _initStartZ + _roadBeginOffset)
                        {
                            if (!homeCntrl.Destroyed)
                            {
                                movable.GameObject.SetActive(true);
                                
                            }
                        
                        }
                        
                    }
                    if (movable.GameObject.transform.position.z <=_initStartZ+_roadBeginOffset/3 && isLevelNumered)
                    {
                            var Chimney = homeCntrl?.GetComponentInChildren<ChimneySensor>();
                            if (Chimney != null)
                            {
                                homeCntrl.SetActiveNumber(Chimney.StatesData.Contains(ChimneyState.Numered));
                            }
                    }
                    // if (movable.GameObject.activeInHierarchy)
                    // {
                    if (movable.GameObject.transform.position.z <= santaZ+7f && homeCntrl!=null)
                    {
                        if (movable.GameObject.activeInHierarchy)  //чтоб не вырос разрушенный домик +)
                        {
                            movable.GameObject.transform.localScale =
                                new Vector3(
                                movable.GameObject.transform.localScale.x,
                                Mathf.Clamp(movable.GameObject.transform.localScale.y - 0.09f * Time.fixedDeltaTime, 0.5f, int.MaxValue),
                                movable.GameObject.transform.localScale.z
                                );
                        }

                        
                    }
                        
                        if (movable.GameObject.transform.position.z >= santaZ)
                        {
                            movable.GameObject.transform.position -= new Vector3(0, 0, 1) * basespeed * GameService.Instance.CurrentLevel.Speed * Time.deltaTime;
                        }
                        else
                        {
                            Home homeControl = movable.GameObject.GetComponent<Home>();
                            if (homeControl != null)
                            {
                                CleareHomeEnviropment(homeControl);
                            }
                            DeleteFromMove(movable.GameObject);
                            movable.ReleaseToPool();

                        }
                   // }

                }

                pathProgress = 1f -
                    Mathf.InverseLerp(santaZ + _roadBeginOffset / 3, santaZ + _roadBeginOffset + _lenght,
                    Mathf.Clamp(_lastHome.transform.position.z, santaZ + _roadBeginOffset / 3,
                    santaZ + _roadBeginOffset + _lenght));
                UIWindowsManager.GetWindow<MainWindow>().SetProgressSlider(pathProgress);
            }
         
            _mover = null;

        }

        private void SpawnHomes()
        {
            
            int wrongHomes = Mathf.CeilToInt(GameService.Instance.CurrentLevel.Giftscount*_GiftsMultiply *0.5f);
            Texture2D texture = DataBaseManager.Instance.GetRandomHousesTexture();
            float firstHomeZ = 0;
            int minTypeNum = (int)PooledObjectType.Homes1;
            int minTypeEnvir = (int)PooledObjectType.Envir1;
            int maxTypeEnvir = minTypeEnvir + SceneLoader.SceneSettings.EnvirPrefab.Length;
            float homeSX = 0f;
            float prevHomeSX = 0f;
            float prevX = 0f;
            int wrongSetup = 0;

            int wrongRow = 0;

            int multiply = GameService.Instance.CurrentLevel.Speed > 2f ? _GiftsMultiply: _GiftsMultiply+1;
            for (int i = 0; i < GameService.Instance.CurrentLevel.Giftscount * multiply; i++)
            {
                prevX = startX;
                prevHomeSX = homeSX;

                float spaceBtw = Random.Range(spaceBtwHZ, spaceBtwHZ * _rowMultiply);

                int randNumHouse = UnityEngine.Random.Range(minTypeNum, minTypeNum + SceneLoader.SceneSettings.HomesPrefab.Length);
                PooledObject Home, Envir;
                Home = ObjectPoolManager.Instance.GetPooledObject((PooledObjectType)randNumHouse);
                
                var homeData = Home.GameObject.GetComponent<Home>();
                homeData.SetActiveLight(false);
                homeData.SetActiveSmoke(false);
                homeData.SetActiveFence(false);

                homeData.type = Home.SelfType;
                //в зависимости от уровня настраиваем состояние сенсора (случайно из доступных фич)
                //int randFiches = Random.Range(-3, GameService.Instance.currentLevel.GetFiches().Count);
                //пока костыль (спросить у Вит, что правильнее свич или доп список с next или...)
                int randPoint = Random.Range(0, 3);
                int randFiches = UnityEngine.Random.Range(minTypeEnvir, maxTypeEnvir);
                if (randFiches < maxTypeEnvir)
                {
                    
                    Envir = ObjectPoolManager.Instance.GetPooledObject((PooledObjectType)randFiches, 
                                                homeData.Points[randPoint].transform);   // сделал тебе перегрузку метода сразу чтоб устанавливать родителя

                                     
                    Envir.GameObject.transform.localPosition = Vector3.zero; // если у префаба были какие-то позиции, то они станут локальным сдвигом
                    //надо чтобы смотрели в сторону главной камеры(только для снеговиков)
                    if (Envir.SelfType == PooledObjectType.Envir6 )
                    {
                        Envir.GameObject.transform.LookAt(Envir.GameObject.transform.position + new Vector3(0, 0, -2));
                        Envir.GameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, -90));
                    }
                    Envir.GameObject.SetActive(true);

                    _enviropment.Add(Envir);
                }
               
                var HomeChildren = Home.GameObject.GetComponentInChildren<ChimneySensor>();
                
                HomeChildren.StatesData.Clear();  // а то на следующем уровне добавит к предыдущим фичам ещё
                HomeChildren.HideNumber();


                randFiches = UnityEngine.Random.Range(0, 5);

                //только одну фичу!

                var allFiches = GameService.Instance.CurrentLevel.GetFiches();
                if (allFiches.Count > 0)
                {
                    if (UnityEngine.Random.value > (0.5f + 0.2f*wrongRow) && wrongSetup < wrongHomes)
                    {
                        //ficha
                        randFiches = UnityEngine.Random.Range(0, allFiches.Count);

                        var fich = allFiches[randFiches];
                        if (fich is SmokedChimney)
                        {
                            HomeChildren.StatesData.Add(ChimneyState.Smoked);
                            homeData.SetActiveSmoke(true);

                        }

                        if (fich is LightHouseChimney)
                        {
                            HomeChildren.StatesData.Add(ChimneyState.LightOn);
                            homeData.SetActiveLight(true);
                        }

                        if (fich is NumberedHouseChimney)
                        {
                            var nums = GameService.Instance.CurrentLevel.GetHousesNumber();
                            int maxCorrectValue = nums.Max();
                            HomeChildren.HouseNumber = UnityEngine.Random.Range(maxCorrectValue + 1, maxCorrectValue + 11);
                            HomeChildren.StatesData.Add(ChimneyState.Numered);
                        }

                        wrongSetup++;
                        wrongRow++;
                    }
                    else
                    {
                        //ne ficha
                        wrongRow = 0;
                        LevelInfo level = GameService.Instance.CurrentLevel;
                        if (level.Contains<NumberedHouseChimney>())
                        {
                            HomeChildren.StatesData.Add(ChimneyState.Numered);
                            var nums = level.GetHousesNumber();
                            HomeChildren.HouseNumber = nums[UnityEngine.Random.Range(0, nums.Count)];
                        }
                    }


                }



                //Home.GetComponentInChildren<ChimneySensor>().StatesData[0] = (ChimneyState)randFiches;
                //делаем активным, для определения размеров 
                Transform homeMesh = Home.GameObject.transform.GetChild(0);
                if (homeMesh!=null)
                {
                    homeMesh.Find("props")?.gameObject.SetActive(false);
                }
                
                Home.GameObject.SetActive(true);
                //case ChimneyState.
                var HomeController = Home.GameObject.GetComponent<BoxCollider>();
                //находим максимально возможное расстояние от центра
                homeSX = Mathf.Sqrt(HomeController.bounds.extents.x
                    * HomeController.bounds.extents.x
                    + HomeController.bounds.extents.z
                    * HomeController.bounds.extents.z);

                var leftSpace = Mathf.Abs(_leftXBound - prevX);
                var rightSpace = Mathf.Abs(prevX - _rightXBound);

                //проверяем можно ли следующий домик поместить по этой же Z
                if (Mathf.Max(leftSpace, rightSpace) - prevHomeSX > (homeSX))
                {
                    if (leftSpace < rightSpace)
                    {
                        startX = Random.Range(prevX + prevHomeSX + homeSX, _rightXBound);
                    }
                    else
                    {
                        startX = Random.Range(_leftXBound, prevX + prevHomeSX + homeSX);
                    }
                    homeSX = _rightXBound - _leftXBound;
                }
                else
                {
                    //получаем случайное значение позиции по Х
                    startX = Random.Range(_leftXBound, _rightXBound);
                    currentZ = currentZ + spaceBtw;
                }



                Home.GameObject.transform.position = new Vector3(startX, startY, currentZ);
                Home.GameObject.transform.localScale = Home.GameObject.transform.localScale.SetZTo(Home.GameObject.transform.localScale.y);

                
                HomeChildren.TurnOn();
                
                HomeChildren.GetComponentInParent<MeshRenderer>().material.mainTexture = texture;

                firstHomeZ = i == 0 ? Home.GameObject.transform.position.z : firstHomeZ;
                _lenght = Home.GameObject.transform.position.z - firstHomeZ;
                _lastHome = Home.GameObject;

                _movableObjects.Add(Home);
                //
                Home.GameObject.SetActive(false);
            }

        }




    }
}