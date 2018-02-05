using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachEnemy : Enemy {

    [SerializeField] GameObject meshParent;
    [SerializeField] float maxSineAngle = 15f;
    [SerializeField] float sineSpeed = 1f;
    float currentSineValue = 0f;

    private void Start() {
        currentSineValue = Random.Range(-100f, 100f);
    }

    protected override void Update() {
        base.Update();
        currentSineValue += sineSpeed * Time.deltaTime;
    }

    protected override void Spawn() {
    }

    protected override void Move() {
        Vector3 moveForce = SteerTowards(PlayerController.m_Transform.position);
        moveForce = Quaternion.Euler(0f, 0f, MyMath.Map(Mathf.Sin(currentSineValue), -1f, 1f, -maxSineAngle, maxSineAngle)) * moveForce;
        m_Rigidbody.AddForce(moveForce * m_Stats.accelerateSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    protected override void Stunned() {
        base.Stunned();
        foreach (Rotator rotator in meshParent.GetComponentsInChildren<Rotator>()) {
            rotator.freezeRotation = true;
        }
    }

    protected override void RecoverFromStun() {
        base.RecoverFromStun();
        foreach (Rotator rotator in meshParent.GetComponentsInChildren<Rotator>()) {
            rotator.freezeRotation = false;
        }
    }
}
