using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChargeInfo
{
    public float power;
    public float knockback;
    public float chargeTimer;
    public Color indicatorColor;
}

public class Shoot : MonoBehaviour
{
    public ChargeInfo[] chargeInfo;
    public Projectile ball;

    public AnimationCurve chargeCurve;
    public ChargeIndicator chargeIndicator;
    public float maxChargeTime;
    public float maxShootPower = 30f;
    public AnimationCurve knockbackCurve;
    public float shootKnockBackDistance = 40;
    public float cameraShakeForce = 0.05f;

    private int currentChargeCycle;
    private Robot myRobot;
    private float currentChargeTime;
    private bool isCharging;

    private bool maxCharged;
    // Use this for initialization
    void Start ()
    {
        currentChargeCycle = 0;
        myRobot = GetComponent<Robot>();
        SetIndicatorColors();
    }

    void SetIndicatorColors()
    {
        Vector4[] colorsAsVector4 = new Vector4[chargeInfo.Length];
        for (int i = 0; i < chargeInfo.Length; i++)
        {
            colorsAsVector4[i] = chargeInfo[i].indicatorColor;
        }
        chargeIndicator.SetColors(colorsAsVector4);
    }
	// Update is called once per frame
	void Update () {
	    if (isCharging)
	    {
	        if (maxCharged) return;

	        currentChargeTime += Time.deltaTime;
	        if (currentChargeTime >= chargeInfo[currentChargeCycle].chargeTimer) GoToNextCycle();

            chargeIndicator.SetValue(currentChargeTime/chargeInfo[currentChargeCycle].chargeTimer);
        }
	}

    public void GoToNextCycle()
    {
        if (currentChargeCycle >= chargeInfo.Length-1)
        {
            maxCharged = true;
            // TODO: Start max charge stuff
            return;
        }
        currentChargeCycle++;
        currentChargeTime = 0;
        chargeIndicator.GoTeNextCycle(currentChargeCycle);
    }

    public void ShootStart()
    {
        isCharging = true;
        currentChargeTime = 0f;
    }

    public void ShootRelease()
    {
        myRobot.Player.SetVibration(0, 1.0f, 0.5f);
        // Instatiate the projectile
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward / 4), transform.rotation, transform);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        float chargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime) / maxChargeTime;
        float chargeForce = chargeCurve.Evaluate(chargeTime);
        CameraShake.instance.shakeDuration = chargeForce * cameraShakeForce;
        projectile.Shoot(transform.forward * (maxShootPower * chargeForce));
        projectile.SetChargeForce(chargeTime);
        ApplyKnockBack();

        // Set default values
        isCharging = false;
        maxCharged = false;
        currentChargeCycle = 0;
        currentChargeTime = 0f;
        chargeIndicator.Reset();
    }

    void ApplyKnockBack()
    {
        float knockBackForce = knockbackCurve.Evaluate((Mathf.Clamp(currentChargeTime, 0f, maxChargeTime) / maxChargeTime));
        myRobot.TotalMoveVector += -transform.forward * knockBackForce * shootKnockBackDistance;
    }
}
