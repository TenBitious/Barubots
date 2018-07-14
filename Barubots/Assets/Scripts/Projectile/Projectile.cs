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
    }

    private void DoKnockBack(Collision col)
    {
        Robot robot = col.transform.GetComponent<Robot>();
        robot.GetHit(positions.Last.Value, damage, knockBack);
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.velocity = Vector3.zero;

        StartCoroutine(AddForce(robot));
    }

    IEnumerator AddForce(Robot robot)
    {
        yield return new WaitForSeconds(0.15f);
        myRigidbody.AddForce((positions.First.Value - robot.GetPreviousPosition()).normalized * 100 , ForceMode.Force);
        //while (true)
        //{
        //    myRigidbody.AddForce((positions.First.Value - robot.GetPreviousPosition()).normalized * 10 * slowMotion.Evaluate(slowMotionTime), ForceMode.Force);
        //    slowMotionTime += Time.deltaTime;   
        //    Debug.Log(slowMotionTime);
        //    yield return new WaitForFixedUpdate();
        //}
    }

    void OnCollisionExit(Collision col)
    {
    }
}
