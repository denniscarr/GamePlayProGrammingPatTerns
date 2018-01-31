using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachEnemy : Enemy {

    [SerializeField] GameObject meshParent;

    protected override void Spawn() {
    }

    protected override void Move() {
        SteerTowards(PlayerController.m_Transform.position);
    }

    protected override void GetStunned() {
        base.GetStunned();
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

    protected override void Die() {
    }
}
