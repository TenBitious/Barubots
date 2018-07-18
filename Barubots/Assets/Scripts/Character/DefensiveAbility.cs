using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveAbility : MonoBehaviour {
    public GameObject shockWave;
    private bool cooldown = false;
    public float cooldownDuration = 3f;

	// Use this for initialization
	void Start () {
		
	}

    public void DoShockWave()
    {
        if (!cooldown)
        {
            GameObject shockwave = Instantiate(shockWave, transform);
            StartCoroutine(StartCooldown());
        }
    }

    IEnumerator StartCooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        cooldown = false;
    }
}
