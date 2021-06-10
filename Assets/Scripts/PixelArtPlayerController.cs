using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PixelArtPlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask climbingLayers;
    [SerializeField] private LayerMask deathLayers;
    [SerializeField] private UnityEvent DeathEvents;

    private InputMaster _inputMaster;
    private Animator _anim;
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private PlayerInput _playerInput;
    private LivesUI _livesUI;
    private Vector2 _waypoint;

    private bool _running = false;
    private bool _canClimb = false;
    private bool _isClimbing;
    private bool _holdingDownJump = false;
    public enum PlayerState
    {
        Playing,
        InDialog,
        Paused
    }

    private PlayerState _state;

    private bool IsGrounded()
    {
        float extraHeight = 0.02f;
        Bounds colliderBounds = _collider.bounds;
        Vector2 origin = new Vector2(colliderBounds.center.x - colliderBounds.extents.x, colliderBounds.center.y - (colliderBounds.extents.y + extraHeight));
        float distance = colliderBounds.size.x;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, distance, groundLayers);

        Color color;
        color = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector2.right * distance, color);
        return hit.collider != null;
    }
    private bool IsFalling()
    {
        return _rb.velocity.y < -0.1;
    }

    private bool CanAddForce(float maxMagnitude)
    {
        return _rb.velocity.y < maxMagnitude;
    }
    
    private float _gravityScale;

    public float walkSpeed;
    public float runSpeedMultiplier;
    public float climbingSpeedMultiplier;
    public float jumpForce;
    public float jumpDecayRate;
    
    public int NumberOfLives { get; set; } = 3;

    private int _coinsCollected = 0;
    

    public int CoinsCollected
    {
        get => _coinsCollected;
        set
        {
            _coinsCollected = value;
            FindObjectOfType<CoinUI>().SetText(_coinsCollected);
        }
    }

    

    private void Start()
    {
        _waypoint = transform.position;
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _playerInput = GetComponent<PlayerInput>();
        _livesUI = FindObjectOfType<LivesUI>();
        
        _livesUI.SpawnLives(NumberOfLives);

        _gravityScale = _rb.gravityScale;

        SetState(PlayerState.Playing);
    }

    private void Update()
    {
        //Flying
        if (!_isClimbing)
        {
            _anim.SetBool("Flying", !IsGrounded());
        }
        
        // Jump
        if (_inputMaster.Player.Jump.triggered) _holdingDownJump = !_holdingDownJump;
        
        switch (_state)
        {
            case PlayerState.Playing:
                handleInput();
                break;
            case PlayerState.Paused:
                break;
            case PlayerState.InDialog:
                
                // Resetting Parameters
                _anim.SetTrigger("Idle");
                break;
        }
    }

    private void handleInput()
    {
        // Walk
        if (!_isClimbing)
        {
            Walk();
        }
        
        if (_holdingDownJump && IsGrounded()) Jump();

        // Climb
        if (_inputMaster.Player.StartClimb.triggered && _canClimb)
        {
            _anim.ResetTrigger("Flying");
            print("Started climbing");
            _isClimbing = true;
            _playerInput.SwitchCurrentActionMap("Climb");
            _rb.gravityScale = 0;
            _running = false;
        }

        if (_isClimbing)
        {
            Climb();
        }
    }

    private void Walk()
    {
        float movementInput = _inputMaster.Player.Walk.ReadValue<float>() * walkSpeed;

        //if running
        if (_inputMaster.Player.Run.triggered)
        {
            _running = !_running;
        }

        if (_running)
        {
            movementInput *= runSpeedMultiplier;
        }
        
        //Animations
        if (movementInput != 0 && IsGrounded())
        {
            _anim.SetTrigger(_running ? "Run" : "Walk");
        }
        else
        {
            _anim.SetTrigger("Idle");
            _running = false;
        }
        
        //Rotate
        RotatePlayer(movementInput);

        _rb.velocity = new Vector2(movementInput * walkSpeed, _rb.velocity.y);
    }

    private void Jump()
    {
        print("Jump Button Pressed");
        
        StartCoroutine(DoJump());
    }

    private IEnumerator DoJump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
        var currentSpeed = jumpForce;
        
        yield return null;

        while(_holdingDownJump && currentSpeed > 0.1)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, currentSpeed);
             
            currentSpeed -= jumpDecayRate * Time.deltaTime;
            yield return null;
        }
    }

    private void Climb()
    {
        Vector2 movementInput = _inputMaster.Climb.Climb.ReadValue<Vector2>();
        _anim.SetTrigger("Climb");

        //Animations
        _anim.speed = movementInput != Vector2.zero ? 1 : 0;

        _rb.velocity = movementInput * climbingSpeedMultiplier;
    }

    public void SetState(PlayerState newState)
    {
        _state = newState;
    }

    public void EnteringDialog()
    {
        print("Entering Dialog");
        SetState(PlayerState.InDialog);
        _rb.velocity = Vector2.zero;
    }

    public void ExitingDialog()
    {
        print("Exiting Dialog");
        SetState(PlayerState.Playing);
    }

    private void TakeLife()
    {
        NumberOfLives--;
        if (_livesUI.RemoveLife())
        {
            transform.position = _waypoint;
            print("Player took life and teleported to start");
            return;
        }
        
        Death();
    }

    private void Death()
    {
        print("Player died");
        DeathEvents.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (climbingLayers == (climbingLayers | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Entered climbing zone");
            _canClimb = true;
        }

        if (deathLayers == (deathLayers | (1 << collision.gameObject.layer)))
        {
            print("Player entered death zone");
            TakeLife();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (climbingLayers == (climbingLayers | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Exited climbing zone");
            _canClimb = false;

            _isClimbing = false;
            _playerInput.SwitchCurrentActionMap("Player");
            _rb.gravityScale = _gravityScale;
        }
    }

    private void RotatePlayer(float movementInput)
    {
        if (movementInput < 0) { transform.rotation = Quaternion.Euler(0, 180, 0); }
        if (movementInput > 0) { transform.rotation = Quaternion.Euler(0, 0, 0); }
    }

    
}