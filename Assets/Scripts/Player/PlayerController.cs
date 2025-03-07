using System;
using System.Collections;
using System.Threading;
using DG.Tweening.Plugins.Options;
using Unity.Android.Gradle;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Splines.Interpolators;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private GameManager _gameManager;
    private GameObject _camera;
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Animator _animator;

    public Vector3 lookDir;
    private Vector3 _lastVelocity;
    [SerializeField] private int _maxJumps;
    [SerializeField] private int _jumpsLeft;
    private bool _isJumpCancelled;
    private bool _isGrounded;
    float _jumpCharge;

    [SerializeField][Range(0, 200)] private int _jumpHeight;
    [SerializeField][Range(0, 200)] private int _jumpForce;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        _gameManager = GameManager.instance;
        _camera = transform.GetChild(1).gameObject;
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = transform.GetChild(0).GetComponent<Animator>();

        #region Inputs
        InputAction _playerAim = InputSystem.actions.FindAction("Aim");
        InputAction _playerLook = InputSystem.actions.FindAction("Look");
        _playerLook.performed += Look;
        _playerLook.canceled += Look;
        InputAction _playerJump = InputSystem.actions.FindAction("Jump");
        _playerJump.started += JumpCharge;
        _playerJump.canceled += Jump;
        _playerJump.performed += JumpCancel;
        InputAction _playerGrip = InputSystem.actions.FindAction("Grip");
        _playerGrip.started += GripOnGround;
        InputAction _playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        _playerToggleUI.performed += Pause;
        InputAction _playerRestart = InputSystem.actions.FindAction("Restart");
        _playerRestart.performed += Restart;
        #endregion

        _jumpsLeft = _maxJumps;
    }

    private void Look(InputAction.CallbackContext context)
    {
        // 
    }

    private void JumpCharge(InputAction.CallbackContext context) // Holding down jump button
    {
        _isJumpCancelled = false;
    }

    private void Jump(InputAction.CallbackContext context) // Released jump button when charging jump
    {
        if (_jumpsLeft != 0 && _isJumpCancelled == false)
        {
            lookDir = (transform.position - new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z)).normalized;
            _rigidbody.AddForce(new Vector3(lookDir.x * _jumpForce, (_jumpCharge + 0.3f) * _jumpHeight, lookDir.z * _jumpForce), ForceMode.Impulse);
            _jumpsLeft -= 1;
            _jumpCharge = 0;
            _rigidbody.rotation = Quaternion.LookRotation(lookDir, transform.up);
            _animator.Play("Jump");
            StartCoroutine(BugCheck());
        }
    }

    private void JumpCancel(InputAction.CallbackContext context) // Waited too long to jump
    {
        _isJumpCancelled = true;
        _jumpCharge = 0;
    }

    private void GripOnGround(InputAction.CallbackContext context)
    {
        if (_isGrounded && _rigidbody.linearVelocity != Vector3.zero)
            _rigidbody.linearVelocity = Vector3.zero;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        GameManager.instance.PauseToggle(PauseSetting.toggle, true);
    }

    private void Restart(InputAction.CallbackContext context)
    {
        _gameManager.RestartLevel();
        _gameManager.PauseToggle(PauseSetting.pause, false);
    }

    void Update()
    {
        // Charge the jump if the jump button is being held
        if (InputSystem.actions.FindAction("Jump").phase == InputActionPhase.Started && _jumpCharge < 1)
        {
            _jumpCharge += 1 * Time.deltaTime;
            if (_jumpCharge > 1)
                _jumpCharge = 1;
        }
    }

    void FixedUpdate()
    {
        if (!_isGrounded)
            _rigidbody.AddForce(new Vector3(0, -1f, 0) * _rigidbody.mass * 30);

        _lastVelocity = _rigidbody.linearVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isGrounded)
        {
            RaycastHit[] hit = Physics.BoxCastAll(transform.position, GetComponent<Collider>().bounds.size, Vector3.down, quaternion.identity, 1f);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].transform.CompareTag("Ground"))
                    {
                        Land();
                        break;
                    }
                }
            }
        }
    }

    // Check if we haven't left the ground, but expended a jump. If both true: 
    // we determine that we are stuck and should be reset back to being on the ground properly
    IEnumerator BugCheck()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (_jumpsLeft != _maxJumps && _isGrounded == true)
        {
            Land();
        }
    }

    void Land()
    {
        _jumpsLeft = _maxJumps;
        _isGrounded = true;
        if (InputSystem.actions.FindAction("Grip").phase != InputActionPhase.Waiting)
            _rigidbody.linearVelocity = Vector3.zero;
        else
            _rigidbody.linearVelocity = new Vector3(_lastVelocity.x, 0, _lastVelocity.z) / 2;
        _animator.Play("FrogLand");
    }

    void OnCollisionExit(Collision collision)
    {
        _isGrounded = false;
    }
}
