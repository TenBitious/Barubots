using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody myRigidbody;
    private Collider myCollider;
    public ParticleSystem onShootParticleSystem;
    public ParticleSystem onHitParticleSystem;
    public ParticleSystem afterFreezeParticleSystem;
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
    private float radius;
    private List<GameObject> hittedGameObjects = new List<GameObject>();

        // Use this for initialization
    void Awake ()
	{
        slowMotionTime = 0;
	    myCollider = GetComponent<SphereCollider>();
	    myCollider.enabled = false;
        myRigidbody = GetComponent<Rigidbody>();
        radius = GetComponent<SphereCollider>().radius;
	}

    private void FixedUpdate()
    {
        currentPosition = transform.position;

        positions.AddLast(transform.position);
        if (positions.Count > 2)
        {
            positions.RemoveFirst();
        }

        CheckIfNextFrameHitsAPlayer();
    }

    private void CheckIfNextFrameHitsAPlayer()
    {
        Vector3 forward = force.normalized;
        float length = (myRigidbody.velocity * Time.deltaTime).magnitude + radius;
        RaycastHit hit;

        Debug.DrawRay(transform.position, forward * length, Color.green);
        if (Physics.Raycast(transform.position, forward, out hit, length))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                if (hittedGameObjects.Contains(hit.collider.gameObject))
                {
                    return;
                }
                transform.position.Set(hit.point.x, hit.point.y, hit.point.z);
                Debug.Log("next frame hit player ");
                DoKnockBack(hit.collider);
                hittedGameObjects.Add(hit.collider.gameObject);
                // Destroy(GetComponent("Rigidbody"));
            }
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
        onShootParticleSystem.Play(true);
        myRigidbody.AddForce(force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision col)
    {
        if (hittedGameObjects.Contains(col.gameObject))
        {
            return;
        }
        if (col.transform.tag == "Player")
        {
            Debug.Log("On PLayer hit");
            foreach (ContactPoint contact in col.contacts)
            {
                Debug.Log(contact);
                //transform.position.Set(contact.point.x, contact.point.y, contact.point.z);
            }
            DoKnockBack(col.collider);
            return;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            force = myRigidbody.velocity;
        }
    }

    private void DoKnockBack(Collider col)
    {
        ParticleSystem particleSystem = Instantiate(onHitParticleSystem, transform.position, transform.rotation);
        particleSystem.Play(true);
        Destroy(particleSystem.gameObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);

        Robot robot = col.transform.GetComponent<Robot>();
        robot.GetHit(force.normalized, damage, force.magnitude * knockBackMultiplier, chargeForce);
        myRigidbody.angularVelocity = Vector3.zero;
        myRigidbody.velocity = Vector3.zero;

        StartCoroutine(AddForce(robot));
    }

    IEnumerator AddForce(Robot robot)
    {
        float slowMoDuration = GameManager.Instance.MaxSlowMotionDuration * chargeForce;
        ParticleSystem particleSystem = Instantiate(afterFreezeParticleSystem, transform.position, transform.rotation);
        yield return new WaitForSeconds(slowMoDuration * 0.8f);

        particleSystem.Play(true);
        Destroy(particleSystem.gameObject, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        yield return new WaitForSeconds(slowMoDuration * 0.2f);

        myRigidbody.AddForce(force.normalized * 100 , ForceMode.Force);
        myRigidbody.useGravity = true;
        Destroy(gameObject);
    }

    public void SetChargeForce(float chargeForce)
    {
        this.chargeForce = chargeForce;
    }
}
