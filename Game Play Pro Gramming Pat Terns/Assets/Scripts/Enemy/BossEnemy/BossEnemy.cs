using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossEnemy : Enemy {

    [SerializeField] BossArm[] arms;
    [SerializeField] GameObject colliderParent;
    [SerializeField] GameObject triggersParent;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Rotator armRotator;
    [SerializeField] FloatRange rotateSpeedRange = new FloatRange(7f, 15f);

    StateMachine<BossEnemy> stateMachine;
    protected override Collider[] m_Colliders { get { return colliderParent.GetComponentsInChildren<Collider>(); } }
    private Collider[] m_Triggers { get { return triggersParent.GetComponentsInChildren<Collider>(); } }
    private List<SkinnedMeshRenderer> m_SkinnedMeshRenderers = new List<SkinnedMeshRenderer>();


    public override void Initialize() {

        foreach(BossArm arm in arms) {
            m_SkinnedMeshRenderers.Add(arm.armMesh);
        }

        stateMachine = new StateMachine<BossEnemy>(this);
        stateMachine.TransitionTo<RotateState>();

        base.Initialize();
    }


    public override void Run() {
        switch (m_State) {
            case State.Normal:
                foreach (BossArm arm in arms) { arm.Run(); }
                stateMachine.Update();
                break;
            case State.Stunned:
                RunStunTimer();
                break;
            case State.Knockback:
                break;
            case State.Spawning:
                break;
        }
    }


    protected override void Move() {}


    protected override void SetRenderersAlpha(float value) {
        foreach(SkinnedMeshRenderer meshRenderer in m_SkinnedMeshRenderers) {
            Color newColor = originalColor;
            newColor.a = value;
            meshRenderer.material.color = newColor;
        }
    }


    protected override void SetCollidersEnabled(bool value) {
        base.SetCollidersEnabled(value);
        foreach(Collider trigger in m_Triggers) { trigger.enabled = value; }
    }


    protected override void MemorizeOriginalColor() {
        if (m_SkinnedMeshRenderers.Count > 0) {
            originalColor = m_SkinnedMeshRenderers[0].material.color;
        }
    }


    protected override void FadeRenderers(Color targetColor, float duration) {
        foreach (SkinnedMeshRenderer meshRenderer in m_SkinnedMeshRenderers) { meshRenderer.material.DOColor(targetColor, duration); }
    }


    public void MoveAimlessly() {
        m_Rigidbody.AddForce(UnityEngine.Random.insideUnitCircle * accelerateSpeed, ForceMode.Acceleration);

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }


    public void SetArmRotationSpeed(int deadArms) {
        float newSpeed = MyMath.Map(deadArms, 0, 6, rotateSpeedRange.min, rotateSpeedRange.max);
        armRotator.relativeSpeed = new Vector3(0f, newSpeed, 0f);
    }


    public void Shoot() {
        foreach (BossArm arm in arms) {
            arm.Shoot(bulletPrefab);
        }
    }



    // STATES:

    // In case I want any functionality which applies to all classes, you know being prepared thinking ahead. I was a boy scout.
    private abstract class BossEnemyState : StateMachine<BossEnemy>.State {}


    private class RotateState : BossEnemyState {

        private FloatRange howOftenToShootRange = new FloatRange(0.9f, 0.2f);
        private float howOftenToShoot = 1f;
        float shootTimer = 0f;

        public override void Update() {

            Context.MoveAimlessly();

            shootTimer += Time.deltaTime;
            if (shootTimer >= howOftenToShoot) {
                Context.Shoot();
                shootTimer = 0f;
            }

            // If arms are all busted, transition to kill state.
            int deadArms = 0;
            for (int i = 0; i < Context.arms.Length; i++) {
                if (Context.arms[i].iAmAStump) { deadArms++; }
            }

            Context.SetArmRotationSpeed(deadArms);
            howOftenToShoot = MyMath.Map(deadArms, 0, 5, howOftenToShootRange.min, howOftenToShootRange.max);

            if (deadArms == 6) { TransitionTo<ChaseState>(); }
        }
    }


    private class ChaseState : BossEnemyState {

        public override void Update() {
            Debug.Log(":-)");
        }
    }
}
