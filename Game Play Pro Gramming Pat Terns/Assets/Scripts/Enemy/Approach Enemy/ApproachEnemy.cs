using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using System;

public class ApproachEnemy : Enemy {

    [SerializeField] float maxSineAngle = 15f;
    [SerializeField] float sineSpeed = 1f;
    public int requiredPulses = 5;
    public float attackRange = 2.5f;
    public float attackDistance = 5f;
    [SerializeField] float attackSpeed = 10f;

    private Tree<ApproachEnemy> _tree;
    [HideInInspector] public bool isFleeing = false;
    [HideInInspector] public Vector3 fleePoint;
    [HideInInspector] public int timesPulsed;

    float currentSineValue = 0f;



    public override void Initialize() {
        base.Initialize();
        currentSineValue = UnityEngine.Random.Range(-100f, 100f);

        foreach(Rotator rotator in meshParent.GetComponentsInChildren<Rotator>()) {
            rotator.transform.rotation = UnityEngine.Random.rotation;
            rotator.relativeSpeed = new Vector3(
                    rotator.relativeSpeed.x + UnityEngine.Random.Range(-0.1f, 0.1f),
                    rotator.relativeSpeed.y + UnityEngine.Random.Range(-0.1f, 0.1f),
                    rotator.relativeSpeed.z + UnityEngine.Random.Range(-0.1f, 0.1f)
                );
        }

        GetNewFleePoint();

        // Define behavior tree
        _tree = new Tree<ApproachEnemy>(
            new Selector<ApproachEnemy>(

                // Highest priority: Flee
                new Sequence<ApproachEnemy>(
                     new ShouldFlee(),
                     new Not<ApproachEnemy>(new HasReachedFleePoint()),
                     new FleeBehavior()
                ),

                // Attack behavior
                new Sequence<ApproachEnemy>(
                    new IsPlayerInRange(),

                    // Pulse behavior
                    new Sequence<ApproachEnemy>(
                        new PulseBehavior(),
                        new IsFinishedPulsing()
                    ),

                    new AttackBehavior()
                ),

                // Default behavior: Seek
                new SeekBehavior()
            )
        );
    }

    protected override void Move() {}

    public override void Run() {
        base.Run();
        _tree.Update(this);
        currentSineValue += sineSpeed * Time.deltaTime;
    }

    private void MoveTowardPlayer() {
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

    public void GetNewFleePoint() {
        fleePoint = GameManager.GetPointInArena(new Vector2(0.5f, 0.5f));
    }

    public void Pulse() {
        m_Rigidbody.velocity = Vector3.zero;
        if (!isPulsing) { pulseCoroutine = StartCoroutine(PulseCoroutine()); }
    }

    bool isPulsing;
    Coroutine pulseCoroutine;
    private IEnumerator PulseCoroutine() {
        isPulsing = true;
        yield return new WaitForSeconds(0.5f);
        timesPulsed++;
        Debug.Log("Pulse: " + timesPulsed);
        isPulsing = false;
        yield return null;
    }


    public override void GetHitByBullet(PlayerBullet bullet) {
        base.GetHitByBullet(bullet);

        if (isPulsing) {
            StopCoroutine(pulseCoroutine);
            timesPulsed = 0;
            isFleeing = true;
        }
    }


    public void Attack() {
        timesPulsed = 0;
        if (!isAttacking) { StartCoroutine(AttackCoroutine()); }
    }


    bool isAttacking;
    private IEnumerator AttackCoroutine() {
        isAttacking = true;
        Vector3 attackDirection = Vector3.Normalize(PlayerController.m_Transform.position - transform.position);
        Vector3 attackPoint = transform.position + attackDirection * attackDistance;

        yield return new WaitUntil(() => {
            if (Vector3.Distance(transform.position, attackPoint) <= 0.5f) {
                return true;
            }

            else {
                transform.position += attackDirection * attackSpeed * Time.deltaTime;
                return false;
            }
        });

        isAttacking = false;
        yield return null;
    }


    ///// BEHAVIOR TREE STUFF /////

    /* Conditions */
    private class ShouldFlee : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            Debug.Log(context.isFleeing);
            return context.isFleeing;
        }
    }

    private class HasReachedFleePoint : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            bool hasReachedFleePoint = Vector3.Distance(context.transform.position, context.fleePoint) <= 0.5f;
            if (hasReachedFleePoint) {
                context.GetNewFleePoint();
                context.isFleeing = false;
            }
            return hasReachedFleePoint;
        }
    }

    private class IsPlayerInRange : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            return context.GetDistanceToPlayer() <= context.attackRange;
        }
    }

    private class IsFinishedPulsing : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            return context.timesPulsed >= context.requiredPulses;
        }
    }


    /* Actions */
    private class FleeBehavior : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            Debug.Log("fleeing.");
            context.MoveTowardPoint(context.fleePoint);
            return true;
        }
    }

    private class PulseBehavior : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            context.Pulse();
            return true;
        }
    }

    private class AttackBehavior : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            context.Attack();
            return true;
        }
    }

    private class SeekBehavior : Node<ApproachEnemy> {
        public override bool Update(ApproachEnemy context) {
            context.MoveTowardPlayer();
            return true;
        }
    }
}
