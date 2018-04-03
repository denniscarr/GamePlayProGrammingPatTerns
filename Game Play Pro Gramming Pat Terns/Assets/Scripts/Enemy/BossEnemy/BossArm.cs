using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArm : MonoBehaviour {

    public SkinnedMeshRenderer armMesh;
    [SerializeField] Collider myCollider;

    private const int MAX_HEALTH = 10;
    private int currentHealth;
    private int CurrentHealth {
        get { return currentHealth; }
        set {
            // Remove health from my health.
            currentHealth = value;
            currentHealth = Mathf.Clamp(currentHealth, 0, MAX_HEALTH);

            // Make my mesh renderer look different based on how much health I now have.
            float newBlendValue = MyMath.Map(currentHealth, 0, MAX_HEALTH, 100f, 0f);
            armMesh.SetBlendShapeWeight(0, newBlendValue);

            // Oh yeah, and become stump.
            if (currentHealth == 0) {
                BecomeStump();
            }
        }
    }

    // Gains one point of health back every [this number] seconds.
    const float HEALTH_REGEN_RATE = 0.2f;
    float healthRegenTimer = 0f;

    private const float STUMP_ROTATION = 65f;
    public bool iAmAStump = false;


    private void Awake() {
        currentHealth = MAX_HEALTH;
    }


    public void Run() {
        if (!iAmAStump) {
            healthRegenTimer += Time.deltaTime;
            if (healthRegenTimer >= HEALTH_REGEN_RATE) {
                CurrentHealth++;
                healthRegenTimer = 0f;
            }
        }
    }


    public void Shoot(GameObject bulletPrefab) {
        if (iAmAStump) { return; }
        GameObject newBullet = Instantiate(bulletPrefab, transform.position + transform.right * 4f, Quaternion.identity);
        newBullet.GetComponent<EnemyBullet>().direction = transform.right;
    }


    private void BecomeStump() {
        // Stop being um you know bendy or whatever.
        armMesh.SetBlendShapeWeight(0, 0f);

        // Rotate arm so that it looks like it came off.
        Vector3 newRotation = armMesh.transform.localRotation.eulerAngles;
        newRotation.z = STUMP_ROTATION;
        armMesh.transform.localRotation = Quaternion.Euler(newRotation);

        // Turn off collision.
        myCollider.enabled = false;
        GetComponent<Collider>().enabled = false;

        iAmAStump = true;
    }


    private void OnTriggerEnter(Collider other) {
        // Confirm that other is player bullet.
        if (other.GetComponent<PlayerBullet>() != null) {
            CurrentHealth--;
            healthRegenTimer = 0f;
        }
    }
}
