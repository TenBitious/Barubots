// MyCharacter.cs - A simple example showing how to get input from Rewired.Player

using UnityEngine;
using System.Collections;
using Rewired;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Robot : MonoBehaviour
{

    public int playerId = 0; // The Rewired player id of this character

    public float moveSpeed = 3.0f;
    public float bulletSpeed = 15.0f;
    public GameObject bulletPrefab;

    private Player player; // The Rewired Player
    private CharacterController cc;
    private Vector3 moveVector;
    private Vector3 gravityVector;
    private bool fire;

    void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
        Debug.Log("Awake0");
        // Debug.Log("awake");
        // Get the character controller
        cc = GetComponent<CharacterController>();

        gravityVector = Physics.gravity;
    }

    void Update()
    {
        GetInput();
        ProcessInput();

        ApplyGravity();
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

        moveVector.x = player.GetAxis("move_horizontal"); // get input by name or action id
        moveVector.z = player.GetAxis("move_vertical");
        fire = player.GetButtonDown("fire");
    }

    private void ApplyGravity()
    {
        cc.Move(gravityVector * Time.deltaTime);
    }

    private void ProcessInput()
    {
        // Process movement
        if (moveVector.x != 0.0f || moveVector.y != 0.0f)
        {
            cc.Move(moveVector * moveSpeed * Time.deltaTime);
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveVector, 0.1f, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        // Process fire
        if (fire)
        {
            Debug.Log("Fire");
         //   GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + transform.right, transform.rotation);
         //   bullet.GetComponent<Rigidbody>().AddForce(transform.right * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}