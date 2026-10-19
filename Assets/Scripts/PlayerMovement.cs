using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    //Added Jumping variables
    [SerializeField] private float jumpForce = 5f;
    private int jumpCount = 0;
    private const int maxJumps = 2;
    //New and initial variables
    public float speed = 6f;
    public float moveY = 0f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private GameObject player;


    //new variable created by connor for jumping
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() //use update not fixed update when using (WasPressedThisFrame) because fixed update does not run every frame
    {
        // Ensure a keyboard is connected
        if (Keyboard.current == null) return;
        float moveX = 0f;
        float moveZ = 0f;


        if (controller.isGrounded && velocity.y < 0f) //reset velocity and jump count here only while grounded and not moving horizontally
        {
            velocity.y = -2f;
            jumpCount = 0;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && jumpCount < maxJumps) //only allow jump on keyboard pressed and if the jump count is less than the maxjumps
        {
            Jump();
        }

        //make the player jump
        velocity.y += 2*gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // Check WASD or Arrow keys using the new Input System
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;

        // Combine inputs into a direction vector and normalize so diagonal movement isn't faster
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // Move the player
        controller.Move(move * speed * Time.deltaTime);
    }
    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -1f * gravity);
        jumpCount++;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; //Resets jumps
        }
    }

}