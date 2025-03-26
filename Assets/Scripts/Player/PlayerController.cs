using System;
using System.Collections;
using System.Threading;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using NUnit.Framework.Interfaces;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public GameManager gameManager;
    private GameObject _camera;
    private GameObject _cinemachineCamera;
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    public GameObject tongue;
    private FrogTongue _frogTongue;

    private Vector3 lookDir;
    private Vector3 _lastVelocity;
    private bool _canJump = true;
    private bool _isJumpCancelled;
    private bool _isGrounded = true;
    private float _jumpCharge;

    // float and bool for storing jump data if we jumped some frames before we land
    private Coroutine _leniencyCoroutine;
    private float _lastCharge;
    private bool _jumpToken;

    [Tooltip("Percentage of jump height added on start")][SerializeField][Range(0, 1)] private float _startingHeight;
    [Tooltip("Percentage of jump force added on start")][SerializeField][Range(0, 1)] private float _startingForce;
    [SerializeField][Range(0, 200)] private int _jumpHeight;
    [SerializeField][Range(0, 200)] private int _jumpForce;
    [SerializeField][Range(0, 0.1f)] private float _leniencyJumpDuration;

    #region Setup
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        _camera = transform.GetChild(1).gameObject;
        _cinemachineCamera = transform.GetChild(2).gameObject;
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _frogTongue = tongue.GetComponent<FrogTongue>();

        _camera.SetActive(false);
    }

    void OnEnable()
    {
        _rigidbody.isKinematic = false;
        InputAction playerJump = InputSystem.actions.FindAction("Jump");
        playerJump.started += JumpCharge;
        playerJump.canceled += InputJump;
        playerJump.performed += JumpCancel;
        InputAction playerGrip = InputSystem.actions.FindAction("Grip");
        playerGrip.started += GripOnGround;
        InputAction playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        playerToggleUI.performed += Pause;
        InputAction playerRestart = InputSystem.actions.FindAction("Restart");
        playerRestart.performed += Restart;
        _camera.SetActive(true);
        _cinemachineCamera.SetActive(true);
    }

    void OnDisable()
    {
        _rigidbody.isKinematic = true;
        InputAction playerJump = InputSystem.actions.FindAction("Jump");
        playerJump.started -= JumpCharge;
        playerJump.canceled -= InputJump;
        playerJump.performed -= JumpCancel;
        InputAction playerGrip = InputSystem.actions.FindAction("Grip");
        playerGrip.started -= GripOnGround;
        InputAction playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        playerToggleUI.performed -= Pause;
        InputAction playerRestart = InputSystem.actions.FindAction("Restart");
        playerRestart.performed -= Restart;
        _cinemachineCamera.SetActive(false);
    }

    public void DisabeCamera()
    {
        _camera.SetActive(false);
    }

    public void GameManagerHook()
    {
        gameManager.ue_sceneReset.AddListener(OnReset);
        OnReset();
    }

    private void OnReset()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        transform.position = gameManager.start.position + new Vector3(0, 0.3f, 0);
        _rigidbody.MovePosition(gameManager.start.position + new Vector3(0, 0.3f, 0));
        transform.eulerAngles = new Vector3(0, gameManager.startRotation, 0);
        _rigidbody.MoveRotation(Quaternion.Euler(0, gameManager.startRotation, 0));
        _jumpCharge = 0;
        _canJump = true;
        _isGrounded = true;
        _jumpToken = false;
        _cinemachineCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value = transform.eulerAngles.y;
        _cinemachineCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = 10;
        _rigidbody.linearVelocity = Vector3.zero;
        _cinemachineCamera.GetComponent<CinemachineInputAxisController>();
        _animator.Play("Idle", 0, 0);
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }
    #endregion

    private void GripOnGround(InputAction.CallbackContext context)
    {
        if (_isGrounded && _rigidbody.linearVelocity != Vector3.zero)
            _rigidbody.linearVelocity = Vector3.zero;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        GameManager.instance.PauseToggle(PauseSetting.Toggle, true);
    }

    private void Restart(InputAction.CallbackContext context)
    {
        gameManager.RestartLevel();
        gameManager.PauseToggle(PauseSetting.Pause, false);
    }

    #region Jumping
    private void JumpCharge(InputAction.CallbackContext context) // Holding down jump button
    {
        _isJumpCancelled = false;
    }

    private void InputJump(InputAction.CallbackContext context) // Released jump button when charging jump
    {
        if (_canJump && _isJumpCancelled == false && !gameManager.isGamePaused && !_jumpToken)
        {
            Jump(true);
        }
        else
        {
            _lastCharge = _jumpCharge;
            _jumpCharge = 0;
            if (!_isGrounded)
            {
                if (_leniencyCoroutine != null)
                    StopCoroutine(_leniencyCoroutine);
                _leniencyCoroutine = StartCoroutine(LeniencyJump());
            }
        }
    }

    private void Jump(bool input)
    {
        if (_canJump)
        {
            _canJump = false;
            StartCoroutine(GroundUpdate());
            lookDir = (transform.position - new Vector3(_cinemachineCamera.transform.position.x, transform.position.y, _cinemachineCamera.transform.position.z)).normalized;

            float usedCharge;
            if (input)
                usedCharge = _jumpCharge;
            else
                usedCharge = _lastCharge;

            float heightStrength = usedCharge + _startingHeight;
            float forceStrength = usedCharge + _startingForce;
            if (heightStrength > 1) heightStrength = 1;
            if (forceStrength > 1) forceStrength = 1;
            _rigidbody.AddForce(new Vector3(lookDir.x * _jumpForce * forceStrength, _jumpHeight * heightStrength, lookDir.z * _jumpForce * forceStrength), ForceMode.Impulse);

            _jumpCharge = 0;
            _rigidbody.rotation = Quaternion.LookRotation(lookDir, transform.up);
            _animator.Play("Jump", -1, 0f);
        }
    }

    private IEnumerator GroundUpdate()
    {
        yield return new WaitForFixedUpdate();
        _isGrounded = false;
    }

    private IEnumerator LeniencyJump()
    {
        _jumpToken = true;
        yield return new WaitForSeconds(_leniencyJumpDuration);
        _jumpToken = false;
    }

    private void JumpCancel(InputAction.CallbackContext context) // Waited too long to jump
    {
        _isJumpCancelled = true;
        _jumpCharge = 0;
    }
    #endregion

    #region Updates
    void Update()
    {
        // Charge the jump if the jump button is being held
        if (InputSystem.actions.FindAction("Jump").phase == InputActionPhase.Started && _jumpCharge < 1 && !gameManager.isGamePaused)
        {
            _jumpCharge += 1 * Time.deltaTime;
            if (_jumpCharge > 1) _jumpCharge = 1;
        }
    }

    void FixedUpdate()
    {
        if (_rigidbody.position.y <= gameManager?.killDepth)
        {
            gameManager.RestartLevel();
            gameManager.PauseToggle(PauseSetting.Pause, false);
            return;
        }

        _lastVelocity = _rigidbody.linearVelocity;

        GroundCheck();

        if (!_isGrounded)
            _rigidbody.AddForce(Vector3.down * 30);
    }

    private void GroundCheck()
    {
        bool foundGround = false;

        Vector3 worldCenter = transform.position + _boxCollider.center;
        Vector3 halfExtends = _boxCollider.size / 2;

        RaycastHit[] hit = Physics.BoxCastAll(worldCenter, halfExtends, Vector3.down, transform.rotation, 0.04f);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.CompareTag("Ground"))
                {
                    foundGround = true;
                    break;
                }
            }
        }

        if (foundGround != _isGrounded)
        {
            _isGrounded = foundGround;
            if (_isGrounded) Land();
        }
    }

    private void Land()
    {
        _canJump = true;
        if (InputSystem.actions.FindAction("Grip").phase != InputActionPhase.Waiting)
            _rigidbody.linearVelocity = Vector3.zero;
        else
            _rigidbody.linearVelocity = new Vector3(_lastVelocity.x, 0, _lastVelocity.z) / 2;


        if (_jumpToken)
        {
            Jump(false);
        }
        else _animator.Play("Land", -1, 0f);
    }

    #endregion

    #region DELETEONPROJECTEND
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(_boxCollider.bounds.center - new Vector3(0, 0.2f, 0), _boxCollider.bounds.size);
    }
    #endregion

    private void ShootTongue()
    {

    }

    private void GetDisowned()
    {
        transform.SetParent(null);
    }

}