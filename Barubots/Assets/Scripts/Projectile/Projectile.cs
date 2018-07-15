using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody myRigidbody;
    private Collider myCollider;
    public ParticleSystem particlePrefab;
    public float lifeTime = 1.0f;
    public float damage = 10f;
    public float knockBack = 100f;

    public AnimationCurve slowMotion;
    private float slowMotionTime;
    private Vector3 direction;

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
        direction = force;
        transform.parent = null;
        myCollider.enabled = true;
        currentLifeTime = lifeTime;
        isActive = true;
        particlePrefab.Play();
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
            direction = myRigidbody.velocity;
        }
    }

    private void DoKnockBack(Collision col)
    {
        Robot robot = col.transform.GetComponent<Robot>();
        robot.GetHit(direction.normalized, damage, knockBack);
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.velocity = Vector3.zero;

        StartCoroutine(AddForce(robot));
    }

    IEnumerator AddForce(Robot robot)
    {
        yield return new WaitForSeconds(0.15f);
        myRigidbody.AddForce((positions.First.Value - robot.GetPreviousPosition()).normalized * 100 , ForceMode.Force);
    }

    void OnCollisionExit(Collision col)
    {

    }
}
