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

    [Header("References")]
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _cinemachineCamera;
    [SerializeField] private GameObject _crosshair;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private FrogTongue _frogTongue;
    [HideInInspector] public GameManager gameManager;

    private Vector3 lookDir;
    private Vector3 _lastVelocity;
    [HideInInspector] public bool canJump = true;
    private bool _isJumpCancelled;
    [HideInInspector] public bool isGrounded = true;
    private float _jumpCharge;

    // float and bool for storing jump data if we jumped some frames before we land
    private Coroutine _leniencyCoroutine;
    private Coroutine _rumbleCoroutine;
    private float _lastCharge;
    private bool _jumpToken;

    // Attack
    public bool canAttack = true;
    public bool swinging;

    [Header("Jump settings")]
    [Tooltip("Percentage of jump height added on start")][SerializeField][Range(0, 1)] private float _startingHeight;
    [Tooltip("Percentage of jump force added on start")][SerializeField][Range(0, 1)] private float _startingForce;
    [SerializeField][Range(0, 200)] private int _jumpHeight;
    [SerializeField][Range(0, 200)] private int _jumpForce;
    [SerializeField][Range(0, 0.1f)] private float _leniencyJumpDuration;

    [Header("Joint settings")]
    [Range(0, 200)] public int spring;
    [Range(0, 200)] public int damper;
    [Range(0, 200)] public int massScale;

    [Header("Tongue settings")]
    [SerializeField][Range(0, 20)] private float _releaseForceMult;


    #region Setup
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _camera.SetActive(false);
    }

    void OnEnable()
    {
        _rigidbody.isKinematic = false;
        InputAction playerJump = InputSystem.actions.FindAction("Jump");
        playerJump.started += JumpCharge;
        playerJump.canceled += InputJump;
        playerJump.performed += JumpCancel;
        InputAction playerAttack = InputSystem.actions.FindAction("Attack");
        playerAttack.performed += ShootTongue;
        playerAttack.canceled += RetractTongue;
        InputAction playerGrip = InputSystem.actions.FindAction("Grip");
        playerGrip.started += GripOnGround;
        InputAction playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        playerToggleUI.performed += Pause;
        InputAction playerRestart = InputSystem.actions.FindAction("Restart");
        playerRestart.performed += Restart;
        _camera.SetActive(true);
        _cinemachineCamera.SetActive(true);
        _crosshair.SetActive(true);
    }

    void OnDisable()
    {
        _rigidbody.isKinematic = true;
        InputAction playerJump = InputSystem.actions.FindAction("Jump");
        playerJump.started -= JumpCharge;
        playerJump.canceled -= InputJump;
        playerJump.performed -= JumpCancel;
        InputAction playerAttack = InputSystem.actions.FindAction("Attack");
        playerAttack.performed -= ShootTongue;
        playerAttack.canceled -= RetractTongue;
        InputAction playerGrip = InputSystem.actions.FindAction("Grip");
        playerGrip.started -= GripOnGround;
        InputAction playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        playerToggleUI.performed -= Pause;
        InputAction playerRestart = InputSystem.actions.FindAction("Restart");
        playerRestart.performed -= Restart;
        _cinemachineCamera.SetActive(false);
        _crosshair.SetActive(false);
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
        _frogTongue.OnReset();
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.None;
        transform.position = gameManager.start.position + new Vector3(0, 0.25f, 0);
        _rigidbody.MovePosition(gameManager.start.position + new Vector3(0, 0.3f, 0));
        transform.eulerAngles = new Vector3(0, gameManager.startRotation, 0);
        _rigidbody.MoveRotation(Quaternion.Euler(0, gameManager.startRotation, 0));
        _jumpCharge = 0;
        canJump = true;
        isGrounded = true;
        _jumpToken = false;
        _cinemachineCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value = transform.eulerAngles.y;
        _cinemachineCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = 10;
        _cinemachineCamera.GetComponent<CinemachineInputAxisController>();
        _animator.Play("Idle", 0, 0);
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.Sleep();
    }
    #endregion

    private void GripOnGround(InputAction.CallbackContext context)
    {
        if (isGrounded && _rigidbody.linearVelocity != Vector3.zero)
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
        if (canJump && _isJumpCancelled == false && !gameManager.isGamePaused && !_jumpToken)
        {
            Jump(true);
        }
        else
        {
            _lastCharge = _jumpCharge;
            _jumpCharge = 0;
            if (!isGrounded)
            {
                if (_leniencyCoroutine != null)
                    StopCoroutine(_leniencyCoroutine);
                _leniencyCoroutine = StartCoroutine(LeniencyJump());
            }
        }
    }

    private void Jump(bool input)
    {
        if (canJump)
        {
            canJump = false;
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
            if (_playerInput.currentControlScheme == "Gamepad")
                ControllerRumble(0.2f, 0.2f, 0.1f);
        }
    }

    private IEnumerator GroundUpdate()
    {
        yield return new WaitForFixedUpdate();
        isGrounded = false;
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

        if (!isGrounded)
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

        if (foundGround != isGrounded)
        {
            isGrounded = foundGround;
            if (isGrounded) Land();
        }
    }

    private void Land()
    {
        if (!swinging) canJump = true;

        if (InputSystem.actions.FindAction("Grip").phase != InputActionPhase.Waiting)
            _rigidbody.linearVelocity = Vector3.zero;
        else
            _rigidbody.linearVelocity = new Vector3(_lastVelocity.x, 0, _lastVelocity.z) / 2;


        if (_jumpToken)
            Jump(false);
        else
        {
            _animator.Play("Land", -1, 0f);
            if (_playerInput.currentControlScheme == "Gamepad")
                ControllerRumble(0.2f, 0.2f, 0.25f);
        }
    }

    #endregion


    Vector3 point = new Vector3();

    #region DELETEONPROJECTEND
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(_boxCollider.bounds.center - new Vector3(0, 0.2f, 0), _boxCollider.bounds.size);
        Gizmos.DrawCube(point, new Vector3(2, 2, 2));
    }
    #endregion

    private void ShootTongue(InputAction.CallbackContext context)
    {
        if (gameManager.isGamePaused) return;


        RaycastHit hit;

        Vector3 adjustedForward = _camera.transform.forward + new Vector3(0, 0.35f, 0);

        if (Physics.Raycast(_camera.transform.position, adjustedForward, out hit, _frogTongue.maxRange + 10, ~(1 << 7)))
            point = hit.point;
        else
            return;

        if ((point != Vector3.zero) && canAttack && Vector3.Distance(transform.position, hit.point) <= _frogTongue.maxRange)
        {
            _frogTongue.gameObject.SetActive(true);

            _frogTongue.SetTarget(point);
            canAttack = false;
        }
    }



    private void RetractTongue(InputAction.CallbackContext context)
    {
        if (_frogTongue.StopSwinging() && TongueAngle())
            ReleaseForce();
    }

    private bool TongueAngle()
    {
        float dotProduct = Vector3.Dot(point, Vector3.up);

        if (dotProduct > 0)
        {
            float angleToUp = Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f)) * Mathf.Rad2Deg;

            float maxAngleFromHorizontal = 45;
            return angleToUp < maxAngleFromHorizontal;
        }
        else
            return false;
    }

    private void ReleaseForce()
    {
        lookDir = (transform.position - new Vector3(_cinemachineCamera.transform.position.x, transform.position.y, _cinemachineCamera.transform.position.z)).normalized;

        _rigidbody.AddForce(new Vector3(lookDir.x * _releaseForceMult, _releaseForceMult, lookDir.z * _releaseForceMult), ForceMode.Impulse);
    }

    public void ControllerRumble(float lowFreq, float highFreq, float duration)
    {
        if (_rumbleCoroutine != null)
        {
            StopCoroutine(_rumbleCoroutine);
        }
        _rumbleCoroutine = StartCoroutine(Rumble(lowFreq, highFreq, duration));
    }

    private IEnumerator Rumble(float lowFreq, float highFreq, float duration)
    {
        Gamepad.current.SetMotorSpeeds(lowFreq, highFreq);
        yield return new WaitForSecondsRealtime(duration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}