using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VOrb.CubesWar;
using VOrb.CubesWar.Levels;

namespace VOrb.CubesWar
{

    public class HomesLife : MonoBehaviour
    {
        [Header("-- Базовые настройки --")]
        [SerializeField] private float _initStartZ = 65f;
        [SerializeField] private float _leftXBound = -3.38f;
        [SerializeField] private float _rightXBound = 3.79f;
        [SerializeField,
         Tooltip("Во сколько количество домов больше числа подарков")]
        private int _GiftsMultiply = 2;
        [SerializeField,
         Tooltip("Для рандома растояния между рядами домов - единичное растояние на этот коэф.")]
        private float _rowMultiply = 2f;
        [SerializeField] private float _roadBeginOffset = 2f;
        //расстояние между домами
        public float spaceBtwHZ = 15f;
        public float basespeed = 10f;
        public List<PooledObject> MovableObjects = new List<PooledObject>();
        public List<PooledObject> allEnvir = new List<PooledObject>();





        [Header("--Онлайн info --")]
        //позиция с которой стартуем
        public float startX = 0f;
        public float startY = -10f;
        public float currentZ = 65f;

        //скорость (будем брать из настроек текущего уровня)
        private GameObject _lastHome;
        private float _lenght;
        private Coroutine mover = null;


        private void OnEnable()
        {
            currentZ = _initStartZ;

            mover = StartCoroutine(HomeMover());

        }
        private void OnDisable()
        {
            if (mover!=null)
            {
                StopCoroutine(mover);
                mover = null;
            }
            _lastHome = null;
            _lenght = 0;
        }
        public void DeleteFromMove(GameObject gameObject) 
        {
            var obj = MovableObjects.FirstOrDefault(p => p.GameObject == gameObject);
            MovableObjects.Remove(obj);            
        }
        public void CleareEnviropment()
        {
            foreach (var item in allEnvir)
            {
                item.ReleaseToPool();
            }
            allEnvir.Clear();
        }
        public void ReleseMovebleObject(GameObject gameObject)
        {
            var obj = MovableObjects.FirstOrDefault(p => p.GameObject == gameObject);
            if (obj.GameObject!=null)
            {
                MovableObjects.Remove(obj);
                obj.ReleaseToPool();
            }   

        }
        public void CleareHomePoints(Empty_home home)
        { 
            
            if (home != null)
            {
                foreach (var point in home._points)
                {
                    if (point.transform.childCount>0)
                    {
                        var obj = allEnvir.FirstOrDefault(p => p.GameObject == point.transform.GetChild(0).gameObject);
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
            MovableObjects.Clear();
            bool isLevelNumered = GameService.Instance.currentLevel.Contains<NumberedHouseChimney>();
            SpawnHomes();

            float GunZ = GameService.Instance.GunContainer.transform.position.z;
            float pathProgress = 0;
            //пул окружения
            

            while (GameService.Instance.GameStarted)
            {
                yield return new WaitForFixedUpdate();

                var MoveList = MovableObjects.Where(o => o.GameObject != null).ToList(); 

                if (_lastHome.transform.position.z <= GunZ || pathProgress>=0.99f || MoveList.Count == 0)
                {
                    Debug.Log("доехал до конца pathProgress=" + pathProgress);
                    //доехал до конца
                    GameService.Instance.StopGame();
                    break;
                }
                
                foreach (var movable in MoveList)
                {
                    var homeCntrl = movable.GameObject.GetComponent<Empty_home>();
                    if (!movable.GameObject.activeInHierarchy)
                    {
                        if (homeCntrl!= null
                            &&movable.GameObject.transform.position.z <= _initStartZ + _roadBeginOffset)
                        {
                            if (!homeCntrl.destroyed)
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
                    if (movable.GameObject.transform.position.z <= GunZ+7f && homeCntrl!=null)
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
                        
                        if (movable.GameObject.transform.position.z >= GunZ)
                        {
                            movable.GameObject.transform.position -= new Vector3(0, 0, 1) * basespeed * GameService.Instance.currentLevel.Speed * Time.deltaTime;
                        }
                        else
                        {
                            Empty_home homeControl = movable.GameObject.GetComponent<Empty_home>();
                            if (homeControl != null)
                            {
                                CleareHomePoints(homeControl);
                            }
                            DeleteFromMove(movable.GameObject);
                            movable.ReleaseToPool();

                        }
                   // }

                }

                pathProgress = 1f -
                    Mathf.InverseLerp(GunZ + _roadBeginOffset / 3, GunZ + _roadBeginOffset + _lenght,
                    Mathf.Clamp(_lastHome.transform.position.z, GunZ + _roadBeginOffset / 3,
                    GunZ + _roadBeginOffset + _lenght));
                UIWindowsManager.GetWindow<MainWindow>().SetProgressSlider(pathProgress);
            }
         
            mover = null;

        }

        private void SpawnHomes()
        {
            
            int wrongHomes = Mathf.CeilToInt(GameService.Instance.currentLevel.Giftscount*_GiftsMultiply *0.5f);
            Texture2D texture = DataBaseManager.Instance.GetRandomHousesTexture();
            float firstHomeZ = 0;
            int minTypeNum = (int)PooledObjectType.Homes1;
            int minTypeEnvir = (int)PooledObjectType.Envir1;
            int maxTypeEnvir = minTypeEnvir + SceneLoader.sceneSettings.EnvirPrefab.Length;
            float homeSX = 0f;
            float prevHomeSX = 0f;
            float prevX = 0f;
            int wrongSetup = 0;

            int wrongRow = 0;

            int multiply = GameService.Instance.currentLevel.Speed > 2f ? _GiftsMultiply: _GiftsMultiply+1;
            for (int i = 0; i < GameService.Instance.currentLevel.Giftscount * multiply; i++)
            {
                prevX = startX;
                prevHomeSX = homeSX;

                float spaceBtw = Random.Range(spaceBtwHZ, spaceBtwHZ * _rowMultiply);

                int randNumHouse = UnityEngine.Random.Range(minTypeNum, minTypeNum + SceneLoader.sceneSettings.HomesPrefab.Length);
                PooledObject Home, Envir;
                Home = ObjectPoolManager.Instance.GetPooledObject((PooledObjectType)randNumHouse);
                
                var homeData = Home.GameObject.GetComponent<Empty_home>();
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
                                                homeData._points[randPoint].transform);   // сделал тебе перегрузку метода сразу чтоб устанавливать родителя

                                     
                    Envir.GameObject.transform.localPosition = Vector3.zero; // если у префаба были какие-то позиции, то они станут локальным сдвигом
                    //надо чтобы смотрели в сторону главной камеры(только для снеговиков)
                    if (Envir.SelfType == PooledObjectType.Envir6 )
                    {
                        Envir.GameObject.transform.LookAt(Envir.GameObject.transform.position + new Vector3(0, 0, -2));
                        Envir.GameObject.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, -90));
                    }
                    Envir.GameObject.SetActive(true);

                    allEnvir.Add(Envir);
                }
               
                var HomeChildren = Home.GameObject.GetComponentInChildren<ChimneySensor>();
                
                HomeChildren.StatesData.Clear();  // а то на следующем уровне добавит к предыдущим фичам ещё
                HomeChildren.HideNumber();


                randFiches = UnityEngine.Random.Range(0, 5);

                //только одну фичу!

                var allFiches = GameService.Instance.currentLevel.GetFiches();
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
                            var nums = GameService.Instance.currentLevel.GetHousesNumber();
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
                        LevelInfo level = GameService.Instance.currentLevel;
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

                MovableObjects.Add(Home);
                //
                Home.GameObject.SetActive(false);
            }

        }




    }
}