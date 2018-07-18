using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveAbility : MonoBehaviour {
    public GameObject shockWave;

	// Use this for initialization
	void Start () {
		
	}

    public void DoShockWave()
    {
        GameObject shockwave = Instantiate(shockWave, transform);
    }
}
