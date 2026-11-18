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
    public float speed = 4f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private GameObject player;
    private float pMoveX;
    private float pMoveZ;
    private float unclampedMoveX;
    private float unclampedMoveZ;
    private float dashingTimer;
    public float dashCooldown = 5f;
    private bool canDash = false;
    private float dashCoolCounter;
    private bool holdingForward;
    private bool holdingLeft;
    private bool holdingRight;
    private bool holdingBack;



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
        holdingLeft = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed;
        holdingRight = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed;
        holdingForward = Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed;
        holdingBack = Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed;
        bool isInputing = false;
        
        if (holdingBack||holdingRight||holdingForward||holdingLeft) isInputing = true;
        if (Keyboard.current.shiftKey.wasPressedThisFrame && canDash)
        {
            Dash();
            canDash = false;
            dashCoolCounter = 0;
        }
        //Jumping Logic
        if (Keyboard.current.spaceKey.wasPressedThisFrame && ((jumpCount < maxJumps)||canWallJump)) //only allow jump on keyboard pressed and if the jump count is less than the maxjumps
        {
            if (canWallJump)
            {   
                //Wall Jumping Logic
                if (holdingRight)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump(jumpForce*1.5f);
                    pMoveX *= -1;
                    unclampedMoveX = -speed*4;
                }
                else if (holdingLeft)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump(jumpForce*1.5f);
                    pMoveX *= -1;
                    unclampedMoveX = speed*4;
                }
                else if (holdingForward)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump(jumpForce*1.5f);
                    pMoveZ *= -1;
                    unclampedMoveZ = -speed*4;
                }
                else if (holdingBack)
                {
                    canWallJump = false;
                    jumpCount--;
                    Jump(jumpForce*1.5f);
                    pMoveZ *= -1;
                    unclampedMoveZ = speed*4;
                }
            }
            else if(jumpCount>1) Jump(jumpForce*2);
            else Jump(jumpForce);
        }
        if (!canDash)
        {
            dashCoolCounter+=Time.deltaTime;
            if (dashCoolCounter>=dashCooldown) canDash = true;
        }

        //make the player jump
        velocity.y += 2.35f*gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check WASD or Arrow keys using the new Input System
        if (holdingLeft) pMoveX += -speed*Time.deltaTime;
        if (holdingRight) pMoveX += speed*Time.deltaTime;
        if (holdingBack) pMoveZ += -speed*Time.deltaTime;
        if (holdingForward) pMoveZ += speed*Time.deltaTime;


        if (controller.isGrounded) //reset velocity and jump count here only while grounded and not moving horizontally
        {   
            unclampedMoveX = 0;
            unclampedMoveZ = 0;
            velocity.y = -2f;
            jumpCount = 0;
            if (pMoveX < 0f){
                if(!isInputing){
                    pMoveX -= 2*friction * Time.deltaTime;
                }
                pMoveX -= friction * Time.deltaTime;
            } 
            if (pMoveX > 0f){
                if(!isInputing){
                    pMoveX += 2*friction * Time.deltaTime;
                }
                pMoveX += friction * Time.deltaTime;
            } 
            if (pMoveZ > 0f){
                if(!isInputing){
                    pMoveZ += 2*friction * Time.deltaTime;
                }
                pMoveZ += friction * Time.deltaTime;
            } 
            if (pMoveZ < 0f){
                if(!isInputing){
                    pMoveZ -= 2*friction * Time.deltaTime;
                }
                pMoveZ -= friction * Time.deltaTime;
            } 
        if ((unclampedMoveX != 0 || unclampedMoveZ != 0)&&dashingTimer<=3)
        {
            if (unclampedMoveX < 0f){
                unclampedMoveX -=speed * Time.deltaTime;
            } 
            if (unclampedMoveX > 0f){
                unclampedMoveX += speed * Time.deltaTime;
            } 
            if (unclampedMoveZ > 0f){
                unclampedMoveZ += speed * Time.deltaTime;
            } 
            if (unclampedMoveZ < 0f){
                unclampedMoveZ -= speed * Time.deltaTime;
            } 
        }
        }

        // Clamps player speed to go no faster than max speed, which is determined by speed
        pMoveX = math.clamp(pMoveX,-topSpeed,topSpeed);
        pMoveZ = math.clamp(pMoveZ,-topSpeed,topSpeed);
        // Adds pMove to actual vector and normalizes it.
        move.x = pMoveX;
        move.z = pMoveZ;
        move = Vector3.ClampMagnitude(move,topSpeed);
        move.x += unclampedMoveX*Time.deltaTime*50;
        move.z += unclampedMoveZ*Time.deltaTime*50;

        // Move the player
        controller.Move(move * speed * Time.deltaTime);
    }
    void Jump(float jForce)
    {
        velocity.y = Mathf.Sqrt(jForce * -1f * gravity);
        jumpCount++;
        //Change direction if opposite direction is held while midair
        if (((move.x>0)&&(Keyboard.current.aKey.isPressed||Keyboard.current.leftArrowKey.isPressed))||(move.x<0&&(Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed))) pMoveX*=-1;
    }
    
    void Dash()
    {
        if (holdingRight) unclampedMoveX += 5;
        if (holdingLeft) unclampedMoveX -= 5;
        if (holdingForward||(!holdingRight&&!holdingLeft&&!holdingBack)) unclampedMoveZ += 5;
        if (holdingBack) unclampedMoveZ -= 5;
        while (dashingTimer < 3) dashingTimer += Time.deltaTime;
        dashingTimer = 0;

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