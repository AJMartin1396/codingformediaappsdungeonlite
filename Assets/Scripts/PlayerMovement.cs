using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;
    public float moveY = 0f;
    public float gravity = 0.1f;
    private CharacterController controller;
    private GameObject player;
    private bool canDouble = false;
    void Start()
    {
        controller = GetComponent<CharacterController>();
       
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        // Ensure a keyboard is connected
        if (Keyboard.current == null) return;
        float moveX = 0f;
        float moveZ = 0f;
        if (controller.isGrounded)
        {
            moveY = 0f;
            canDouble = true;
        }
        else moveY -= gravity;



        // Check WASD or Arrow keys using the new Input System
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;
        if (Input.getKeyDown("space")) if (controller.isGrounded) moveY = 4f;
        else if (canDouble)
        {
            moveY = 2f;
            canDouble = false;
        }

        // Combine inputs into a direction vector and normalize so diagonal movement isn't faster
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
        move.y = moveY;
        

        // Move the player
        controller.Move(move * speed * Time.deltaTime);
    }
}