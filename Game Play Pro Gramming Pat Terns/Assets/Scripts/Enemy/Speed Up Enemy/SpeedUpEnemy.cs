using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpEnemy : Enemy {

    [SerializeField] float speedIncrease;
    [SerializeField] float accelerationIncrease;

    public override void Initialize() {
        base.Initialize();
        GameEventManager.instance.Subscribe<GameEvents.EnemyStunned>(OnEnemyStunned);
    }

    protected override void Move() {
        Vector3 moveForce = SteerTowards(PlayerController.m_Transform.position);
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

    void OnEnemyStunned(GameEvent gameEvent) {
        Debug.Log("Increasing Speed");
        maxSpeed += speedIncrease;
        accelerateSpeed += accelerationIncrease;
    }
}
