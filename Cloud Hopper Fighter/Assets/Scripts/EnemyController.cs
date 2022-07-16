using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("AI Configurables")]
    public float moveSpeed = 5.0f;
    public float viewDistance = 5.0f;
    public float attackDistance = 1.0f;
    public float attackSpeed = 0.5f;
    public float attackDuration = 0.3f;
    public float attackCooldownTime = 0.2f;
    public LayerMask targetMask;
    [SerializeField] float rotationLerpSpeed = 1.0f;

    [Header("Component References")]
    [SerializeField] GameObject model;
    [SerializeField] Animator animator;
    Rigidbody rb;
    Quaternion targetRotation;

    [Header("Read Only")]
    [ReadOnlyInspector] public GameObject targetPlayerObject;
    [ReadOnlyInspector] public PlayerController targetPlayer;
    [ReadOnlyInspector] public float targetPlayerDistance;
    [ReadOnlyInspector] public bool hasTarget = false;
    [ReadOnlyInspector] public bool canAttack = true;
    [ReadOnlyInspector] public bool attacking = false;
    [ReadOnlyInspector] public bool waitingToAttack = false;
    [ReadOnlyInspector] public bool attackCooldown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (waitingToAttack || attacking || attackCooldown) canAttack = false;
        else canAttack = true;

        targetPlayerObject = LocatePlayer(); // try to locate target player
        UpdateTarget();


        if (targetPlayerDistance < attackDistance && hasTarget) // if the target is within the attack range
        {
            if (canAttack) // if not already attacking
            {
                StartCoroutine(Attack()); // trigger an attack
            }
        }

        if (hasTarget)
        {
            Vector3 moveDirection = targetPlayerObject.transform.position - transform.position;
            moveDirection = moveDirection.normalized;
            rb.velocity += moveDirection * moveSpeed;
            targetRotation = Quaternion.LookRotation(targetPlayerObject.transform.position - transform.position);
        }
        
        model.transform.rotation = Quaternion.Lerp(model.transform.rotation, targetRotation, rotationLerpSpeed * Time.deltaTime);
    }

    GameObject LocatePlayer()
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, viewDistance, targetMask); // get all colliders within view distance
        GameObject target = null;
        Debug.Log(objectsDetected.Length);
        foreach (Collider col in objectsDetected)
        {
            if (col.gameObject.tag == "Player")
            {
                if (!hasTarget) // if there is not already a target
                {
                    target = col.gameObject;
                    targetPlayerDistance = Vector3.Distance(transform.position, target.transform.position); // update target distance
                    hasTarget = true;
                }
                else
                {
                    if (Vector3.Distance(transform.position, col.gameObject.transform.position) < targetPlayerDistance) // if this player is closer than the current target
                    {
                        // switch target
                        target = col.gameObject;
                        targetPlayerDistance = Vector3.Distance(transform.position, target.transform.position); // update target distance
                        hasTarget = true;
                    }
                }
            }
        }

        return target;
    }

    void UpdateTarget()
    {
        if (targetPlayerObject != null)
        {
            targetPlayer = targetPlayerObject.GetComponent<PlayerController>(); // get playercontroller
            hasTarget = true;
        }
        else
        {
            targetPlayer = null;
            targetPlayerDistance = 0.0f;
            hasTarget = false;
        }
    }

    IEnumerator Attack()
    {
        waitingToAttack = true;

        // warm up stage
        yield return new WaitForSeconds(attackSpeed);
        waitingToAttack = false;
        attacking = true;

        // play animation
        animator.SetTrigger("punch");

        // attacking/animation stage
        yield return new WaitForSeconds(attackDuration);

        attacking = false;

        attackCooldown = true;

        // cooldown stage
        yield return new WaitForSeconds(attackCooldownTime);

        attackCooldown = false;

        yield return null;
    }
}
