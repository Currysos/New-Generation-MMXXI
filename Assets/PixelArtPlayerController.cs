using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PixelArtPlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayers;
    
    private InputMaster _inputMaster;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D collider;

    public float WalkSpeed;
    public float RunSpeedMultiplier;
    public float JumpForce;
    private bool Running = false;
    
    
    private void Awake()
    {
        
    }

    void Start()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Walk
        Walk();
        // Jump
        if (_inputMaster.Player.Jump.triggered && IsGrounded())
        {
            Jump();
        }
        
        
        //Flying
        anim.SetBool("Flying", !IsGrounded());
        
        // Climb
        if (_inputMaster.Player.Climb.triggered)
        {
            
        }
    }

    void Walk()
    {
        float movementInput = _inputMaster.Player.Walk.ReadValue<float>() * WalkSpeed;
        
        //if running
        if (_inputMaster.Player.Run.triggered)
        {
            Running = !Running;
        }

        if (Running)
        {
            print("Running: " + Running);
            movementInput *= RunSpeedMultiplier;
        }
        
        //Animations
        if (movementInput != 0 && IsGrounded())
        {
            // anim.ResetTrigger("Idle");
            anim.SetTrigger(Running ? "Run" : "Walk");
        }
        else
        {
            anim.SetTrigger("Idle");
        }
        //Rotate
        if (movementInput < 0) { transform.rotation = Quaternion.Euler(0, 180, 0); }
        if (movementInput > 0) { transform.rotation = Quaternion.Euler(0, 0, 0); }
        
        rb.velocity = new Vector2(movementInput * WalkSpeed, rb.velocity.y);
    }

    void Jump()
    {
        print("Jump Button Pressed");
        rb.AddForce(new Vector2(0, JumpForce));
        anim.SetTrigger("Jump");
    }

    private bool IsGrounded()
    {
        float extraHeightText = 0.05f;
        Vector2 origin = collider.bounds.center;
        float distance = collider.bounds.extents.y + extraHeightText;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, distance, groundLayers);
        
        Color color;
        color = hit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(origin, Vector2.down * distance, color);
        return hit.collider != null;
    }
}
