using System;
using System.Threading;
using DG.Tweening.Plugins.Options;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private Vector3 lookDir;
    [SerializeField] private int _maxJumps;
    [SerializeField] private int _jumpsLeft;
    private bool _isJumpCancelled;
    private bool _isGrounded;
    float _jumpCharge;

    [SerializeField][Range(0, 20)] private int _jumpHeight;
    [SerializeField][Range(0, 20)] private int _jumpForce;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (instance == null)
            instance = this;


        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();

        #region Inputs
        InputAction _playerAim = InputSystem.actions.FindAction("Aim");
        InputAction _playerLook = InputSystem.actions.FindAction("Look");
        _playerLook.performed += Look;
        _playerLook.canceled += Look;
        InputAction _playerJump = InputSystem.actions.FindAction("Jump");
        _playerJump.started += JumpCharge;
        _playerJump.canceled += Jump;
        _playerJump.performed += JumpCancel;
        #endregion

        _jumpsLeft = _maxJumps;

    }

    private void Look(InputAction.CallbackContext context)
    {
        Debug.Log("Hi");
        lookDir = (transform.position - new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z)).normalized;
    }

    private void JumpCharge(InputAction.CallbackContext context)
    {
        Debug.Log("Charging jump");
        _isJumpCancelled = false;
        // _jumpsLeft = _maxJumps;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_jumpsLeft != 0 && _isJumpCancelled == false)
        {
            _rigidbody.AddRelativeForce(new Vector3(lookDir.x * _jumpForce, (_jumpCharge + 0.3f) * _jumpHeight, lookDir.z * _jumpForce), ForceMode.Impulse);
            _jumpsLeft -= 1;
            _jumpCharge = 0;
        }
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Nevermind I don't wanna jump");
        _isJumpCancelled = true;
        _jumpCharge = 0;
    }

    void Update()
    {
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
}
