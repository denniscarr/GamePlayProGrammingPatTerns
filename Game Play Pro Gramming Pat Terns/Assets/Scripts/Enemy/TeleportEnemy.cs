using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEnemy : Enemy {

    bool chooseNewPoint = true;
    Coroutine movementCoroutine;

    public override void Run() {
        base.Run();

        if (m_State == State.InBackground || m_State == State.Spawning) { meshParent.GetComponent<ScaleFuckerUpper>().enabled = false; }
        else { meshParent.GetComponent<ScaleFuckerUpper>().enabled = true; }
    }

    protected override void Move() {
        if (chooseNewPoint) {
            Vector3 nextPosition = GameManager.GetPointInArena(Vector2.one * (GetComponent<SphereCollider>().radius + 0.5f));
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
