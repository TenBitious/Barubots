using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChargeInfo
{
    public float projectileSpeed;
    public float shooterKnockback;
    public float chargeTimer;
    public float walkSpeedReduction;
    public float vibrationAtReleaseDuration;
    public float vibrationAtReleasePower;
    public float vibrationAtChargeDuration;
    public float vibrationAtChargePower;
    public float cameraShakeDurationAtShot;
    public Color indicatorColor;
}

public class Shoot : MonoBehaviour
{
    public ChargeInfo[] chargeInfo;
    public Projectile ball;
    
    public ChargeIndicator chargeIndicator;
    public float cameraShakeForce = 0.05f;

    private int currentChargeCycle;
    private Robot myRobot;
    private float currentChargeTime;
    private bool isCharging;

    private bool maxCharged;

    private float slowReduction;
    public float SlowReduction
    {
        get { return 1.0f - (slowReduction/100); }
    }
    // Use this for initialization
    void Start ()
    {
        currentChargeCycle = 0;
        myRobot = GetComponent<Robot>();
        SetIndicatorColors();
    }

    private void SetIndicatorColors()
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

	        chargeIndicator.SetValue(currentChargeTime / chargeInfo[currentChargeCycle].chargeTimer);
            if (currentChargeTime >= chargeInfo[currentChargeCycle].chargeTimer) GoToNextCycle();
        }
	}

    private void GoToNextCycle()
    {
        myRobot.Player.SetVibration(0, chargeInfo[currentChargeCycle].vibrationAtChargePower, chargeInfo[currentChargeCycle].vibrationAtChargeDuration);
        slowReduction = chargeInfo[currentChargeCycle].walkSpeedReduction;
        if (currentChargeCycle >= chargeInfo.Length-1)
        {
            currentChargeCycle++;
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
        if (currentChargeCycle < 1)
        {
            ResetCharge();
            return;
        }

        myRobot.Player.SetVibration(0, chargeInfo[currentChargeCycle - 1].vibrationAtReleasePower,
            chargeInfo[currentChargeCycle - 1].vibrationAtReleaseDuration);
        // Instatiate the projectile
        Projectile projectile = Instantiate(ball, (transform.position + transform.forward / 4), transform.rotation, transform);
        Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());

        // CameraShake.instance.shakeDuration = chargeForce * cameraShakeForce;
        projectile.Shoot(transform.forward * chargeInfo[currentChargeCycle -1 ].projectileSpeed);
        projectile.SetChargeForce(currentChargeCycle);

        ApplyKnockBack();
        ResetCharge();
    }

    /// <summary>
    /// Sets default value of charge properties
    /// </summary>
    private void ResetCharge()
    {
        // Set default values
        isCharging = false;
        maxCharged = false;
        slowReduction = 0;
        currentChargeCycle = 0;
        currentChargeTime = 0f;
        chargeIndicator.Reset();
    }

    void ApplyKnockBack()
    {
        myRobot.TotalMoveVector += -transform.forward * chargeInfo[currentChargeCycle-1].shooterKnockback;
    }
}
