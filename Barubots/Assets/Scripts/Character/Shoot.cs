using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Projectile ball;

    public AnimationCurve chargeCurve;
    public Material chargeMaterial;
    public float maxChargeTime;
    public float maxShootPower = 30f;

    private Robot myRobot;
    private float currentChargeTime;
    private bool isCharging;
    // Use this for initialization
    void Start ()
    {
        myRobot = GetComponent<Robot>();
    }

	// Update is called once per frame
	void Update () {
	    if (isCharging)
	    {
	        currentChargeTime += Time.deltaTime;
	        chargeMaterial.SetFloat("_AlphaValue", 1 - (currentChargeTime/maxChargeTime));
        }
	}

    public void ShootStart()
    {
        Debug.Log("On ShootStart");
        isCharging = true;
        currentChargeTime = 0f;
    }

    public void ShootRelease()
    {
        myRobot.Player.SetVibration(0, 1.0f, 0.5f);
        chargeMaterial.SetFloat("_AlphaValue", 1);
        Debug.Log("On ShootRelease");
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward / 4), transform.rotation, transform);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        float chargeForce = chargeCurve.Evaluate((Mathf.Clamp(currentChargeTime, 0f, maxChargeTime) / maxChargeTime));

        Debug.Log("chargeForce: " + chargeForce + " / currentChargeTime: " + currentChargeTime);
        CameraShake.instance.shakeDuration = chargeForce * 0.25f;
        myRobot.TotalMoveVector += -transform.forward * chargeForce * 20f;
        projectile.Shoot(transform.forward * (maxShootPower * chargeForce));

        isCharging = false;
        currentChargeTime = 0f;
    }
}
