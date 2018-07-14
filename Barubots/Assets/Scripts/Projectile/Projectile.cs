using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody myRigidbody;
    private Collider myCollider;
    public ParticleSystem particlePrefab;
    public float lifeTime = 1.0f;

    private float currentLifeTime;
    private bool isActive;

	// Use this for initialization
	void Awake ()
	{
	    myCollider = GetComponent<Collider>();
	    myCollider.enabled = false;
        myRigidbody = GetComponent<Rigidbody>();
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
	
	// Update is called once per frame
	void Update ()
	{
	    if (!isActive) return;

	    if (currentLifeTime < 0)
	    {
            Destroy(gameObject);
	        return;
	    }

	    currentLifeTime -= Time.deltaTime;
	}

    void OnCollisionEnter(Collision col)
    {
    }

    void OnCollisionExit(Collision col)
    {
    }
}
