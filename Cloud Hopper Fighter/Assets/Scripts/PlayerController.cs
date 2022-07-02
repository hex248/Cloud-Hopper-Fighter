using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MovementDirections
{
    none,
    up,
    down,
    left,
    right,
    upLeft,
    upRight,
    downLeft,
    downRight
}

enum InputModes
{
    keyboard,
    controller
}

public class PlayerController : MonoBehaviour
{
    InputModes inputMode;
    Rigidbody rb;

    [Header("Movement")]
    [SerializeField] bool lockedMovement = false;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField][Range(0.0f,1.0f)] float airMultiplier = 0.75f;
    [SerializeField] [Range(0.0f, 100.0f)] float jumpForce= 50.0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDrag = 0.5f;
    [SerializeField] Transform orientation;
    [SerializeField][Range(0.0f,1.0f)] float rotationSmoothing = 0.5f;
    bool grounded;
    bool canJump;

    Quaternion desiredRotation;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject model;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.7f, groundLayer);

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        if (grounded)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        Vector3 moveDirection = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");

        float speed = grounded ? moveSpeed : moveSpeed * airMultiplier;
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        if (moveDirection != Vector3.zero) desiredRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        model.transform.rotation = Quaternion.Lerp(model.transform.rotation, desiredRotation, ((1.0f - rotationSmoothing) * Time.deltaTime) * 20.0f);

        animator.SetBool("moving", moveDirection != Vector3.zero);

        //if (Input.GetAxisRaw("Jump") > 0 && canJump) Jump();


        // old movement system
        #region
        MovementDirections direction = MovementDirections.none;
        if (1 > 2 && inputMode == InputModes.keyboard)
        {
            if (lockedMovement)
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");

                if (vertical == 1.0f) // up
                {
                    switch (horizontal)
                    {
                        case -1.0f:
                            direction = MovementDirections.upLeft;
                            break;
                        case 1.0f:
                            direction = MovementDirections.upRight;
                            break;
                        default:
                            direction = MovementDirections.none;
                            break;
                    }
                }
                else if (vertical == -1.0f) //down
                {
                    switch (horizontal)
                    {
                        case -1.0f:
                            direction = MovementDirections.downLeft;
                            break;
                        case 1.0f:
                            direction = MovementDirections.downRight;
                            break;
                        default:
                            direction = MovementDirections.none;
                            break;
                    }
                }

                Vector3 movement;
                switch (direction)
                {
                    case MovementDirections.upLeft:
                        movement = new Vector3(0.0f, 0.0f, 1.0f);
                        break;
                    case MovementDirections.upRight:
                        movement = new Vector3(1.0f, 0.0f, 0.0f);
                        break;
                    case MovementDirections.downLeft:
                        movement = new Vector3(-1.0f, 0.0f, 0.0f);
                        break;
                    case MovementDirections.downRight:
                        movement = new Vector3(0.0f, 0.0f, -1.0f);
                        break;
                    default:
                        movement = new Vector3(0.0f, 0.0f, 0.0f);
                        break;
                }
                Debug.Log(movement);
                transform.position += movement * moveSpeed * Time.deltaTime;
            }
            else
            {
                float horizontal = Input.GetAxisRaw("Horizontal");
                float vertical = Input.GetAxisRaw("Vertical");

                if (vertical == 1.0f) // up
                {
                    switch (horizontal)
                    {
                        case -1.0f:
                            direction = MovementDirections.upLeft;
                            break;
                        case 1.0f:
                            direction = MovementDirections.upRight;
                            break;
                        case 0.0f:
                            direction = MovementDirections.up;
                            break;
                        default:
                            direction = MovementDirections.none;
                            break;
                    }
                }
                else if (vertical == -1.0f) //down
                {
                    switch (horizontal)
                    {
                        case -1.0f:
                            direction = MovementDirections.downLeft;
                            break;
                        case 1.0f:
                            direction = MovementDirections.downRight;
                            break;
                        case 0.0f:
                            direction = MovementDirections.down;
                            break;
                        default:
                            direction = MovementDirections.none;
                            break;
                    }
                }
                else if (vertical == 0)
                {
                    switch (horizontal)
                    {
                        case -1.0f:
                            direction = MovementDirections.left;
                            break;
                        case 1.0f:
                            direction = MovementDirections.right;
                            break;
                        case 0.0f:
                            direction = MovementDirections.none;
                            break;
                        default:
                            direction = MovementDirections.none;
                            break;
                    }
                }

                Debug.Log(direction);

                Vector3 movement;
                switch (direction)
                {
                    case MovementDirections.up:
                        movement = new Vector3(1.0f, 0.0f, 1.0f);
                        break;
                    case MovementDirections.down:
                        movement = new Vector3(-1.0f, 0.0f, -1.0f);
                        break;
                    case MovementDirections.left:
                        movement = new Vector3(-1.0f, 0.0f, 1.0f);
                        break;
                    case MovementDirections.right:
                        movement = new Vector3(1.0f, 0.0f, -1.0f);
                        break;
                    case MovementDirections.upLeft:
                        movement = new Vector3(0.0f, 0.0f, 1.0f);
                        break;
                    case MovementDirections.upRight:
                        movement = new Vector3(1.0f, 0.0f, 0.0f);
                        break;
                    case MovementDirections.downLeft:
                        movement = new Vector3(-1.0f, 0.0f, 0.0f);
                        break;
                    case MovementDirections.downRight:
                        movement = new Vector3(0.0f, 0.0f, -1.0f);
                        break;
                    default:
                        movement = Vector3.zero;
                        break;
                }

                rb.AddForce(movement.normalized * moveSpeed * 10f, ForceMode.Force);
            }
        }
        #endregion
    }

    void Jump()
    {
        rb.AddForce(0, jumpForce, 0);
    }
}
