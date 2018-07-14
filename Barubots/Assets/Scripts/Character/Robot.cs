﻿// MyCharacter.cs - A simple example showing how to get input from Rewired.Player

using UnityEngine;
using System.Collections;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class Robot : MonoBehaviour
{
    public int playerId = 0; // The Rewired player id of this character

    public AnimationCurve startUpCurve;
    public float moveSpeed = 3.0f;
    public Vector3 drag;
    public float shootForce = 40;

    private Player player; // The Rewired Player
    private CharacterController cc;
    private Vector3 moveVector;
    private Vector3 rotateVector;
    private Vector3 gravityVector;

    private bool fire;
    private float startUpTime = 0f;
    private Shoot shootComponent;
    private Vector3 totalMoveVector;

    void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);

        // Get the character controller
        cc = GetComponent<CharacterController>();
        
        shootComponent = GetComponent<Shoot>();
        // Get Physics gravity
        gravityVector = Physics.gravity;
    }

    void Update()
    {
        GetInput();

        CalculateMovement();
        CalculateGravity();
        ApplyRotation();
        ApplyFire();

        ApplyDrag();
        ApplyMove();
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

        moveVector.x = player.GetAxis("move_horizontal"); // get input by name or action id
        moveVector.z = player.GetAxis("move_vertical");

        rotateVector.x = player.GetAxis("rotate_horizontal");
        rotateVector.z = player.GetAxis("rotate_vertical");

        fire = player.GetButtonDown("fire");
    }

    private void CalculateGravity()
    {
        totalMoveVector += gravityVector * Time.deltaTime;
    }

    // Process movement input
    private void CalculateMovement()
    {
        // Reset startUpTime if moveVector is zero.
        if (moveVector.magnitude == 0)
        {
            startUpTime = 0;
        }
        else
        {
            startUpTime += Time.deltaTime;
        }

        // Process movement
        if (moveVector.x != 0.0f || moveVector.z != 0.0f)
        {
            totalMoveVector += moveVector * moveSpeed * Time.deltaTime * startUpCurve.Evaluate(startUpTime);
            // cc.Move(moveVector * moveSpeed * Time.deltaTime * startUpCurve.Evaluate(startUpTime));
        }
    }
 
    private void ApplyFire()
    {
        if (fire)
        {
            shootComponent.ShootProjectile();
            CameraShake.instance.shakeDuration = 0.05f;
            totalMoveVector += -transform.forward * shootForce;
        }
    }
    
    // Process rotation input
    private void ApplyRotation()
    {
        // Check if the right joystick is zero or not
        if (rotateVector.y != 0.0f || rotateVector.z != 0.0f)
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, rotateVector, 0.5f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }
    private void ApplyDrag()
    {
        totalMoveVector.x /= 1 + drag.x * Time.deltaTime;
        totalMoveVector.y /= 1 + drag.y * Time.deltaTime;
        totalMoveVector.z /= 1 + drag.z * Time.deltaTime;
    }

    private void ApplyMove()
    {
        cc.Move(totalMoveVector * Time.deltaTime);
    }
}