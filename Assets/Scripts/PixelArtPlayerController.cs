using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PixelArtPlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private LayerMask climbingLayers;

    private InputMaster _inputMaster;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D collider;
    private PlayerInput _playerInput;

    public float WalkSpeed;
    public float RunSpeedMultiplier;
    public float JumpForce;
    public float ClimbingSpeedMultiplier;

    private bool Running = false;
    private bool CanClimb = false;
    private bool IsClimbing;
    
    private int CoinsCollected = 0;

    public void AddCoin()
    {
        CoinsCollected++;
        FindObjectOfType<CoinUI>().SetText(CoinsCollected);
    }

    public void RemoveCoin() => CoinsCollected--;

    public int GetCoins()
    {
        return CoinsCollected;
    }

    private float gravityScale;

    private void Start()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        _playerInput = GetComponent<PlayerInput>();

        gravityScale = rb.gravityScale;
    }

    private void Update()
    {
        // Walk
        if (!IsClimbing)
        {
            Walk();
        }
        // Jump
        if (_inputMaster.Player.Jump.triggered && IsGrounded())
        {
            Jump();
        }

        //Flying
        if (!IsClimbing)
        {
            anim.SetBool("Flying", !IsGrounded());
        }

        // Climb
        if (_inputMaster.Player.StartClimb.triggered && CanClimb)
        {
            anim.ResetTrigger("Flying");
            print("Started climbing");
            IsClimbing = true;
            _playerInput.SwitchCurrentActionMap("Climb");
            rb.gravityScale = 0;
            Running = false;
        }

        if (IsClimbing)
        {
            Climb();
        }
    }

    private void Walk()
    {
        float movementInput = _inputMaster.Player.Walk.ReadValue<float>() * WalkSpeed;

        //if running
        if (_inputMaster.Player.Run.triggered)
        {
            Running = !Running;
        }

        if (Running)
        {
            movementInput *= RunSpeedMultiplier;
        }
        
        //Animations
        if (movementInput != 0 && IsGrounded())
        {
            anim.SetTrigger(Running ? "Run" : "Walk");
        }
        else
        {
            anim.SetTrigger("Idle");
        }
        
        //Rotate
        RotatePlayer(movementInput);

        rb.velocity = new Vector2(movementInput * WalkSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        print("Jump Button Pressed");
        rb.AddForce(new Vector2(0, JumpForce));
    }

    private void Climb()
    {
        Vector2 movementInput = _inputMaster.Climb.Climb.ReadValue<Vector2>();
        anim.SetTrigger("Climb");

        //Animations
        anim.speed = movementInput != Vector2.zero ? 1 : 0;

        rb.velocity = movementInput * ClimbingSpeedMultiplier;
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.05f;
        Bounds colliderBounds = collider.bounds;
        Vector2 origin = new Vector2(colliderBounds.center.x - colliderBounds.extents.x, colliderBounds.center.y - (colliderBounds.extents.y + extraHeight));
        float distance = colliderBounds.size.x;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, distance, groundLayers);

        Color color;
        color = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector2.right * distance, color);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (climbingLayers == (climbingLayers | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Entered climbing zone");
            CanClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (climbingLayers == (climbingLayers | (1 << collision.gameObject.layer)))
        {
            Debug.Log("Exited climbing zone");
            CanClimb = false;

            IsClimbing = false;
            _playerInput.SwitchCurrentActionMap("Player");
            rb.gravityScale = gravityScale;
        }
    }

    private void RotatePlayer(float movementInput)
    {
        if (movementInput < 0) { transform.rotation = Quaternion.Euler(0, 180, 0); }
        if (movementInput > 0) { transform.rotation = Quaternion.Euler(0, 0, 0); }
    }
}