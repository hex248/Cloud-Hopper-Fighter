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
    [SerializeField] public Camera camera;
    ScreenManager screenManager;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField][Range(0.0f,1.0f)] float airMultiplier = 0.75f;
    [SerializeField] [Range(0.0f, 100.0f)] float jumpForce= 50.0f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDrag = 0.5f;
    [SerializeField] GameObject orientationObject;
    Transform orientation;
    [SerializeField][Range(0.0f, 10.0f)] float rotationSmoothing = 2.0f;

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] float spawnSpacing;

    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask noCollisionLayer;

    bool grounded;
    bool canJump;

    Quaternion desiredRotation;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] public GameObject model;

    //public InputAction input;
    PlayerManager playerManager;
    PlayerInput playerInput;
    PlayerInputManager playerInputManager;

    float horizontal;
    float vertical;

    [Header("Combat")]
    [SerializeField] public bool attacking;
    [SerializeField] public bool canAttack = true;
    [SerializeField] public Attack currentAttack;
    public bool hitPlayer = false;
    public int health = 100;

    public bool lockedInput = false;
    public bool waitingForRespawn = true;

    void Start()
    {
        orientation = orientationObject.transform;
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInput.playerIndex + 1;
        playerManager.PlayerSpawned(this);
        screenManager = FindObjectOfType<ScreenManager>();
        transform.position = spawnPosition + new Vector3(playerNumber * spawnSpacing, 0, -playerNumber * spawnSpacing);
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 0.7f, groundLayer);

        if (grounded) rb.drag = groundDrag;
        else rb.drag = 0;

        if (grounded && !lockedInput)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
        if (!lockedInput)
        {
            vertical = Mathf.Round(vertical);
            horizontal = Mathf.Round(horizontal);

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

    public void OnPunch(InputAction.CallbackContext context)
    {
        if (!canAttack) return; // only allow one attack at a time
        attacking = true;
        canAttack = false;
        currentAttack = playerManager.attacks.Find(attack => attack.attackName == "Punch");
        animator.SetTrigger("punch");
        StartCoroutine(StartAttackTimer(currentAttack.hitDuration, currentAttack.cooldown));
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        gameObject.layer = playerLayer;
        animator.Rebind();
        animator.Update(0f);
        lockedInput = false;
        waitingForRespawn = false;
        // hide dead overlay

    }

    public bool IsValidQuaternion(Quaternion q)
    {
        return q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0 ? false : true;
    }

    public void TakeHit(Attack attack, PlayerController attacker)
    {
        Debug.Log($"Player {playerNumber} hit by punch");
        animator.SetTrigger("hit");
        health -= attack.damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator StartAttackTimer(float hitDuration, float cooldown)
    {
        yield return new WaitForSeconds(hitDuration);
        attacking = false;
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        hitPlayer = false;
        yield return null;
    }

    public IEnumerator Die()
    {
        // play death animation
        animator.SetTrigger("death");

        // lock input
        lockedInput = true;
        gameObject.layer = noCollisionLayer;
        yield return new WaitForSeconds(2.5f);

        // show dead overlay


        // wait to respawn
        waitingForRespawn = true;

        yield return null;
    }
}
