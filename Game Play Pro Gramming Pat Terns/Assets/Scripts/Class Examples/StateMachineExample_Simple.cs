using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineExample_Simple : MonoBehaviour {

    // This class represents like a monster or something I guess....

    float health = 5f;

    // We keep track of the current state with an enum.
    public enum State {
        Sleeping,
        Searching,
        Attacking,
        Fleeing
    }

    private State state;

    private void Update() {

        // If behavior and logic are relatively simple, we can put everything inside a switch case.
        switch (state) {
            case State.Sleeping:
                if (LoudNoiseDetectedInRange(10)) { state = State.Searching; }
                break;
            case State.Searching:
                if (EnemyInSight(50)) { state = State.Attacking; }
                break;
            case State.Attacking:
                Enemy enemy = GetNearestEnemy();
                if (enemy != null) {
                    if (health < 5f) { state = State.Fleeing; } else { Attack(enemy); }
                } else {
                    state = State.Searching;
                }
                break;
            case State.Fleeing:
                Enemy emeny = GetNearestEnemy();
                if (emeny != null) {
                    RunInDirection(Vector3.Normalize(emeny.transform.position - transform.position));
                }
                else {
                    state = State.Searching;
                }
                break;
        }

        /* But notice how, even with simple logic, things are already beginning to become very messy. Also, what if we wanted to have specific variables or functionality
         * unique to certain states? This is why we might want to take a class-based approach instead. */
    }


    void RunInDirection(Vector3 direction) {
        // Do stuff.
    }


    void Attack(Enemy target) {
        // Do stuff.
    }


    bool LoudNoiseDetectedInRange(float range) {
        // Do stuff.
        return true;
    }


    bool EnemyInSight(float range) {
        // Do stuff.
        return true;
    }


    Enemy GetNearestEnemy() {
        Enemy enemy = FindObjectOfType<Enemy>();
        return enemy;
    }
}
