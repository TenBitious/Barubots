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
        if (col.transform.tag == "Player")
        {
            col.transform.GetComponent<Robot>().GetHit(transform.position, damage, knockBack);
        }

        Destroy(gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        Debug.Log("Has NO collision with: " + col.transform.name);
    }
}
