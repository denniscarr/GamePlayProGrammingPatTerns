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

    [HideInInspector] public bool isCoreVulnerable = false;

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
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
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
        foreach(Collider trigger in m_Triggers) {
            //if (!(trigger is SphereCollider)) {
                trigger.enabled = value;
            //}
        }
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

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
    }


    public void SetArmRotationSpeed(float newSpeed) {
        armRotator.relativeSpeed = new Vector3(0f, newSpeed, 0f);
    }


    public void Shoot() {
        foreach (BossArm arm in arms) {
            arm.Shoot(bulletPrefab);
        }
    }


    //void OnReportedTriggerEnter(Collider other) {
    //    OnTriggerEnter(other);
    //}


    public override void GetHitByBullet(PlayerBullet bullet) {
        if (!isCoreVulnerable) { return; }

        StartCoroutine(BulletHitSequence());

        if (m_State == State.Stunned) {
            bullet.ShowStunnedHitParticles();
        } else {
            bullet.ShowHitParticles();
        }
    }


    public override void GetHitByCharge(PlayerCharge charge, Vector3 chargeHitPoint) {
        if (!isCoreVulnerable) { return; }

        Debug.Log("boss enemy hit by charge.");

        charge.ShowHitParticles(chargeHitPoint);
        StartCoroutine(GetHitByChargeCoroutine());
    }


    protected override IEnumerator BulletHitSequence() {

        m_Animator.SetTrigger("Hurt Trigger");

        switch (m_State) {
            case (State.Normal):
                //m_Rigidbody.velocity = Vector3.zero;
                currentHealth--;
                freezeHealthRecharge = true;
                healthRechargeTimer = 0;

                yield return new WaitForSeconds(0.1f);

                freezeHealthRecharge = false;
                break;

            case (State.Stunned):
                stunTimer -= 0.1f;
                break;
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

            float newSpeed = MyMath.Map(deadArms, 0, 6, Context.rotateSpeedRange.min, Context.rotateSpeedRange.max);
            Context.SetArmRotationSpeed(newSpeed);

            howOftenToShoot = MyMath.Map(deadArms, 0, 5, howOftenToShootRange.min, howOftenToShootRange.max);

            if (deadArms == 6) { TransitionTo<ChaseState>(); }
        }
    }


    private class ChaseState : BossEnemyState {

        public override void OnEnter() {
            base.OnEnter();

            Context.SetArmRotationSpeed(200f);
            Context.accelerateSpeed = 30f;
            Context.maxSpeed = 100f;

            Context.isCoreVulnerable = true;
        }

        public override void Update() {
            Vector3 moveForce = Context.SteerTowards(PlayerController.m_Transform.position);
            Context.m_Rigidbody.AddForce(moveForce * Context.m_Stats.accelerateSpeed * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
