using System.Threading;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem; // Required for the new Input System

[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    //Added Jumping variables
    [SerializeField] private float jumpForce = 5f;
    private int jumpCount = 0;
    private const int maxJumps = 2;
    private bool canWallJump = false;
    //New and initial variables
    public float speed = 6f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private GameObject player;
    


    //new variable created by connor for jumping
    [SerializeField] private Vector3 velocity;
    private Vector3 move;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() //use update not fixed update when using (WasPressedThisFrame) because fixed update does not run every frame
    {
        // Ensure a keyboard is connected
        if (Keyboard.current == null) return;

        // Change movement variables to change with speed
        float topSpeed = speed/4;
        float friction = -speed/3;
        
        if (Keyboard.current.spaceKey.wasPressedThisFrame && ((jumpCount < maxJumps)||canWallJump)) //only allow jump on keyboard pressed and if the jump count is less than the maxjumps
        {
            if (canWallJump&&(jumpCount==maxJumps))
            {
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump();
                    move.x = -3f;
                }
                else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump();
                    move.x = 3f;
                }
            }
            else Jump();
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame) Debug.Log("Grounded?: "+canWallJump);

        //make the player jump
        velocity.y += 2.5f*gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);



        // Check WASD or Arrow keys using the new Input System
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x += -speed*Time.deltaTime;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += speed*Time.deltaTime;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.z += -speed*Time.deltaTime;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.z += speed*Time.deltaTime;

        // Clamps player speed to go no faster than max speed, which is determined by speed
        move.x = math.clamp(move.x,-topSpeed,topSpeed);
        move.z = math.clamp(move.z,-topSpeed,topSpeed);
        
        Debug.Log("isGrounded: " + controller.isGrounded);
        Debug.Log("move.x: "+move.x);
        Debug.Log("canWallJump: "+canWallJump);

        if (controller.isGrounded) //reset velocity and jump count here only while grounded and not moving horizontally
        {   
            velocity.y = -2f;
            jumpCount = 0;
            if (move.x < 0f) move.x -= friction * Time.deltaTime;
            if (move.x > 0f) move.x += friction * Time.deltaTime;
            if (move.z > 0f) move.z += friction * Time.deltaTime;
            if (move.z < 0f) move.z -= friction * Time.deltaTime;
        }

        // Move the player
        controller.Move(move * speed * Time.deltaTime);
    }
    void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -1f * gravity);
        jumpCount++;
        //Change direction if opposite direction is held while midair
        if (((move.x>0)&&(Keyboard.current.aKey.isPressed||Keyboard.current.leftArrowKey.isPressed))||(move.x<0&&(Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed))) move.x*=-1;
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Wall")) canWallJump = true;
    }
    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Wall")) canWallJump = false;
    }

}