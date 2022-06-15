using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;




namespace VOrb
{
    public class CutScript : MonoBehaviour
    {
        private int _cutCount = 0 ;
        private GameObject _selected;
        private GameObject _deleted;
        private bool _сanCollide = true;
        private bool _isUpCut;


        private Vector3 _ropeVelocity;
        private Vector3 _avatarVelocity;
        private float _beginY;
        private bool _needUpdVelocity = false;
        private Rigidbody _cutRig;
        private Rigidbody _newRig;
        private Vector3 _ropOrientation = new Vector3(0.5f, 0f, -0.8f);

        public bool CanCutTheRig => _сanCollide;
        public bool CutIsUp => _isUpCut;

        private void Start()
        {
            GameService.Instance.Jumpie_anim = transform.parent.parent.parent.GetComponent<Animator>();
            _beginY = transform.position.y;
            _cutCount = 0;
        }

        private void FixedUpdate()
        {
            if (_needUpdVelocity)
            {
                if (!_isUpCut)
                {
                    _cutRig.mass = 2;
                    _newRig.velocity += new Vector3(0, 10, 0);
                    _cutRig.velocity = _ropOrientation * 30;
                }
                else
                {
                    _cutRig.mass = 0.1f;
                    _cutRig.velocity = _ropeVelocity * 5;
                    _newRig.velocity = _avatarVelocity;
                }
                _needUpdVelocity = false;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if ((UIWindowsManager.ActiveNow is MainWindow) )//&& GameService.Instance.GameStarted)
            {
                GameObject PlayerAvatar = GameService.Instance.GetAvatarObj();
                Material mat = GameService.Instance.SliceMaterial;
                PhysicMaterial PhysicMat = GameService.Instance.Current_physicMaterial;


                if (PlayerAvatar != null && _сanCollide)
                {
                    if (other.gameObject.name == PlayerAvatar.name)
                    {
                        
                        _isUpCut = (transform.position.y > _beginY - 10);

                        _avatarVelocity = other.gameObject.GetComponent<Rigidbody>().velocity;
                        _ropeVelocity = transform.forward * 3;

                        SlicedHull hull = PlayerAvatar.Slice(transform.position, transform.up, mat);
                        if (hull != null)
                        {
                            AudioServer.PlaySound(Sound.Blood);
                            _сanCollide = false;
                            GameObject cutupobj = hull.CreateUpperHull(PlayerAvatar, mat, PlayerAvatar.transform.parent);
                            GameObject cutlowobj = hull.CreateLowerHull(PlayerAvatar, mat, PlayerAvatar.transform.parent);
                            MeshCollider _newMesh, _newMesh2;

                            _newMesh = cutupobj.AddComponent<MeshCollider>();
                            _newMesh2 = cutlowobj.AddComponent<MeshCollider>();
                            try
                            {
                                _newMesh.convex = true;
                                _newMesh2.convex = true;
                            }
                            catch 
                            {
                               
                                Destroy(cutupobj);
                                Destroy(cutlowobj);
                                _сanCollide = true;
                                return;                                                           
                            }
                                                        
                            float boup = cutupobj.GetComponent<MeshFilter>().sharedMesh.VolumeOfMesh();
                            float bolow = cutlowobj.GetComponent<MeshFilter>().sharedMesh.VolumeOfMesh();
                            float health;
                            _cutCount++;
                            if (boup > bolow)
                            {
                                _selected = cutupobj;
                                _deleted = cutlowobj;
                                health = Mathf.InverseLerp(SceneLoader.sceneSettings.SizeLim, 100, (boup / GameService.Instance.FullSizeVolume) * 100)*100;
                                MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
                                mn.HealthBar.SetProgress(health);
                                if (boup-(boup*0.1f) <= GameService.Instance.FullSizeVolume*(SceneLoader.sceneSettings.SizeLim/100))
                                {
                                    StartDie();
                                }
                            }
                            else
                            {
                                _selected = cutlowobj;
                                _deleted = cutupobj;
                                _newMesh = _newMesh2;
                                health = Mathf.InverseLerp(SceneLoader.sceneSettings.SizeLim, 100, (bolow / GameService.Instance.FullSizeVolume) * 100) * 100;
                                MainWindow mn = UIWindowsManager.GetWindow<MainWindow>();
                                mn.HealthBar.SetProgress(health);
                                if (bolow-(bolow*0.1f) <= GameService.Instance.FullSizeVolume * (SceneLoader.sceneSettings.SizeLim / 100))
                                {
                                    StartDie();
                                }
                            }
                            _deleted.tag = "part";
                            _cutRig = _deleted.AddComponent<Rigidbody>();

                            _newRig = _selected.GetOrAddComponent<Rigidbody>();


                            _newRig.constraints |= RigidbodyConstraints.FreezePositionX;
                            _newRig.constraints |= RigidbodyConstraints.FreezePositionZ;
                            if (GameService.Instance.PlayerState.EasyMode)
                            {
                                _newRig.constraints |= RigidbodyConstraints.FreezeRotationX;
                                //newRig.constraints |= RigidbodyConstraints.FreezeRotationY;
                                _newRig.constraints |= RigidbodyConstraints.FreezeRotationZ;
                            }
                            

                            _newRig.angularDrag = 4f;

                            _selected.AddComponent<Animator>();

                            BounceControl sel_controller = _selected.AddComponent<BounceControl>();

                            _newMesh.material = PhysicMat;
                            if (GameService.Instance.AvatarControl.InFall)
                            {
                                _newMesh.material.bounciness = 0.4f;
                            }
                            _selected.name = PlayerAvatar.name;


                            Destroy(PlayerAvatar);
                            Destroy(_deleted, SceneLoader.sceneSettings.DestroyTime);

                            GameService.Instance.UpdateAvatarObject(_selected);
                            SetUncuttedForTime(SceneLoader.sceneSettings.DelayTime);
                            _needUpdVelocity = true;
                            GameService.Instance.AvatarControl.NeedUpdateBloodPosition = true;
                         
                        }
                    }
                }
            }
        }

        private void StartDie()
        {
            if (_cutCount == 6)
            {
                Debug.Log("6 CUTS!");
                //ДОСТИЖЕНИЕ  умри за 6 порезов
                if (GameService.Instance.PlayServicesConnected)
                {
                   
                }

            }

            _cutCount = 0;
            GameService.Instance.GameSessionEnd(0);
        }

        public void SetUncuttedForTime(float tm)
        {
            StartCoroutine(SetCollideOnTimer(tm));
        }
        public void MakeCuttedAgain()
        {
            GameObject PlayerAvatar = GameService.Instance.GetAvatarObj();
            StopAllCoroutines();
            _сanCollide = true;
            if (PlayerAvatar != null)
            {
                PlayerAvatar.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
            }
        }
       
            private IEnumerator SetCollideOnTimer ( float tm)
        {
            GameObject PlayerAvatar = GameService.Instance.GetAvatarObj();
            _сanCollide = false;  // при вызове извне
            //float timer = 0f;
            //bool TimeExpired = false;
           // PlayerAvatar.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
            yield return new WaitForSeconds(tm);
            //while (!TimeExpired)
            //{

            //    yield return new WaitForEndOfFrame();
            //    timer += Time.deltaTime;
            //    TimeExpired = (timer>=tm);
            //}
            _сanCollide = true;
            //if (PlayerAvatar!=null)
            //{
            //    PlayerAvatar.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.white);
            //}
            GameService.Instance.AvatarControl.ShieldEffect.SetActive(false);

        }

    }

}