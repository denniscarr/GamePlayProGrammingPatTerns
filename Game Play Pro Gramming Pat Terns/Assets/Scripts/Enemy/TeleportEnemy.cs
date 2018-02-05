using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEnemy : Enemy {

    bool chooseNewPoint = true;
    Coroutine movementCoroutine;

    protected override void Update() {
        base.Update();
    }


    protected override void Spawn() {
    }


    protected override void Move() {
        if (chooseNewPoint) {
            Vector3 nextPosition = new Vector3(
                    Random.Range(-GameManager.groundHalfExtents.x, GameManager.groundHalfExtents.x),
                    Random.Range(-GameManager.groundHalfExtents.y, GameManager.groundHalfExtents.y),
                    transform.position.z
                );

            movementCoroutine = StartCoroutine(MoveToPoint(nextPosition));

            chooseNewPoint = false;
        }
    }


    IEnumerator MoveToPoint(Vector3 pointToMoveTo) {

        GetComponentInChildren<Rotator>().freezeRotation = false;

        yield return new WaitForSeconds(1.8f);

        GetComponentInChildren<Rotator>().freezeRotation = true;

        yield return new WaitUntil(() => {
            if (Vector3.Distance(pointToMoveTo, transform.position) <= 0.1f) {
                return true;
            } else {
                Vector3 direction = Vector3.Normalize(pointToMoveTo - transform.position);
                Vector3 newPosition = transform.position + direction * m_Stats.maxSpeed * Time.deltaTime;
                m_Rigidbody.MovePosition(newPosition);
                return false;
            }
        });

        chooseNewPoint = true;
        yield return null;
    }


    protected override void Stunned() {
        StopCoroutine(movementCoroutine);
        GetComponentInChildren<Rotator>().freezeRotation = true;
        base.Stunned();
    }


    protected override void RecoverFromStun() {
        base.RecoverFromStun();
        GetComponentInChildren<Rotator>().freezeRotation = false;
        chooseNewPoint = true;
    }

}
