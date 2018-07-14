using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{

    public Projectile ball;
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShootProjectile()
    {
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward/4), transform.rotation, transform);
        Physics.IgnoreCollision(GetComponent<Collider>(), projectile.GetComponent<Collider>());
        // TODO: De-ignore when we will use pickup instead of Instantiate
        projectile.Shoot(transform.forward * 10f);
    }
}
