using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Projectile ball;

    public AnimationCurve chargeCurve;
    public ChargeIndicator chargeIndicator;
    public float maxChargeTime;
    public float maxShootPower = 30f;
    public AnimationCurve knockbackCurve;
    public float shootKnockBackDistance = 40;
    public float cameraShakeForce = 0.05f;

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
	        chargeIndicator.SetValue(currentChargeTime/maxChargeTime);
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
        // Instatiate the projectile
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward / 4), transform.rotation, transform);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        float chargeForce = chargeCurve.Evaluate((Mathf.Clamp(currentChargeTime, 0f, maxChargeTime) / maxChargeTime));
        CameraShake.instance.shakeDuration = chargeForce * cameraShakeForce;
        projectile.Shoot(transform.forward * (maxShootPower * chargeForce));

        ApplyKnockBack();

        // Set default values
        chargeIndicator.SetValue(0);
        isCharging = false;
        currentChargeTime = 0f;
    }

    void ApplyKnockBack()
    {
        float knockBackForce = knockbackCurve.Evaluate((Mathf.Clamp(currentChargeTime, 0f, maxChargeTime) / maxChargeTime));
        myRobot.TotalMoveVector += -transform.forward * knockBackForce * shootKnockBackDistance;
    }
}
