using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VOrb.CubesWar
{

    public class Joystik: MonoBehaviour, ITouchSensetive 
    {
        [SerializeField] private GameObject _parentContainer;        
        [SerializeField] private GameObject _joystikZone; // объект картинки(если используется) зоны джостика
        [SerializeField] private JoystykData _outputInfo;
        [SerializeField] private float _minAlfa = 0.35f;
        [SerializeField] private float _maxAlfa = 0.70f;
        [SerializeField] public bool DirectionChoosed { get => _outputInfo.DirectionChoosed; private set => _outputInfo.DirectionChoosed = value; }

        public bool isStatic = true;
        public Vector2 Direction { get => _outputInfo.Direction; private set => _outputInfo.Direction = value; }
        public float Angle { get => _outputInfo.Angle; private set => _outputInfo.Angle = value; }
        public float Power { get => _outputInfo.Power; private set => _outputInfo.Power = value; }
        public float Speed { get => _outputInfo.Speed; private set => _outputInfo.Speed = value; }
        public int Horizontal { get => _outputInfo.Horizontal; private set => _outputInfo.Horizontal = value; }
        public int Vertical { get => _outputInfo.Vertical; private set => _outputInfo.Vertical = value; }
        
        public UnityEvent TapEvent { get ; set; }
        public UnityEvent SvipeEvent { get; set; }
        public TouchEventResponse[] responses { get; set ; }
        public int TapID { get; private set; }

        private Vector3 _p_StartPosition_LOCAL;
        private LifeZone _lifeZone;
        private float _TapStartTime;        
        private Camera _cam;
        private Vector3 _p_JostikPos_nul;
        private float _p_ZoneRadius;
        private Vector2 _thisTapCoord;
        

        public void Init()
        {
            if (GameService.Instance.Sensor != null)
            {
                TouchRegistrator sensor = GameService.Instance.Sensor;
                SetJoystikParam(true, sensor.CamMin + new Vector2(sensor.Width*.05f, sensor.Height * .05f),
                    sensor.CamMax - new Vector2(sensor.Width * .05f,sensor.Height/2-sensor.Height*.05f)
                );
            }
            else
                SetJoystikParam(true);
        }
        void Start()
        {
            _outputInfo = new JoystykData();

            responses = new TouchEventResponse[] {
                new TouchEventResponse(OnPointerDownEvent), //DownTouch
                new TouchEventResponse(OnPointerUpEvent), //UpTouch
                new TouchEventResponse(OnDragEvent), //DragTouch
                new TouchEventResponse((sv)=>{ }) //DropTouch
            };


            _p_StartPosition_LOCAL = _parentContainer.transform.localPosition;
            DirectionChoosed = false;
            _cam = Camera.main;
            TapID = JoystykBaseConst.NO_ID;
            _thisTapCoord = Vector2.zero;
            _p_ZoneRadius = (int)(_joystikZone.gameObject.GetComponent<RectTransform>().rect.width/2);

            Vector3 CenterWorldPoint, CenterScreenPoint, WorldRightContainerPoint, ScreenRightContainerPoint;
            CenterWorldPoint = _joystikZone.transform.position;
            CenterScreenPoint = _cam.WorldToScreenPoint(CenterWorldPoint);
            
            _p_JostikPos_nul = CenterScreenPoint;


            Vector3 right = _joystikZone.gameObject.GetComponent<RectTransform>().rect.center.GetVector3(CenterWorldPoint.z) + new Vector3(_p_ZoneRadius, 0, 0);
            WorldRightContainerPoint = _joystikZone.gameObject.GetComponent<RectTransform>().TransformPoint(right);
            ScreenRightContainerPoint = _cam.WorldToScreenPoint(WorldRightContainerPoint);

            _p_ZoneRadius = ScreenRightContainerPoint.x - CenterScreenPoint.x;            

        }

        public void SetJoystikParam(bool CanMoveinArea,Vector2 ScreenPointLeftBottom = new Vector2(), Vector2 ScreenPointRightTop = new Vector2())
        {           
            ScreenPointLeftBottom = (ScreenPointLeftBottom == null) ? LifeZone.Empty : ScreenPointLeftBottom;
            ScreenPointRightTop = (ScreenPointRightTop == null) ? LifeZone.Empty : ScreenPointRightTop;

            isStatic = !CanMoveinArea;
            _lifeZone = new LifeZone(ScreenPointLeftBottom, ScreenPointRightTop);
            if (_lifeZone.isEmpty())
            {
                throw new System.Exception("Empty touch Zone!");
            }

        }

        public bool SetPosition(Swipe startTouch)
        {                     
            if ((_lifeZone.Contains(startTouch.screenPoint_start, _p_ZoneRadius) || _lifeZone.isEmpty()) && (TapID == JoystykBaseConst.NO_ID))
            {
                Power = 0;
                Vector3 p_new_point;                
                Transform mainTrans = _parentContainer.transform;
                p_new_point = new Vector3(startTouch.screenPoint_start.x, startTouch.screenPoint_start.y, _p_JostikPos_nul.z);
                mainTrans.position = _cam.ScreenToWorldPoint(p_new_point);
                //mainTrans.position = new Vector3(mainTrans.position.x, mainTrans.position.y, JoystykBaseConst.ZOrder_zone);
                SetNewNulPosition(startTouch.SwipeID);
                //OnPointerDownEvent(startTouch.fingerId);
                return true;
            }
            else
                return false;

        }
        public void DropState()
        {
            _TapStartTime = 0;
            TapID = JoystykBaseConst.NO_ID;
            SetNewNulPosition(JoystykBaseConst.NO_ID);
            EventPublisher.JoystikUp.Publish(Direction, 0);
            DirectionChoosed = false;            
        }
        
        /*
         * Закрытые методы
         */
        private void SwingButtonAlfa(int state)
        {
            var BZone = _joystikZone.GetComponent<Button>();
            var bzColors = BZone.colors;
            if (state == JoystykBaseConst.BTN_UP)
                bzColors.normalColor = new Color(1, 1, 1, _minAlfa);
            else
                bzColors.normalColor = new Color(1, 1, 1, _maxAlfa);
            BZone.colors = bzColors;
            var BJ = gameObject.GetComponent<Button>();
            var bJColors = BJ.colors;
            if (_maxAlfa == 0)
            {
                bJColors.normalColor = new Color(1, 1, 1, 0f);
                bJColors.pressedColor = new Color(1, 1, 1, 0f);
                bJColors.selectedColor = new Color(1, 1, 1, 0f);
                bJColors.highlightedColor = new Color(1, 1, 1, 0f);
            }
            else
            {
                bJColors.normalColor = new Color(1, 1, 1, 0.23f);
                bJColors.pressedColor = new Color(1, 1, 1, 1f);
                bJColors.selectedColor = new Color(1, 1, 1, 0.51f);
                bJColors.highlightedColor = new Color(1, 1, 1, 51f);
            }
            if (state == JoystykBaseConst.BTN_UP)
                bJColors.normalColor = new Color(1, 1, 1, _minAlfa);
            else
                bJColors.normalColor = new Color(1, 1, 1, _maxAlfa);
            BJ.colors = bJColors;
        }
        private void SetNewNulPosition(int CurrentTapID)
        {

            TapID = CurrentTapID;
           
            if (TapID == JoystykBaseConst.NO_ID)
            {
                _parentContainer.transform.localPosition = _p_StartPosition_LOCAL;
                gameObject.transform.position = _parentContainer.transform.position;
            }

            Transform transform = _joystikZone.transform;
            _p_JostikPos_nul = _cam.WorldToScreenPoint(transform.position);
        }

        private bool CheckPositionIsNew()
        {
            Vector2 CurLocal = _parentContainer.transform.localPosition;            
            if (CurLocal != (Vector2)_p_StartPosition_LOCAL)
            {
                return true;
            }
            return false;
        }

        void UpdatePosition()
        {
            if (CheckPositionIsNew()) SetNewNulPosition(TapID);
            Touch[] AllTouches = Input.touches;
            if (TapID != JoystykBaseConst.NO_ID)
            {
                _thisTapCoord = Input.mousePosition;
                if (Input.touchCount > 1)
                {
                    for (int i = 0; i < AllTouches.Length; i++)
                    {
                        if (AllTouches[i].fingerId == TapID)
                        {
                            _thisTapCoord = new Vector2(AllTouches[i].position.x, AllTouches[i].position.y);
                        }
                    }
                }
            }
        }

        public void OnPointerDownEvent(Swipe data)
        {
            if (SetPosition(data))
            {
                DirectionChoosed = false;
                TapID = data.SwipeID;
                _TapStartTime = Time.realtimeSinceStartup;
                EventPublisher.JoysticFirstTap.Publish(data.screenPoint_start);
                // SwingButtonAlfa(JoystykBaseConst.BTN_DOWN);
            }                       
        } 
        public void OnPointerUpEvent(Swipe data)
        {
            if (TapID !=JoystykBaseConst.NO_ID && TapID==data.SwipeID)
            {
                //SwingButtonAlfa(JoystykBaseConst.BTN_UP);
                _TapStartTime = 0;
                DirectionChoosed = true;
                TapID = JoystykBaseConst.NO_ID;
                SetNewNulPosition(JoystykBaseConst.NO_ID);
                if (CheckSelectedDirection())
                {
                    EventPublisher.JoystikUp.Publish(Direction, Power);
                }
                else
                    EventPublisher.JoystikUp.Publish(Direction, 0);
            }
            

            
        }

        
        public void OnDragEvent(Swipe data)
        {
            
            UpdatePosition();
            if (TapID != JoystykBaseConst.NO_ID)
            {
                if (!_lifeZone.Contains(data.screenPoint_end))
                {
                    DropState();
                    return;
                }

                Vector3 p_CurPos, p_Dist;
                float p_CurRadius, Alfa2, deltaTime;
                p_CurPos = new Vector3(_thisTapCoord.x, _thisTapCoord.y, _p_JostikPos_nul.z);
                p_Dist = p_CurPos - _p_JostikPos_nul;
                p_CurRadius = Mathf.Sqrt((p_Dist.x * p_Dist.x) + (p_Dist.y * p_Dist.y));
                
                Alfa2 = Mathf.Atan2(p_Dist.y, p_Dist.x);
                //публично транслируем Угол поворота
                Angle = Alfa2 * Mathf.Rad2Deg;

                if (Angle < 0)
                {
                    Angle = 360 + Angle;
                }
                //публично транслируем направление и силу
                if (p_CurRadius < _p_ZoneRadius)
                {
                    gameObject.transform.position = _cam.ScreenToWorldPoint(p_CurPos);
                    Power = (p_CurRadius / _p_ZoneRadius) * 100;
                    Direction = new Vector2(p_Dist.x, p_Dist.y);
                }
                else
                {
                    Power = 100f;
                    float inZone_X, inZone_Y;
                    float overrun = p_CurRadius - _p_ZoneRadius;
                    if (!isStatic)
                    {
                        if (overrun > (_p_ZoneRadius / 7))
                        {
                            overrun /= 2;
                            Vector3 deltaNewPos;
                            deltaNewPos.x = overrun * Mathf.Cos(Alfa2);
                            deltaNewPos.y = overrun * Mathf.Sin(Alfa2);
                            deltaNewPos.z = 0f;
                            Vector3 p_new_point;
                            Transform mainTrans = _parentContainer.transform;
                            p_new_point = _p_JostikPos_nul + deltaNewPos;
                            if (_lifeZone.Contains(new Vector2(p_new_point.x, p_new_point.y)))
                            {
                            mainTrans.position = _cam.ScreenToWorldPoint(p_new_point);
                            //mainTrans.position = new Vector3(mainTrans.position.x, mainTrans.position.y, JoystykBaseConst.ZOrder_zone);
                            }                                                  

                        }
                    }
                    // двигаем по кромке радиуса

                    inZone_X = _p_ZoneRadius * Mathf.Cos(Alfa2);
                    inZone_Y = _p_ZoneRadius * Mathf.Sin(Alfa2);
                    // Debug.Log(" inZoneX,Y: "+ inZone_X+", " + inZone_Y);
                    Direction = new Vector2(inZone_X, inZone_Y);
                    p_CurPos = new Vector3(_p_JostikPos_nul.x + inZone_X, _p_JostikPos_nul.y + inZone_Y, _p_JostikPos_nul.z);
                    gameObject.transform.position = _cam.ScreenToWorldPoint(p_CurPos);
                }
                //транслируем скорость
                if (Direction.x >= 0) Horizontal = 1;
                else Horizontal = -1;
                if (Direction.y >= 0) Vertical = 1;
                else Vertical = -1;
                deltaTime = Time.realtimeSinceStartup - _TapStartTime;
                Speed = p_CurRadius / deltaTime;
                if (p_CurRadius < (_p_ZoneRadius / 10)) _TapStartTime = Time.realtimeSinceStartup;

                Vector3 leftRange = new Vector3(-0.5f,0f,0.9f);
                Vector3 rightRange = new Vector3(0.5f, 0f, 0.9f);
                var newDir = Vector3.Lerp(leftRange, rightRange, 1 - ((Direction.normalized.x + 1) / 2)) * 11f; // new Vector3(Direction.x, 0, Direction.y) * -1;
                
                //Debug.DrawRay(GameService.Instance.GunController.gameObject.transform.position,newDir,Color.green);

                if (CheckSelectedDirection())
                {                 
                    //if (Vector2.Angle(_lastPublish.Item1, Direction) >= _sensetiveLevel 
                    //    || Mathf.Abs(_lastPublish.Item2-Power)>_sensetivePower)
                    //{
                    //    _lastPublish = (Direction, Power);
                        EventPublisher.JostikMovement.Publish(Direction, Power);
                    //}
                    
                }
                else
                    EventPublisher.JostikMovement.Publish(Direction, 0);



            }


        }

        public void InvokeTouchEvent(TouchEvent updateType, Swipe data)
        {
            responses[(int)updateType].Invoke(data);
        }


   
        public bool CheckSelectedDirection()
        {
            return (Vertical<0) && (Power > 10);
        }
    }
} 