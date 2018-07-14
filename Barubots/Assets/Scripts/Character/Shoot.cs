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
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward/2), transform.rotation, transform);
        projectile.Shoot(transform.forward * 25f);
    }
}
