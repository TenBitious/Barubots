using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody myRigidbody;
    public float lifeTime = 1.0f;

    private float currentLifeTime;
    private bool isActive;

	// Use this for initialization
	void Awake ()
	{
	    myRigidbody = GetComponent<Rigidbody>();
	}

    public void Shoot(Vector3 force)
    {
        transform.parent = null;
        currentLifeTime = lifeTime;
        isActive = true;
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
}
