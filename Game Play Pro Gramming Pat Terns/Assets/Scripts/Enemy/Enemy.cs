﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    [SerializeField] protected EnemyStats m_Stats;
    [SerializeField] protected GameObject getStunnedParticlesPrefab;
    [SerializeField] protected GameObject recoverFromStunParticlesPrefab;
    [SerializeField] protected GameObject deathParticles;
    [SerializeField] protected float knockbackSpeed = 100f;

    public enum State { Normal, Stunned, Knockback }
    [HideInInspector] public State m_State = State.Normal;

    private float _currentHealth;
    protected float currentHealth {
        get { return _currentHealth; }
        set {
            _currentHealth = value;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, m_Stats.maxHealth);
            if (_currentHealth == 0) {
                if (m_State == State.Normal) { Stunned(); }
            }
        }
    }
    private bool freezeHealthRecharge;
    private float healthRechargeRate = 0.3f;
    private float healthRechargeTimer = 0f;

    [HideInInspector] public bool isDead = false;

    private float stunTimer;

    protected Rigidbody m_Rigidbody { get { return GetComponent<Rigidbody>(); } }
    protected Animator m_Animator { get { return GetComponent<Animator>(); } }


    public void Initialize() {
        currentHealth = m_Stats.maxHealth;
    }

    public virtual void Run() {
        switch (m_State) {
            case State.Normal:
                RechargeHealth();
                break;
            case State.Stunned:
                RunStunTimer();
                break;
            case State.Knockback:
                break;
        }
    }

    public virtual void FixedRun() {
        switch (m_State) {
            case State.Normal:
                Move();
                break;
            case State.Stunned:
                break;
        }
    }

    // Sandbox methods.
    public abstract void Spawn();

    protected abstract void Move();

    public virtual void GetHitByBullet() {
        StartCoroutine(BulletHitSequence());
    }

    protected virtual void Stunned() {
        m_State = State.Stunned;
        Instantiate(getStunnedParticlesPrefab, transform.position, Quaternion.identity);
        m_Animator.SetBool("Is Stunned", true);
        stunTimer = 0f;
    }

    protected virtual void Die() {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Camera.main.GetComponent<ScreenShake>().SetShake(0.4f, 0.2f);
        isDead = true;
    }


    // Tool methods:
    protected Vector3 SteerTowards(Vector3 target) {
        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, m_Stats.maxSpeed);
        Vector3 desiredVelocity = Vector3.Normalize(target - transform.position);
        desiredVelocity *= m_Stats.maxSpeed;
        Vector3 turnForce = desiredVelocity - m_Rigidbody.velocity;
        return (turnForce);
    }


    protected void RechargeHealth() {
        if (freezeHealthRecharge) { return; }

        if (currentHealth >= m_Stats.maxHealth) {
            currentHealth = m_Stats.maxHealth;
            return;
        }

        healthRechargeTimer += Time.deltaTime;
        if (healthRechargeTimer >= healthRechargeRate) {
            currentHealth++;
            healthRechargeTimer = 0f;
        }
    }


    protected void RunStunTimer() {
        stunTimer += Time.deltaTime;
        if (stunTimer >= m_Stats.stunTime) {
            stunTimer = 0;
            RecoverFromStun();
        }
    }


    protected virtual void RecoverFromStun() {
        StartCoroutine(RecoverFromStunSequence());
    }


    IEnumerator RecoverFromStunSequence() {
        currentHealth = m_Stats.maxHealth;
        Instantiate(recoverFromStunParticlesPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.7f);
        m_Animator.SetBool("Is Stunned", false);
        m_State = State.Normal;
        yield return null;
    }


    protected IEnumerator BulletHitSequence() {

        m_Animator.SetTrigger("Hurt Trigger");

        switch (m_State) {
            case (State.Normal):
                m_Rigidbody.velocity = Vector3.zero;
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


    public void GetHitByCharge() {
        StartCoroutine(GetHitByChargeCoroutine());
    }


    IEnumerator GetHitByChargeCoroutine() {
        currentHealth -= 5;

        bool willDie = false;
        if (currentHealth <= 0) {
            Services.hitPause.StartHitPause(0.2f);
            willDie = true;
        }

        Vector3 knockbackDirection = transform.position - PlayerController.m_Transform.position;
        knockbackDirection.z = 0f;

        float knockbackDistance = 2.5f;
        if (willDie) { knockbackDistance = 5f; }
        float distanceKnocked = 0f;

        m_State = State.Knockback;

        yield return new WaitUntil(() => {
            if (knockbackDistance - distanceKnocked <= 0.1f) {
                m_Rigidbody.velocity = Vector3.zero;
                return true;
            } else {
                Vector3 newPosition = transform.position + knockbackDirection * knockbackSpeed * Time.deltaTime;
                distanceKnocked += Vector3.Distance(transform.position, newPosition);
                m_Rigidbody.MovePosition(newPosition);
                return false;
            }
        });

        if (willDie) {
            Die();
            yield return null;
        }

        if (m_State != State.Stunned) {
            m_State = State.Normal;
        }
        
        yield return null;
    }
}
