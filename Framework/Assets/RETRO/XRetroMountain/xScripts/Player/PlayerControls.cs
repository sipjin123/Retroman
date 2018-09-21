using UnityEngine;
using System.Collections;
using Common.Utils;
using UniRx;
namespace Retroman
{
    public class PlayerControls : MonoBehaviour {
        #region VARIABLES
        public enum PlayerType
        {
            NORMAL,
            CAT,
            DONKEYKONG,
            SONIC,
            UNICORN,
            YOSHI,
        }
        public PlayerType _playerType;
        public enum PlayerAction
        {
            TURNRIGHT,
            TURNLEFT,
            JUMP,
            FORWARD,
            FALL
        }
        public PlayerAction _playerAction;

        //RAYCAST 
        public bool isGrounded;
        public GameObject RayObject;
        public RaycastHit Rayhit;


        //MOVEMENT
        public GameObject myMesh;
        public float movementSpeed;
        public float player_rotation;
        public float rotatedValue;

        public GameObject lerpToThisObject;

        //JUMP
        public bool isJumping;
        float rotateSpeed = 10;

        //EFFECTS
        public GameObject _shadowObject;
        public GameObject _deathAnim;
        public GameObject _splash;
        public GameObject _bodySpin;

        public bool _activePlayerObject;

        Rigidbody _rigidbody;
        //SPECIALS
        public bool _doubleJumpConsumed;
        float _walkCounter;


        bool _jumpDelaySwitch = false;

        [SerializeField]
        private LayerMask GroundWaterMask, WaterMask,GroundMask, FallStopperMask;

        const string GroundMaskID = "GroundOnly";
        const string WaterMaskID = "WaterOnly";
        const string FallStopperMaskID = "FallStopperMask";

        [SerializeField]
        private CameraControls _CameraControls;

        int WaterMaskLayerIndex = 0;
        int GroundMaskLayerIndex = 0;
        int FallStopperMaskLayerIndex = 0;


        public Transform VFXJumpSpawn;
        public GameObject RunningVFX;
        public GhostAnimator gA;
        #endregion
        //==========================================================================================================================================
        #region INITIALIZATION
        void Awake()
        {
            WaterMaskLayerIndex = LayerMask.NameToLayer(WaterMaskID);
            GroundMaskLayerIndex = LayerMask.NameToLayer(GroundMaskID);
            FallStopperMaskLayerIndex = LayerMask.NameToLayer(FallStopperMaskID);
        }
        public void Start()
        {
               isGrounded = true;
            _activePlayerObject = false;


            //TRANSFORM AND ROTATION RELATED
            movementSpeed = 0.15f;
            rotatedValue = 0;
            player_rotation = 0;

            //PHYSICS RELATED
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;

            _playerAction = PlayerAction.FORWARD;
            isJumping = false;
            _doubleJumpConsumed = false;

            Time.timeScale = 1.5f;

            //COSTUME
            _bodySpin.transform.GetChild((int)_playerType).gameObject.SetActive(true);
            _deathAnim.transform.GetChild(0).GetChild((int)_playerType).gameObject.SetActive(true);

            int currentChar = Factory.Get<DataManagerService>().GetCurrentCharacter();

            currentChar -= 1;
            if (currentChar <= -1)
                currentChar = 0;
            SetupPlayerType( currentChar );
        }
        public void SetupPlayerType(int _playaTyp)
        {
            _playerType = (PlayerType)_playaTyp;
            for (int i = 0; i < _bodySpin.transform.childCount; i++)
            {
                _bodySpin.transform.GetChild(i).gameObject.SetActive(false);
                _deathAnim.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
            //PlayerPrefs.SetInt(DataManagerService.CurrentCharacterSelected_Key, (int)_playerType);

            _bodySpin.transform.GetChild((int)_playerType).gameObject.SetActive(true);
            _deathAnim.transform.GetChild(0).GetChild((int)_playerType).gameObject.SetActive(true);
        }
        #endregion
        //==========================================================================================================================================
        void FixedUpdate()
        {
            if (Factory.Get<DataManagerService>().IFTestMode)
            {
                transform.position = new Vector3(transform.position.x, 7, transform.position.z);
            }
            RaycastFunction();
            if (!_activePlayerObject)
            {
                transform.localPosition = Vector3.zero;
                return;
            }


            if (_playerAction == PlayerAction.FORWARD)
            {
                transform.position += (transform.forward * movementSpeed);
                _walkCounter += movementSpeed;
                if (_walkCounter > 1)
                {
                    Factory.Get<DataManagerService>().MessageBroker.Publish(new SpawnAPlatform());
                    _walkCounter--;
                }
            }
            //JUMP START
            if (!isJumping && isGrounded)
            {
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
                {
                    _jumpDelaySwitch = true;
                    StartCoroutine(JumpDelayENUM());
                    _rigidbody.AddForce(transform.up * (15000/25));//15000);
                   Factory.Get<VFXHandler>().RequestVFX(VFXJumpSpawn.position, VFXHandler.VFXList.JumpUpVFX);
                    isJumping = true;

                    SoundControls.Instance._sfxJump.Play();
                    return;

                }
            }
            else //JUMP SPECIAL
            {
                if (isJumping)
                {

                    if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)))
                    {
                        _rigidbody.AddForce(transform.up * (650/25));// 650);
                    }
                }
            }
            _rigidbody.AddForce(-transform.up * (1000/25)); //1000);

            PlayerTurnFunction();
          //  RaycastFunction();

        }
        //==========================================================================================================================================	
        #region COROUTINES
        IEnumerator JumpDelayENUM()
        {
            yield return new WaitForSeconds(0.25f);
            _jumpDelaySwitch = false;
        }
        #endregion
        //==========================================================================================================================================
      

        #region RAYCAST
        void RaycastFunction()
        {
            Debug.DrawRay(RayObject.transform.position, -RayObject.transform.up * 10,Color.blue);

            if (Physics.Raycast(RayObject.transform.position, -RayObject.transform.up * 10, out Rayhit))// ,GroundWaterMask))
            {
                //Debug.LogError(Rayhit.collider.gameObject.name+ " : " +LayerMask.LayerToName(Rayhit.collider.gameObject.layer));


                if ((Rayhit.collider.gameObject.layer) == GroundMaskLayerIndex )
                {
                  
                    _shadowObject.transform.position = new Vector3(_shadowObject.transform.position.x, Rayhit.point.y, _shadowObject.transform.position.z);
                }

                if (isJumping)
                {
                  
                    if (Rayhit.collider.gameObject.layer == FallStopperMaskLayerIndex)
                    {
                        _shadowObject.SetActive(false);
                    }
                    else if (Rayhit.collider.gameObject.layer == GroundMaskLayerIndex)
                    {
                        _shadowObject.SetActive(true);
                    }
                    else
                    {
                        _shadowObject.SetActive(false);
                    }

                    //RunningVFX.SetActive(false);
                    ParticleSystem ps = RunningVFX.GetComponent<ParticleSystem>();
                    var emission = ps.emission;
                    emission.rateOverTime = 0;
                 
                }
                else
                {

                    //RunningVFX.SetActive(true);
                    ParticleSystem ps = RunningVFX.GetComponent<ParticleSystem>();
                    var emission = ps.emission;
                    emission.rateOverTime = 10;
                  
                }

            }
            else
            {
                //Debug.LogError("No hit");
                _shadowObject.SetActive(false);
            }
        }
        #endregion
        //==========================================================================================================================================
        #region FUNCTIONS

        private enum CurrDirection
        {
            Left,
            Right
        }
        
        CurrDirection cD = CurrDirection.Right;
            
        void PlayerTurnFunction()
        {
            if (_playerAction == PlayerAction.TURNLEFT)
            {
                cD = CurrDirection.Right;
                player_rotation -= rotateSpeed;
                rotatedValue += rotateSpeed;
                if (rotatedValue >= 90)
                {
                    _playerAction = PlayerAction.FORWARD;
                    rotatedValue = 0;
                    //CameraControls.._ResetToDirection(180);
                }
                transform.eulerAngles = new Vector3(0, player_rotation, 0);
                LerpToPosition(lerpToThisObject, lerpToThisObject.transform.position, 0);
            }
            else if (_playerAction == PlayerAction.TURNRIGHT)
            {
                cD = CurrDirection.Left;
                player_rotation += rotateSpeed;
                rotatedValue += rotateSpeed;
                if (rotatedValue >= 90)
                {
                    _playerAction = PlayerAction.FORWARD;
                    rotatedValue = 0;
                    //CameraControls.._ResetToDirection(0);
                }
                transform.eulerAngles = new Vector3(0, player_rotation, 0);
                LerpToPosition(lerpToThisObject, lerpToThisObject.transform.position, 0);
            }
        }
        void LerpToPosition(GameObject _targetPosition, Vector3 StartPos, float _targetTime)
        {
            _targetTime += 0.01f;
            transform.position = Vector3.Lerp(new Vector3(StartPos.x, transform.position.y, StartPos.z), new Vector3(_targetPosition.transform.position.x, transform.position.y, _targetPosition.transform.position.z), _targetTime);
            if (_targetTime < 1)
                LerpToPosition(_targetPosition, StartPos, _targetTime);
        }
        #endregion
        //==========================================================================================================================================
        #region COLLISIONS

        private void OnTriggerStay(Collider hit)
        {
            if (hit.gameObject.name == "Left"
           || hit.gameObject.name == "Right"
           || hit.gameObject.name == "MidLeft"
           || hit.gameObject.name == "MidRight")
            {
                lerpToThisObject = hit.gameObject;
                if (hit.gameObject.name == "Left")
                    _playerAction = PlayerAction.TURNLEFT;
                if (hit.gameObject.name == "Right")
                    _playerAction = PlayerAction.TURNRIGHT;
                if (hit.gameObject.name == "MidRight")
                {
                    Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeCamAngle { Angle = 0 });
                }
                if (hit.gameObject.name == "MidLeft")
                {
                    Factory.Get<DataManagerService>().MessageBroker.Publish(new ChangeCamAngle { Angle = 180 });
                }
                hit.GetComponent<MeshRenderer>().material.color = Color.black;
                hit.GetComponent<BoxCollider>().enabled = (false);
            }
        }

        void OnTriggerEnter(Collider hit)
        {if (hit.gameObject.name == "FallStopper")
            {
                _playerAction = PlayerAction.FALL;
            }
            //JUMPING
            if (!_jumpDelaySwitch)
            {
                if(hit.GetComponent<PlatformMinion>() != null)//if (LayerMask.LayerToName( hit.gameObject.layer) == "GroundOnly")
                {
                    isGrounded = true;
                    if (isJumping)
                    {
                        _rigidbody.velocity = new Vector3(0, 0, 0);
                         Factory.Get<VFXHandler>().RequestVFX(VFXJumpSpawn.position, VFXHandler.VFXList.JumpVFX);

                    }
                    isJumping = false;
                }
            }
        }
        #endregion
        //==========================================================================================================================================
        #region SIGNALS
        void OnEnable()
        {

            


            Factory.Get<DataManagerService>().MessageBroker.Receive<SetupPlayerSplash>().Subscribe(_ =>
            {
                Debug.LogError("WATER DEATH PART 2");
               //   
                _splash.SetActive(_.IfActive);
                _splash.transform.position =  _deathAnim.transform.GetChild(0).transform.position;
                if (cD  == CurrDirection.Right )
                    gA.StartAnimation(true);
                else
                    gA.StartAnimation(false);
               
            }).AddTo(this);


            Factory.Get<DataManagerService>().MessageBroker.Receive<EnablePlayerShadows>().Subscribe(_ =>
            {

                _shadowObject.SetActive(_.IfActive);
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<ActivePlayerObject>().Subscribe(_ =>
            {
                _activePlayerObject = _.IfActive;
            }).AddTo(this);
            

            Factory.Get<DataManagerService>().MessageBroker.Receive<EnablePlayerControls>().Subscribe(_ =>
            {

                GetComponent<PlayerControls>().enabled = _.IfACtive;
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<UpdatePlayerAction>().Subscribe(_ =>
            {

                _playerAction = _.PlayerAction;
            }).AddTo(this);

            Factory.Get<DataManagerService>().MessageBroker.Receive<LaunchGamePlay>().Subscribe(_ =>
            {
               

            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<DisablePlayableCharacter>().Subscribe(_ =>
            {

                transform.GetChild(0).gameObject.SetActive(false);

            }).AddTo(this);
            Factory.Get<DataManagerService>().MessageBroker.Receive<EnableRagdoll>().Subscribe(_ =>
            {
                _deathAnim.SetActive(true);
               if (cD  == CurrDirection.Right )
                    gA.StartAnimation(true);
                else
                    gA.StartAnimation(false);
               
            }).AddTo(this);
            


            Factory.Get<DataManagerService>().MessageBroker.Receive<PauseGame>().Subscribe(_ =>
            {
                if (_.IfPause)
                {
                    OnGamePause();
                }
                else
                {
                    OnGameResume();
                }
            }).AddTo(this);
        }

        void OnGamePause( )
        {
            _activePlayerObject = false;
            _rigidbody.useGravity = false;
        }

        void OnGameResume( )
        {
            _shadowObject.SetActive(true);
            _activePlayerObject = true;
            _rigidbody.useGravity = true;
            _CameraControls._startFollow = true;
        }
        
        #endregion
    }
}