using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VOrb.SantaJam
{
    public class GiftBehaviour : MonoBehaviour
    {
        public bool CanUseScale = false;

        private SafeGiftItem _data;
        public SafeGiftItem Data { get => _data; set => _data = value; }

        public float ThrowAnimDuration = 1f;
        public float NewAnimDuration = 1f;

        [SerializeField] private AnimationCurve _bodyScaleThrow;
        [SerializeField] private AnimationCurve _bodyScaleNew;

        private Rigidbody _selfBody = null;
        private Coroutine _PlayCubThrow = null;
        private Coroutine _PlayCubScale = null;

        public Vector3 StartScale { get; private set; }
        private IGiftState _state;

        public IGiftState State { 
            get { return _state; } 
            set
            {
                if (_state != null)
                {
                    _state = _state.Transaction(this, value);
                }
                else
                {
                    _state = value;
                }
            } }

        private void Start()
        {
            _selfBody = GetComponent<Rigidbody>();

        }

        public void OnEnable()
        {
            this.TryStartCoroutine(DropVelocity());
        }

        public void OnDisable()
        {
            StopAllCoroutines();
            _state = null;
        }

        IEnumerator DropVelocity()
        {
            yield return new WaitForFixedUpdate();
            if (_selfBody != null)
            {
                _selfBody.velocity = Vector3.zero;
            }
        }


        public void UpdateContent(IGiftState updateState, SafeGiftItem content)
        {
            if (content != null)
            {
                Data = content;
                State = updateState;
            }

        }

        public void Play(GiftAnimations anim, Action callback = null)
        {
            if (_PlayCubScale != null)
            {
                StopCoroutine(_PlayCubScale);
                transform.localScale = StartScale;
                _PlayCubScale = null;
            }
            switch (anim)
            {
                case GiftAnimations.Throw:
                    if (_selfBody != null)
                    {
                      _PlayCubThrow = this.TryStartCoroutine(AnimationThrow(true,
                          GameService.Instance.SantaController.ThrowTarget));
                    }
                    break;
                case GiftAnimations.NewOne:
                    _PlayCubScale = this.TryStartCoroutine(AnimateScaleCurve(_bodyScaleNew, NewAnimDuration, callback));
                    break;
                default:
                    break;
            }
        }

        private IEnumerator AnimateScaleCurve(AnimationCurve scale, float duration,  Action callback = null)
        {
            float expiredSeconds = 0;
            Vector3 halfExtents = Vector3.zero;
            float progress = 0;
           
            Vector3 StartPosition = transform.position;
            StartScale = transform.localScale;

            while (progress < 1)
            {

                expiredSeconds = expiredSeconds > duration ? 0 : (expiredSeconds + Time.deltaTime);

                progress = expiredSeconds / duration;

                transform.localScale = CanUseScale ? StartScale * scale.Evaluate(progress) : StartScale;

                yield return null;
            }
                callback?.Invoke();

            _PlayCubScale = null;
        }

        private IEnumerator AnimationThrow(bool isPhysicThrow, Vector3 targetPoint)
        {
            float speed = 5;
            float angle = Mathf.Deg2Rad * 45;
            float expiredSeconds = 0;
            float progress = 0;

            StartScale = transform.localScale;
            Vector3 StartPosition = transform.position;

            speed = Vector3.Distance(StartPosition,targetPoint) / (ThrowAnimDuration* Mathf.Cos(angle));
            while (progress < 1)
            {
                expiredSeconds = expiredSeconds > ThrowAnimDuration ? 1 : (expiredSeconds + Time.deltaTime);

                progress = expiredSeconds / ThrowAnimDuration;

                transform.localScale = CanUseScale ? StartScale * _bodyScaleThrow.Evaluate(progress) : StartScale;               
                
                yield return null;
            }

            DropThrowCoroutine();
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (State is ThrowState )
            {
                ValidateCollision(collision.gameObject);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (State is ThrowState )
            {
                ValidateCollision(other.gameObject);
            }
        }


        private void ValidateCollision(GameObject collision)
        {

            if (_PlayCubThrow != null)
            {
                if (collision.gameObject.GetComponent<CoroutineStoper>() != null)
                {
                    DropThrowCoroutine();
                }
            }
            var Chimney = collision.GetComponent<ChimneySensor>();
            if (collision.GetComponent<GroundBehaviour>() != null)
            {
                //земелька
                UpdateContent(new DeactivatedState(), Data);
                GameService.Instance.GameElements.GetComponentInChildren<HomesLife>().PushToMoveList(new PooledObject(gameObject,PooledObjectType.Gift));
            }

            if (Chimney != null)
            {
                bool Succsess;
                var ChimseyStates = Chimney.StatesData;

                Succsess = true;
                foreach (var type in ChimseyStates)
                {
                    if (type == ChimneyState.Numered)
                    {
                        Debug.Log("Проверка по номеру дома");
                        if (GameService.Instance.CurrentLevel.GetHousesNumber().Contains(Chimney.HouseNumber))
                        {
                            Debug.Log("Номер есть в списке");
                        }
                        else
                            Succsess = false;
                    }
                    if (type == ChimneyState.Smoked)
                    {
                        Debug.Log("Подожгли колпак");

                        Succsess = false;
                    }
                    if (type == ChimneyState.LightOn)
                    {
                        Debug.Log("Спалился!");
                        Succsess = false;
                    }

                }
                if (Succsess)
                {
                    gameObject.ReleaseToPool(PooledObjectType.Gift);

                    
                    Debug.Log("Подарок доставлен!");
                    
                    GameService.VFXDriver.PoofStars(Chimney.ColliderPosition.SetYTo(Chimney.ColliderPosition.y-1.5f));
                    int points = Mathf.Clamp(Mathf.CeilToInt(3 * GameService.Instance.CurrentLevel.Speed), 1, 10);
                    string outString = " +";
                    TextEffectBuilder textBuilder = new TextEffectBuilder().MakeMovable().MakeColored(new Color32(255,176,0,255));                     
                    if (SceneLoader.SceneSettings.IsTestMode)
                    {
                        points += 500;
                    }
                    if (Vector3.Distance(GameService.Instance.SantaController.transform.position, gameObject.transform.position) > 27f)
                    {
                        points *= 2;
                        outString = "HIT! "+outString;
                        textBuilder.MakeColored(Color.red).MakeScaleble(2);
                    }

                    GameService.Instance.SmilesScore += points;
                    UIWindowsManager.GetWindow<MainWindow>().SetTargetsInfo(++GameService.Instance.HitedTargetsCount);

                    UITextEffects.SplashMainScreen(outString + points, 
                        textBuilder
                        .Biuld( Camera.main.WorldToScreenPoint(transform.position)//Chimney.transform.parent.position)
                        , PooledObjectType.NumberPopup1_smile)
                        ); 
                    //UIWindowsManager.GetWindow<MainWindow>().SetSmilesInfo(++GameService.Instance.ScoreGiftsCount);
                    Chimney.TurnOff();
                    Chimney.GetComponentInParent<MeshRenderer>().material.mainTexture = SceneLoader.SceneSettings.GrayTexture;
                    Chimney.transform.parent.Find("props").gameObject.SetActive(true);
                    SoundService.PlaySound(Sound.ChimneySuccsess);

                }
                else
                {
                    // UpdateContent(new DeactivatedState(), Data, 8, CubeUpdateColorData.None);
                    gameObject.ReleaseToPool(PooledObjectType.Gift);
                    GameService.VFXDriver.PoofGift(transform.position);
                    Debug.Log("Неудача!!");
                    SoundService.PlaySound(Sound.ChimneyFailed);
                    UITextEffects.SplashMainScreen("", new TextEffectBuilder()
                    .MakeMovable()
                    .Biuld(Camera.main.WorldToScreenPoint(Chimney.transform.position)
                    , PooledObjectType.NumberPopup3_hmm)
                    );
                }
               
            }
        }

        private void DropThrowCoroutine()
        {
            StopCoroutine(_PlayCubThrow);
            _PlayCubThrow = null;
            transform.localScale = StartScale;
        }


    }    

}
