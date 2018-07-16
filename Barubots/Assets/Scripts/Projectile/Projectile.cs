using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody myRigidbody;
    private Collider myCollider;
    public ParticleSystem particlePrefab;
    public float lifeTime = 1.0f;
    public float damage = 20f;
    public float knockBackMultiplier = 1.2f;

    public AnimationCurve slowMotion;
    private float slowMotionTime;
    private Vector3 force;
    private float chargeForce = 1;
    private float currentLifeTime;
    private bool isActive;
    private Vector3 oldPosition;

    private LinkedList<Vector3> positions = new LinkedList<Vector3>();
    private Vector3 currentPosition; 


        // Use this for initialization
    void Awake ()
	{
        slowMotionTime = 0;
	    myCollider = GetComponent<Collider>();
	    myCollider.enabled = false;
        myRigidbody = GetComponent<Rigidbody>();
	}

    private void FixedUpdate()
    {
        currentPosition = transform.position;

        positions.AddLast(transform.position);
        if (positions.Count > 2)
        {
            positions.RemoveFirst();
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (currentLifeTime < 0)
        {
            Destroy(gameObject);
            return;
        }

        currentLifeTime -= Time.deltaTime;
    }

    public void Shoot(Vector3 force)
    {
        Debug.Log(force.magnitude);
        this.force = force;
        transform.parent = null;
        myCollider.enabled = true;
        currentLifeTime = lifeTime;
        isActive = true;
        particlePrefab.Play(true);
        myRigidbody.AddForce(force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
        {
            DoKnockBack(col);
  

            //Destroy(gameObject);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            force = myRigidbody.velocity;
        }
    }

    private void DoKnockBack(Collision col)
    {
        Robot robot = col.transform.GetComponent<Robot>();
        robot.GetHit(force.normalized, damage, force.magnitude * knockBackMultiplier, chargeForce);
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.velocity = Vector3.zero;

        StartCoroutine(AddForce(robot));
    }

    IEnumerator AddForce(Robot robot)
    {
        yield return new WaitForSeconds(GameManager.Instance.MaxSlowMotionDuration * chargeForce);
        myRigidbody.AddForce((positions.First.Value - robot.GetPreviousPosition()).normalized * 100 , ForceMode.Force);
    }

    public void SetChargeForce(float chargeForce)
    {
        this.chargeForce = chargeForce;
    }
}
