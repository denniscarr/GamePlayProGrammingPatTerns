using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachEnemy : Enemy {

    [SerializeField] float maxSineAngle = 15f;
    [SerializeField] float sineSpeed = 1f;
    float currentSineValue = 0f;

    public override void Initialize() {
        base.Initialize();
        currentSineValue = Random.Range(-100f, 100f);

        foreach(Rotator rotator in meshParent.GetComponentsInChildren<Rotator>()) {
            rotator.transform.rotation = Random.rotation;
            rotator.relativeSpeed = new Vector3(
                    rotator.relativeSpeed.x + Random.Range(-0.1f, 0.1f),
                    rotator.relativeSpeed.y + Random.Range(-0.1f, 0.1f),
                    rotator.relativeSpeed.z + Random.Range(-0.1f, 0.1f)
                );
        }
    }

    public override void Run() {
        base.Run();
        currentSineValue += sineSpeed * Time.deltaTime;
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
