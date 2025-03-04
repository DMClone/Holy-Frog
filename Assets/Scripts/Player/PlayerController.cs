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

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    private Animator _animator;

    public Vector3 lookDir;
    [SerializeField] private int _maxJumps;
    [SerializeField] private int _jumpsLeft;
    private bool _isJumpCancelled;
    private bool _isGrounded;
    float _jumpCharge;

    [SerializeField][Range(0, 20)] private int _jumpHeight;
    [SerializeField][Range(0, 20)] private int _jumpForce;


    private void Awake()
    {
        if (instance == null)
            instance = this;

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
        InputAction _playerToggleUI = InputSystem.actions.FindAction("TogglePause");
        _playerToggleUI.performed += Pause;
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
            lookDir = (transform.position - new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z)).normalized;
            // lookDir = transform.GetChild(2).GetComponent<CinemachineOrbitalFollow>().HorizontalAxis;
            _rigidbody.AddForce(new Vector3(lookDir.x * _jumpForce, (_jumpCharge + 0.3f) * _jumpHeight, lookDir.z * _jumpForce), ForceMode.Impulse);
            _jumpsLeft -= 1;
            _jumpCharge = 0;
            _rigidbody.rotation = Quaternion.LookRotation(lookDir, transform.up);
            _animator.Play("Jump");
        }
    }

    private void JumpCancel(InputAction.CallbackContext context) // Waited too long to jump
    {
        _isJumpCancelled = true;
        _jumpCharge = 0;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        GameManager.instance.PauseToggle();
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
        _rigidbody.AddForce(new Vector3(0, -1f, 0) * _rigidbody.mass * 30);
    }

    void OnCollisionEnter(Collision collision)
    {
        RaycastHit[] hit = Physics.BoxCastAll(transform.position, GetComponent<Collider>().bounds.size, Vector3.down, quaternion.identity, 1f);
        if (hit.Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.CompareTag("Ground"))
                {
                    _jumpsLeft = _maxJumps;
                    _animator.Play("FrogLand");
                    break;
                }
            }
        }
    }



}
