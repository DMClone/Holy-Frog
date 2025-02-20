using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    private bool _canJump;
    [SerializeField] float _jumpCharge;


    private void Awake()
    {
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

    }

    private void Look(InputAction.CallbackContext context)
    {

    }

    private void JumpCharge(InputAction.CallbackContext context)
    {
        Debug.Log("Charging jump");
        _canJump = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_canJump)
        {
            _rigidbody.AddRelativeForce(new Vector3(0, (_jumpCharge + 0.3f) * 10, 1), ForceMode.Impulse);
            _jumpCharge = 0;
        }
    }

    private void JumpCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Nevermind I don't wanna jump");
        _canJump = false;
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
}
