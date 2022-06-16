using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VOrb.CubesWar
{
    public class ThrowDriver : MonoBehaviour
    {
        public const float BASE_ANGLE_Y = 0.8f;
        public const float BASE_POWER = 56f;
        public const float BASE_ISOMETRIC_KOEF = 2f;

        private bool _trajectoryInScreen = false;
        private Vector3 _throwDirection;
        private Vector3 _speed;
        private Vector3 _newDir;
        private Vector3 _throwTarget;
        private bool _needToShoot = false;
        private Rigidbody _throwObject;

        [Serializable]
        private struct LenghtPowerBounds
        {
            public float Min;
            public float Max;
        }

        [SerializeField] private Vector3 _leftBoundVector = new Vector3(-0.48f, 0f, 0.9f);
        [SerializeField] private Vector3 _rightBoundVector = new Vector3(0.53f, 0f, 0.9f);
        [SerializeField] private LenghtPowerBounds _lenghtPowerBounds;
        [SerializeField] private GameObject _spawnPoint;
        [SerializeField, Range(0, 1000)] private float _power = BASE_POWER;
        [SerializeField, Range(0, 10)] private float _yCorrection = BASE_ANGLE_Y;
        [SerializeField] private SantaController _santa;
        [SerializeField] private TrajectoryRenderer _trajectoryRenderer;
        [SerializeField] private GameObject _ground;
        public SantaController Santa => _santa;
        public Vector3 ThrowTarget => _throwTarget;
        public GameObject SpawnPoint { get => _spawnPoint; }
        public GameObject Ground { get => _ground;  }


        private void OnEnable()
        {            
           EventPublisher.JoystikUp.Subscribe(TakeAShot);
           EventPublisher.JostikMovement.Subscribe(TrajectoryShow);
           EventPublisher.JoysticFirstTap.Subscribe(DropSantaAnimation);
            _throwObject = null;
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
            GameService.GiftsController.SpawnGift();
        }
        
        public void TrajectoryShow(Vector3 direction, float power)
        {
            
            if (power == 0 || GameService.GiftsController.NewGift == null)
            {
                _trajectoryRenderer.DropState();
                _trajectoryInScreen = false;
                return;
            }
            if (_throwObject == null)
            {
                _throwObject = GameService.GiftsController.NewGift.GetComponent<Rigidbody>();
            }

            CalculateThrow(direction, power);
            _trajectoryRenderer.ShowTrajectory(SpawnPoint.transform.position, _speed, _throwObject);

            if (!_trajectoryInScreen)
            {
                _santa.PlayReadyMove();
                _trajectoryInScreen = true;
            }
            
        }

 
        private void FixedUpdate()
        {
            if (_needToShoot)
            {
                _needToShoot = false;                
                _throwObject.velocity = Vector3.zero;
                _throwObject.AddForceAtPosition(_speed, transform.position + new Vector3(0.005f, -0.005f, -0.005f), ForceMode.Force);
                StartCoroutine(ShootPublishingDelayed());
                SoundService.PlaySound(Sound.SantaThrow);
            }
        }

        IEnumerator ShootPublishingDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            EventPublisher.onShoot.Publish(_throwObject.gameObject, 0);
            _throwObject = null;
        }
        public void TakeAShot(Vector3 direction, float power)
        {

            GameObject GiftBullet = GameService.GiftsController.NewGift;

            if (GiftBullet != null)
            {

                if (power == 0)
                {
                    _trajectoryRenderer.DropState();
                    _trajectoryInScreen = false;
                    _santa.PlayDropReadyMove();
                    return;
                }
                _santa.ReleaseForThrow(GameService.Instance.GameElements.transform);
                GiftBullet.transform.position = SpawnPoint.transform.position;
                GiftBullet.transform.rotation = Quaternion.identity;
                _throwObject = GiftBullet.GetComponent<Rigidbody>();
                _throwObject.constraints = RigidbodyConstraints.None;
                CalculateThrow(direction, power);
                _needToShoot = true;
                _santa.PlayThrow();
            }
            _trajectoryRenderer.DropState();
            _trajectoryInScreen = false;
           

        }


        

        private void CalculateThrow(Vector3 direction, float power)
        {
            power = PowerNormalize(power);

            _newDir = Vector3.Lerp(_leftBoundVector, _rightBoundVector, 1 - ((direction.normalized.x + 1) / 2));

            _throwDirection = (_newDir + new Vector3(0, _yCorrection, 0)).normalized;

            float PowerMin = _lenghtPowerBounds.Min * _power;
            float PowerMax = _lenghtPowerBounds.Max * _power;

            float CameraCorrection = Mathf.Lerp(PowerMin, PowerMax,
                                                (Mathf.Clamp(Mathf.Abs(direction.normalized.y), 0.35f, 0.9f)-0.35f
                                                )/0.55f
                                                );



            float MaxBound = Mathf.Min(PowerMax, CameraCorrection);


            float JoystikPowerProjection = Mathf.Lerp(PowerMin, MaxBound, power / 100);

            _speed = _throwDirection * JoystikPowerProjection;
        }

        private float PowerNormalize(float power)
        {
            return Mathf.Clamp(power,5f,100f);
        }

    }

}
