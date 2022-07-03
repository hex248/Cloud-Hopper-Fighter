using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public int playerNumber;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField][Range(0.0f,1.0f)] float airMultiplier = 0.75f;
    [SerializeField] [Range(0.0f, 100.0f)] float jumpForce= 50.0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDrag = 0.5f;
    [SerializeField] Transform orientation;
    [SerializeField][Range(0.0f, 10.0f)] float rotationSmoothing = 2.0f;
    bool grounded;
    bool canJump;

    Quaternion desiredRotation;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] public GameObject model;

    //public InputAction input;
    PlayerManager playerManager;
    PlayerInput playerInput;

    float horizontal;
    float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerNumber = playerInput.playerIndex + 1;
        playerManager.PlayerSpawned(this);
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

        if (vertical > 0.5f) vertical = 1.0f;
        else if (vertical < -0.5f) vertical = -1.0f;
        else vertical = 0.0f;
        if (horizontal > 0.5f) horizontal = 1.0f;
        else if (horizontal < -0.5f) horizontal = -1.0f;
        else horizontal = 0.0f;

        Vector3 moveDirection = orientation.forward * vertical + orientation.right * horizontal;

        float speed = grounded ? moveSpeed : moveSpeed * airMultiplier;
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }

        if (moveDirection != Vector3.zero) desiredRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        if (IsValidQuaternion(desiredRotation)) model.transform.rotation = Quaternion.Lerp(model.transform.rotation, desiredRotation, ((10.0f - rotationSmoothing) * Time.deltaTime));

        animator.SetBool("moving", moveDirection != Vector3.zero);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canJump)
        {
            rb.AddForce(0, jumpForce, 0);
        }
    }

    public bool IsValidQuaternion(Quaternion q)
    {
        return q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0 ? false : true;
    }
}
