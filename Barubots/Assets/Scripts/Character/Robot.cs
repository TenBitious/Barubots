// MyCharacter.cs - A simple example showing how to get input from Rewired.Player

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rewired;

[RequireComponent(typeof(CharacterController))]
public class Robot : MonoBehaviour
{
    public enum PlayerId {
        Player0 = 0,
        Player1 = 1,
        Player2 = 2,
        Player3 = 3,
    }
    public PlayerId playerId = 0; // The Rewired player id of this character

    [Header("Movement")]
    public AnimationCurve acceleration;
    public float moveSpeed = 100f;
    public Vector3 drag = new Vector3(18, 0, 18);
    public float shootKnockBackDistance = 40;

    [Header("Ground")]
    public LayerMask Ground;
    public float groundDistance = 0.2f;

    public Vector3 TotalMoveVector
    {
        get { return totalMoveVector; }
        set { totalMoveVector = value; }
    }

    public AnimationCurve slowMotion;
    public Player Player
    {
        get { return player; }
        set { player = value; }
    }

    private Player player; // The Rewired Player
    private CharacterController cc;
    private Vector3 moveVector;
    private Vector3 rotateVector;
    private Vector3 gravityVector;



    private float damagePercentage;
    private bool fire;
    private bool fireDown;
    private bool fireUp;
    private float startUpTime = 0f;
    private Shoot shootComponent;
    private bool isGrounded = true;
    private Vector3 totalMoveVector;
    private Transform groundChecker;

    private Vector3 currentPosition;
    private LinkedList<Vector3> positions = new LinkedList<Vector3>();

    private float slowMotionTimer;


    void Awake()
    {
        damagePercentage = 0;
        slowMotionTimer = 1;
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer((int)playerId);
        groundChecker = transform.Find("GroundChecker");

        // Get the character controller
        cc = GetComponent<CharacterController>();
        
        shootComponent = GetComponent<Shoot>();
        // Get Physics gravity
        gravityVector = Physics.gravity;
    }

    void FixedUpdate()
    {
        GetInput();

        UpdateSlowMotionTimer();

        CheckIfGrounded();

        CalculateMovement();
        CalculateGravity();
        ApplyRotation();
        ApplyFire();

        ApplyDrag();
        ApplyMove();

        UpdatePositions();
    }

    private void UpdatePositions()
    {
        currentPosition = transform.position;

        positions.AddLast(transform.position);
        if (positions.Count > 2)
        {
            positions.RemoveFirst();
        }
    }

    private void UpdateSlowMotionTimer()
    {
        this.slowMotionTimer += Time.deltaTime;
    }

    private void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

        moveVector.x = player.GetAxis("move_horizontal"); // get input by name or action id
        moveVector.z = player.GetAxis("move_vertical");

        rotateVector.x = player.GetAxis("rotate_horizontal");
        rotateVector.z = player.GetAxis("rotate_vertical");

        fireDown = player.GetButtonDown("fire");
        fireUp = player.GetButtonUp("fire");
    }

    private void CheckIfGrounded()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, Ground, QueryTriggerInteraction.Ignore);
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
            totalMoveVector += moveVector * moveSpeed * Time.deltaTime * acceleration.Evaluate(startUpTime);
            // cc.Move(moveVector * moveSpeed * Time.deltaTime * startUpCurve.Evaluate(startUpTime));
        }
    }
 
    private void ApplyFire()
    {
        if (fireDown)
        {
            shootComponent.ShootStart();
        } else if (fireUp)
        {
            shootComponent.ShootRelease();
            totalMoveVector += -transform.forward * shootKnockBackDistance;
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
        totalMoveVector.x /= 1 + (drag.x * Time.deltaTime) * slowMotion.Evaluate(slowMotionTimer);
        totalMoveVector.y /= 1 + (drag.y * Time.deltaTime) * slowMotion.Evaluate(slowMotionTimer);
        totalMoveVector.z /= 1 + (drag.z * Time.deltaTime) * slowMotion.Evaluate(slowMotionTimer);
    }

    private void ApplyMove()
    {
        cc.Move(totalMoveVector * Time.deltaTime * slowMotion.Evaluate(slowMotionTimer));
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //Debug.Log(hit.transform.name);
        //if (body == null || body.isKinematic)
        //    return;

        //if (hit.moveDirection.y < -0.3F)
        //    return;

        //Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        //body.velocity = pushDir * pushPower;
    }

    private void ApplyForce(Vector3 position, float damage, float knockBack)
    {
        Vector3 direction = (positions.First.Value - position).normalized;
        Vector3 knockBackDistance = knockBack * direction * (1 + damagePercentage / 100);     
        totalMoveVector += knockBackDistance;
    }

    public void GetHit(Vector3 position, float damage, float knockBack)
    {
        //CameraShake.instance.shakeDuration = 0.3f;
        ApplyForce(position, damage, knockBack);
        DoDamage(damage);

        slowMotionTimer = 0;
    }

    private void DoDamage(float damage)
    {
        damagePercentage += damage;
    }

    public Vector3 GetPreviousPosition()
    {
        return positions.First.Value;
    }
}