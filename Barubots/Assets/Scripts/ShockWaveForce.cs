using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveForce : MonoBehaviour {

	public float radius = 4;
	public float force = 5f;
	public ParticleSystem postShockFX;
	Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        ShockWave();
	}

	void ShockWave()
	{
		Vector3 pos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        Debug.Log(colliders.Length);
		foreach(Collider col in colliders){
            if (col.tag == "Player")
            {
                if (col.gameObject == transform.parent.parent.gameObject)
                {
                    continue;
                }

                col.GetComponent<Robot>().GetHit((col.transform.position - transform.position).normalized, 0, force, 1);
            } else if (col.tag == "Projectile")
            {
                Debug.Log("Projectile found");
                col.GetComponent<Projectile>().GetHit(1, (col.transform.position - transform.position).normalized * force * 10);
            }
			//rigidbody = col.GetComponent<Rigidbody>();
			//if(rigidbody!=null){
			//	rigidbody.AddExplosionForce(force, pos, radius);
			//}
		}
	}
	
	// Update is called once per frame
	void Update () {
        if (!postShockFX.isPlaying)
        {
            Destroy(gameObject);
        }
    }

}
