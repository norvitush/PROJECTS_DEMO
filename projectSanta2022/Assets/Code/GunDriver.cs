using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VOrb.CubesWar
{
    public class GunDriver : MonoBehaviour
    {
        private bool _trajectoryInScreen = false;
        public bool PhysicThrow = false;
        [Serializable]
        private struct LenghtPowerBounds
        {
            [SerializeField] public float Min;
            [SerializeField]public float Max;

            public LenghtPowerBounds(float minBound, float maxBound)
            {
                Min = minBound;
                Max = maxBound;
            }
        }


       
        public const float BASE_ANGLE_Y = 2.1f;
        public const float BASE_GUN_POWER = 1.8f;
        public const float BASE_ISOMETRIC_KOEF = 2f;

        public GameObject GunSpawnPoint;
        
        [SerializeField, Range(0, 1000)] private float GunPower = BASE_GUN_POWER;
        [SerializeField, Range(0, 10)] private float yCorrection = BASE_ANGLE_Y;
        [SerializeField] private SantaController _santa;
        public SantaController Santa => _santa;

        public TrajectoryRenderer trajectoryRenderer;
        public GameObject Floor;
        
        public ForceMode SelectedForceMode;

        public Vector3 ThrowDirection { get=>_throwDirection; set => _throwDirection = value; }
        public Vector3 ThrowTarget => _throwTarget;

        private Vector3 _throwDirection;
        private Vector3 _speed;
        private Vector3 _newDir;
        private Vector3 _throwTarget;
        private bool _needToShoot = false;

        [SerializeField] private Vector3 leftBoundVector = new Vector3(-0.5f, 0f, 0.9f);
        [SerializeField] private Vector3 rightBoundVector = new Vector3(0.5f, 0f, 0.9f);        
        [SerializeField] private LenghtPowerBounds _lenghtPowerBounds;
        [SerializeField] private LenghtPowerBounds NonPhysics_powerBounds;
        private Rigidbody _bullet;

        [Header("DEBUG SECTION")]

        [SerializeField] GameObject GunObject;
        [SerializeField, Range(0f, 30f)] public float lenght = 0;


        private void OnEnable()
        {            
           EventPublisher.JoystikUp.Subscribe(TakeAShot);
           EventPublisher.JostikMovement.Subscribe(TrajectoryShow);
           EventPublisher.JoysticFirstTap.Subscribe(DropSantaAnimation);
            _bullet = null;
        }
        public void OnDisable()
        {
            EventPublisher.JoystikUp.UnSubscribe(TakeAShot);
            EventPublisher.JostikMovement.UnSubscribe(TrajectoryShow);
            EventPublisher.JoysticFirstTap.UnSubscribe(DropSantaAnimation);
        }

        public void DropSantaAnimation(Vector2 tapPosition)
        {
            _santa.DropState();
        }
        public void ReArm()
        {            
            GameService.CubsDriver.SpawnGift();
        }
        private void Update()
        {
            Debug.DrawRay(transform.position, (GunSpawnPoint.transform.position - GunObject.transform.position).normalized * lenght,Color.blue);
            Debug.DrawRay(transform.position, _speed, Color.green);
            Debug.DrawRay(_throwTarget, Vector3.up*10, Color.green);
        }

        public void TrajectoryShow(Vector3 direction, float power)
        {
            
            if (power == 0 || GameService.CubsDriver.NewCube == null)
            {
                trajectoryRenderer.DropState();
                _trajectoryInScreen = false;
                //_santa.PlayDropReadyMove();
                return;
            }
            if (_bullet == null)
            {
                _bullet = GameService.CubsDriver.NewCube.GetComponent<Rigidbody>();
            }

            if (PhysicThrow)
            {
                CalculateThrow(direction, power);
                trajectoryRenderer.ShowTrajectory(GunSpawnPoint.transform.position, _speed, _bullet);

                if (!_trajectoryInScreen)
                {
                    _santa.PlayReadyMove();
                    _trajectoryInScreen = true;
                }
                
            }
            else
            {
                CalculateNonPhysicsThrow(direction, power);
            }
            
        }

 
        private void FixedUpdate()
        {
            if (_needToShoot)
            {
                _needToShoot = false;                
                _bullet.velocity = Vector3.zero;
                if (PhysicThrow)
                {
                    _bullet.AddForceAtPosition(_speed, transform.position + new Vector3(0.005f, -0.005f, -0.005f), ForceMode.Force);
                }
                StartCoroutine(ShootPublishingDelayed());
                SoundService.PlaySound(Sound.SantaThrow);
            }
        }

        IEnumerator ShootPublishingDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            EventPublisher.onShoot.Publish(_bullet.gameObject, 0);
            _bullet = null;
        }
        public void TakeAShot(Vector3 direction, float power)
        {



            if (GameService.CubsDriver.NewCube!=null)
            {

                if (power == 0)
                {
                    trajectoryRenderer.DropState();
                    _trajectoryInScreen = false;
                    _santa.PlayDropReadyMove();
                    return;
                }

                GameObject CubeBullet = GameService.CubsDriver.NewCube;
                if (CubeBullet != null)
                {
                    _santa.ReturnForThrow(GameService.Instance.GameElements.transform);
                   
                    CubeBullet.transform.position = GunSpawnPoint.transform.position;
                    CubeBullet.transform.rotation = Quaternion.identity;
                    _bullet = CubeBullet.GetComponent<Rigidbody>();
                    _bullet.constraints = RigidbodyConstraints.None;
                    if (PhysicThrow)
                    {
                        CalculateThrow(direction, power);
                        //_bullet.mass = 3.5f;
                    }
                    
                    _needToShoot = true;
                    _santa.PlayThrow();
                }


               
            }
            trajectoryRenderer.DropState();
            _trajectoryInScreen = false;
           

        }


        

        private void CalculateThrow(Vector3 direction, float power)
        {
            power = PowerNormalize(power);

            _newDir = Vector3.Lerp(leftBoundVector, rightBoundVector, 1 - ((direction.normalized.x + 1) / 2));

            _throwDirection = (_newDir + new Vector3(0, yCorrection, 0)).normalized;

            float PowerMin = _lenghtPowerBounds.Min * GunPower;
            float PowerMax = _lenghtPowerBounds.Max * GunPower;

            float CameraCorrection = Mathf.Lerp(PowerMin, PowerMax,
                                                (Mathf.Clamp(Mathf.Abs(direction.normalized.y), 0.35f, 0.9f)-0.35f
                                                )/0.55f
                                                );



            float MaxBound = Mathf.Min(PowerMax, CameraCorrection);


            float JoystikPowerProjection = Mathf.Lerp(PowerMin, MaxBound, power / 100);

            _speed = _throwDirection * JoystikPowerProjection;
        }

        private void CalculateNonPhysicsThrow(Vector3 direction, float power)
        {
            power = Mathf.Clamp(power, 10f, 100f);

            Vector3 HorizontalDirection = new Vector3(
                x: -direction.x,
                y: 0,
                z: -direction.y
                ).normalized;
            float Lenght = Mathf.Lerp(0, NonPhysics_powerBounds.Max , power / 100);

            _speed = HorizontalDirection * Lenght;
            _throwTarget = (transform.position + HorizontalDirection * Lenght).SetYTo(Floor.transform.position.y);

        }

        private float PowerNormalize(float power)
        {
            return Mathf.Clamp(power,5f,100f);
        }

    }

}
