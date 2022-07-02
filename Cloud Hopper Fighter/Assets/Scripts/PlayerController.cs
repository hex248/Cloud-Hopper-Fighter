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
    [SerializeField] bool lockedMovement = false;

    [SerializeField] float moveSpeed = 5.0f;

    void Start()
    {
        
    }

    void Update()
    {
        MovementDirections direction = MovementDirections.none;

        if (inputMode == InputModes.keyboard)
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
            }
        }
    }
}
