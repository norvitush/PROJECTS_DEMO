using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VOrb;

public class AvatarController : MonoBehaviour, ITouchSensetive
{
    
    [SerializeField] [Range(45, 100)] private float powerOfTap = 45f;

    private Vector2 _startFallCoord = Vector2.zero;
    private bool _needJump = false;
    private bool _needSpeedDown = false;
    private bool _needStopFall = false;
    private bool _inFall = false;
    private bool _inGround = false;
    
    private bool _needUpdateBloodPosition;
    private int _playerJumpRemains;
    private Rigidbody _rig;
    private readonly List<Rigidbody> _allInAvatarContainer = new List<Rigidbody>();

    public MainWindow parentWindow;
    public UnityEvent TapEvent { get; set; }
    public UnityEvent SvipeEvent { get; set; }
    public bool InFall { get => _inFall; set => _inFall = value; }
    public bool InGround { get => _inGround; set => _inGround = value; }
    public bool NeedUpdateBloodPosition { get => _needUpdateBloodPosition; set => _needUpdateBloodPosition = value; }
    public int PlayerJumpRemains { get => _playerJumpRemains; set => _playerJumpRemains = value; }
    public Rigidbody Rig { get => _rig; set => _rig = value; }
    public GameObject ShieldEffect;
    public CutScript Cutter;
    public GameObject BloodEffect;

    private void Start()
    {
        // PlayerJumpRemains = SceneLoader.sceneSettings.JumpLimit;
        TapEvent = new UnityEvent();
        SvipeEvent = new UnityEvent();
        TapEvent.AddListener(WhenUserTap);          
        SvipeEvent.AddListener(WhenUserSvipe);
        ShieldEffect.SetActive(false);
        NeedUpdateBloodPosition = false;
    }
    private void FixedUpdate()
    {
        if (GameService.Instance.AvatarControl!=null)
        {
            if (!GameService.Instance.AvatarControl.Cutter.CanCutTheRig)
            {
                Vector3 self = ShieldEffect.transform.position;
                
                ShieldEffect.transform.position = new Vector3(self.x, Rig.worldCenterOfMass.y-3f, self.z); 
            }
            if (NeedUpdateBloodPosition)
            {
                float ang = Cutter.CutIsUp ? 0 : 180;
                BloodEffect.transform.position = Cutter.gameObject.transform.position + new Vector3(0, 0, Cutter.CutIsUp ? 0 : -10f);
                
                BloodEffect.transform.localEulerAngles = new Vector3(ang,0,0);

                NeedUpdateBloodPosition = false;
                BloodEffect.SetActive(true);
                StartCoroutine(nameof(DeactivateBlood));
                
            }
        }
        if (InFall && _needStopFall)
        {
            InFall = false;
            _needStopFall = false;
            _needJump = false;

            Camera.main.gameObject.GetComponent<Animator>().Play("Cam_shake", -1, 0);
            UIWindowsManager.GetWindow<MainWindow>().StampEffect.gameObject.SetActive(true);
            if (_startFallCoord.y > GameService.Instance.yTopCoordForBonus)
            {
                AudioServer.PlaySound(Sound.PullUp, 0.8f);
                GameService.Instance.itemGenerator.PullUpCoin();
            }
                        
        }


        if (_needSpeedDown)
        {
            UIWindowsManager.GetWindow<MainWindow>().StampEffect.gameObject.SetActive(false);
            AudioServer.PlaySound(Sound.Svipe,0.4f);
            Fall();
            _needJump = false;
        }

        if (_needJump)
        {
            AudioServer.PlaySound(Sound.Tap);
            Jump();            
        }
        

    }

    private IEnumerator DeactivateBlood()
    {
        yield return new WaitForSeconds(0.8f);

        BloodEffect.SetActive(false);
    }

    public void WhenUserTap()
    {
        
        if (PlayerJumpRemains > 0 && !_needJump && !InFall && !_needSpeedDown)
        {
            _needJump = true;
            PlayerJumpRemains--;
        }

    }
    public void WhenUserSvipe()
    {


        if ( !_needJump && !InFall && !_needSpeedDown)
        {
            Rig.GetComponent<MeshCollider>().material.bounciness = 0.4f;
            _needSpeedDown = true;
            // rig.GetComponent<MeshCollider>().material = null;
           
            _startFallCoord = Rig.transform.position;
        }
        
    }

    public void Jump()
    {

            _needJump = false;
        //UIEffects.SplashMainScreen("Tap!");
        Rig?.AddForceAtPosition(new Vector3(0, powerOfTap - Rig.velocity.y, 0), Rig.transform.position, ForceMode.Impulse);

    }

   
    public void Fall()
    {

            _needSpeedDown = false;
            InFall = true;
            //UIEffects.SplashMainScreen("WoW!");
            Rig?.AddForceAtPosition(new Vector3(0, (-powerOfTap * 2) - Rig.velocity.y, 0), Rig.transform.position, ForceMode.Impulse);

    }

    public void FallEnds()
    {
        AudioServer.PlaySound(Sound.FallEnded,0.4f);
        _needStopFall = true;  
    }

    public void MoveRigDown()
    {
       
        StartCoroutine(nameof(MoveUnderFloor));
        
    }
    private IEnumerator MoveUnderFloor()
    {
       
        float startTime = Time.unscaledTime;
        yield return new WaitUntil(()=>InGround||(Time.unscaledTime-startTime)>4);

        _allInAvatarContainer.Clear();
        var AllObjects = GameService.Instance.GetAllIn(GameService.Instance.AvatarContainer);//.
                        // Where(g=> g.GetComponent<BounceControl>()!=null||g.tag=="Ground");
        List<Rigidbody> WithOutMeshoff = new List<Rigidbody>();
        foreach (var obj in AllObjects)
        {
            if (obj != null)
            {              
                var cur_rig = obj.GetComponent<Rigidbody>();
                if (cur_rig!=null)
                {
                    _allInAvatarContainer.Add(cur_rig);
                    if (obj.GetComponent<BounceControl>() != null || obj.tag == "Ground")
                    {
                        cur_rig.gameObject.GetComponent<MeshCollider>().enabled = false;
                        cur_rig.constraints = RigidbodyConstraints.FreezeRotation;
                    }
                    else
                    {
                        WithOutMeshoff.Add(cur_rig);
                    }
                   
                }                               
                
            }
           
        }
        
        float timer = 3f;
        bool inPlaying = false;

        while (timer > 0)
        {
            if (timer<2.5f && !inPlaying)
            {
                AudioServer.PlaySound(Sound.Die);
                inPlaying = true;
            }
            foreach (var cur_rig in _allInAvatarContainer)
            {
                if (cur_rig != null)
                {
                    if ( cur_rig.tag != "part")
                    {
                        if (WithOutMeshoff.Contains(cur_rig) && timer > 2.8f)
                        {
                            cur_rig.GetComponent<MeshCollider>().enabled = false;
                            cur_rig.constraints = RigidbodyConstraints.FreezeRotation;
                            WithOutMeshoff.Remove(cur_rig);
                        }
                        else
                        {
                            if (!WithOutMeshoff.Contains(cur_rig))
                            {
                                cur_rig.transform.position += Vector3.down * (Time.deltaTime / 20);
                                cur_rig.velocity = Vector3.zero;
                            }
                        }
                       
                    }

                    
                }
            }

            yield return null;
            timer -= Time.deltaTime;

        }
        foreach (var cur_rig in _allInAvatarContainer)
        {
            if (cur_rig != null)
            {
                cur_rig.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator WaitNearestShake()
    {
        yield return new WaitUntil(()=>_needSpeedDown||!GameService.Instance.GameStarted);
        if (GameService.Instance.GameStarted)
        {         
            GameService.Instance.SetPlayerPrefs(true, (bool)(SafeInt)(int)DataKeeper.LoadParam("easymode", 1));
            DataKeeper.SaveParam("tutor", 1);
        }
        GameService.Instance.TutorialContainer.SetActive(false);

    }

}


