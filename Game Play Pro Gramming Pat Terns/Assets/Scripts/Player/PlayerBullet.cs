using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

    [SerializeField] float speed = 30f;
    [SerializeField] float spread = 10f;
    [SerializeField] PerlinNoise wiggleNoise;
    [SerializeField] GameObject hitEnemyParticlesPrefab;
    [SerializeField] GameObject hitStunnedEnemyParticlesPrefab;

    Vector3 direction;
    Rigidbody m_Rigidbody;

    Vector3 lastPosition;

    private void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        // Get direction based on mouse position.
        Plane movementPlane = new Plane(transform.position, transform.position + transform.up, transform.position + transform.right);
        Vector3 mousePoint = MouseUtil.ProjectMousePositionOntoPlane(movementPlane);
        direction = Vector3.Normalize(mousePoint - transform.position);
        direction = Quaternion.Euler(0, 0, Random.Range(-spread, spread)) * direction;
        wiggleNoise = new PerlinNoise(0.5f);
    }

    private void Update() {
        wiggleNoise.Iterate();
    }

    private void FixedUpdate() {
        lastPosition = transform.position;

        // Move to new position.
        direction = Quaternion.Euler(0f, 0f, MyMath.Map(wiggleNoise.value.x, 0f, 1f, -spread, spread)) * direction;
        Vector3 newPosition = transform.position + direction * speed * Time.fixedDeltaTime;
        m_Rigidbody.MovePosition(newPosition);

        // Delete if too far from level.
        if (Vector3.Distance(Vector3.zero, transform.position) >= 50f) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        bool isEnemy = collider.GetComponent<Enemy>();
        if (isEnemy) {
            collider.GetComponent<Enemy>().GetHitByBullet();
            if (collider.GetComponent<Enemy>().m_State == Enemy.State.Stunned) {
                Instantiate(hitStunnedEnemyParticlesPrefab, lastPosition, Quaternion.identity);
            } else {
                Instantiate(hitEnemyParticlesPrefab, lastPosition, Quaternion.identity);
            }
            //Destroy(gameObject);
        }
    }

}
