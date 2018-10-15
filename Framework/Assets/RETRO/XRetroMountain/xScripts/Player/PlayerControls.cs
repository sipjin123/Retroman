﻿using UnityEngine;
using System.Collections;
using Common.Utils;
using UniRx;
using Framework;

namespace Retroman
{
    public class PlayerControls : MonoBehaviour
    {
        #region VARIABLES
        public PlayerType _playerType;
        public PlayerAction _playerAction;

        CurrDirection cD = CurrDirection.Right;
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

        MessageBroker _Broker;

        int WaterMaskLayerIndex = 0;
        int GroundMaskLayerIndex = 0;
        int FallStopperMaskLayerIndex = 0;

        //SIR HECTOR STUFF
        public Transform VFXJumpSpawn;
        public GameObject RunningVFX;
        public GhostAnimator gA;

        
        //CONSTANTS
        const string RIGHT = "Right";
        const string LEFT = "Left";
        const string MID_RIGHT = "MidRight";
        const string MID_LEFT = "MidLeft"; 
        const string FALL_STOPPER = "FallStopper";
        #endregion
        //==========================================================================================================================================
        #region INITIALIZATION
        void Awake()
        {
            WaterMaskLayerIndex = LayerMask.NameToLayer(WaterMaskID);
            GroundMaskLayerIndex = LayerMask.NameToLayer(GroundMaskID);
            FallStopperMaskLayerIndex = LayerMask.NameToLayer(FallStopperMaskID);

            _Broker = Factory.Get<DataManagerService>().MessageBroker;
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

            _Broker.Publish(new PlayerControlSpawned { PlayerControls = this });
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
        bool bugfixFlagJumper;
        void FixedUpdate()
        {
            RaycastHit groundCheckRay;
            Debug.DrawRay(RayObject.transform.position, -RayObject.transform.up * 10, Color.blue);

            if (Physics.Raycast(RayObject.transform.position, -RayObject.transform.up * 10, out groundCheckRay))// ,GroundWaterMask))
            {
                if(groundCheckRay.collider.GetComponent<PlatformMinion>()!=null)
                {
                    bugfixFlagJumper = true;
                }
                else
                {
                    bugfixFlagJumper = false;
                }
            }






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





            /*//AUTOMATE
            //JUMP START
            if (!isJumping && isGrounded)
            {
                if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
                {
                    JumpStart();
                    return;

                }
            }
            else //JUMP SPECIAL
            {
                if (isJumping)
                {

                    if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)))
                    {
                        JumpHold();
                    }
                }
            }*/
            _rigidbody.AddForce(-transform.up * (1000/25)); //1000);

            PlayerTurnFunction();
          //  RaycastFunction();
        }
        #region JUMPING
        public void GenericJump()
        {
            if (!_activePlayerObject)
                return;
            if (isJumping == false)
            {
                if (bugfixFlagJumper == false)
                    return;

                _jumpDelaySwitch = true;
                StartCoroutine(JumpDelayENUM());
                _rigidbody.AddForce(transform.up * (15000 / 25));//15000);
                Factory.Get<VFXHandler>().RequestVFX(VFXJumpSpawn.position, VFXHandler.VFXList.JumpUpVFX);
                isJumping = true;

                SoundControls.Instance._sfxJump.Play();
            }
            else
            {

                _rigidbody.AddForce(transform.up * (650 / 25));// 650);
            }
        }
        public void JumpStart()
        {

            _jumpDelaySwitch = true;
            StartCoroutine(JumpDelayENUM());
            _rigidbody.AddForce(transform.up * (15000 / 25));//15000);
            Factory.Get<VFXHandler>().RequestVFX(VFXJumpSpawn.position, VFXHandler.VFXList.JumpUpVFX);
            isJumping = true;

            SoundControls.Instance._sfxJump.Play();
        }
        public void JumpHold()
        {
            _rigidbody.AddForce(transform.up * (650 / 25));// 650);
        }
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

                const string HOLE_OBJ = "HOLE_OBJ";
                bool ifHittingPlatform = (Rayhit.collider.GetComponent<PlatformMinion>()) != null;
                if (ifHittingPlatform)//GroundMaskLayerIndex )
                {
                  
                    _shadowObject.transform.position = new Vector3(_shadowObject.transform.position.x, Rayhit.point.y, _shadowObject.transform.position.z);
                }

                if (isJumping)
                {
                  
                    if (Rayhit.collider.gameObject.name == HOLE_OBJ)//FallStopperMaskLayerIndex)
                    {
                        _shadowObject.SetActive(false);
                    }
                    else if (ifHittingPlatform)//(Rayhit.collider.gameObject.layer == GroundMaskLayerIndex)
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
            if (hit.gameObject.name == LEFT
           || hit.gameObject.name == RIGHT
           || hit.gameObject.name == MID_LEFT
           || hit.gameObject.name == MID_RIGHT)
            {
                lerpToThisObject = hit.gameObject;
                if (hit.gameObject.name == LEFT)
                {
                    _playerAction = PlayerAction.TURNLEFT;
                }
                if (hit.gameObject.name == RIGHT)
                {
                    _playerAction = PlayerAction.TURNRIGHT;
                }
                if (hit.gameObject.name == MID_RIGHT)
                {
                    _Broker.Publish(new ChangeCamAngle { Angle = 0 });
                }
                if (hit.gameObject.name == MID_LEFT)
                {
                    _Broker.Publish(new ChangeCamAngle { Angle = 180 });
                }
                hit.GetComponent<MeshRenderer>().material.color = Color.black;
                hit.GetComponent<BoxCollider>().enabled = (false);
            }
        }

        void OnTriggerEnter(Collider hit)
        {if (hit.gameObject.name == FALL_STOPPER)
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


            _Broker.Receive<CharJumpSignal>().Subscribe(_ =>
            {
                    GenericJump();
            }).AddTo(this);

            _Broker.Receive<SetupPlayerSplash>().Subscribe(_ =>
            {
                _splash.SetActive(_.IfActive);
                _splash.transform.position =  _deathAnim.transform.GetChild(0).transform.position;
                if (cD  == CurrDirection.Right )
                    gA.StartAnimation(true);
                else
                    gA.StartAnimation(false);
               
            }).AddTo(this);

            _Broker.Receive<EnablePlayerShadows>().Subscribe(_ =>
            {

                _shadowObject.SetActive(_.IfActive);
            }).AddTo(this);
            _Broker.Receive<ActivePlayerObject>().Subscribe(_ =>
            {
                _activePlayerObject = _.IfActive;
            }).AddTo(this);

            _Broker.Receive<EnablePlayerControls>().Subscribe(_ =>
            {

                GetComponent<PlayerControls>().enabled = _.IfACtive;
            }).AddTo(this);
            _Broker.Receive<UpdatePlayerAction>().Subscribe(_ =>
            {

                _playerAction = _.PlayerAction;
            }).AddTo(this);
            _Broker.Receive<LaunchGamePlay>().Subscribe(_ =>
            {
               

            }).AddTo(this);
            _Broker.Receive<DisablePlayableCharacter>().Subscribe(_ =>
            {

                transform.GetChild(0).gameObject.SetActive(false);

            }).AddTo(this);
            _Broker.Receive<EnableRagdoll>().Subscribe(_ =>
            {
                _deathAnim.SetActive(true);
               if (cD  == CurrDirection.Right )
                    gA.StartAnimation(true);
                else
                    gA.StartAnimation(false);
               
            }).AddTo(this);



            _Broker.Receive<PauseGame>().Subscribe(_ =>
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
            Debug.LogError(D.AUTOMATION + "Resumed Game");
        }

        #endregion
    }
    #region ENUM
    public enum CurrDirection
    {
        Left,
        Right
    }

    public enum PlayerType
    {
        NORMAL,
        CAT,
        DONKEYKONG,
        SONIC,
        UNICORN,
        YOSHI,
    }
    public enum PlayerAction
    {
        TURNRIGHT,
        TURNLEFT,
        JUMP,
        FORWARD,
        FALL
    }
    #endregion
}