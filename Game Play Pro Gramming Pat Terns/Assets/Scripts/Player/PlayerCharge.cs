using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharge : MonoBehaviour {

    [SerializeField] float chargeSpeed = 200f;
    [SerializeField] float chargeLength = 3f;
    [SerializeField] float chargeCooldown = 1f;
    [SerializeField] GameObject chargingMesh;
    [SerializeField] GameObject normalMesh;
    [SerializeField] AudioSource m_AudioSource;

    [HideInInspector] public bool isCharging;
    float cooldownTimer;
    Rigidbody m_Rigidbody;


    private void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
        cooldownTimer = chargeCooldown;
    }


    private void Update() {
        cooldownTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer >= chargeCooldown) {
            StartCoroutine(Charge());
            cooldownTimer = 0f;
        }
    }


    IEnumerator Charge() {
        GetComponent<PlayerController>().isAcceptingInput = false;
        isCharging = true;
        float distanceCharged = 0f;
        normalMesh.SetActive(false);
        chargingMesh.SetActive(true);
        m_AudioSource.Play();

        yield return new WaitUntil(() => {
            if (chargeLength - distanceCharged <= 0.1f) {
                return true;
            } else {
                Vector3 newPosition = transform.position + transform.forward * chargeSpeed * Time.deltaTime;
                distanceCharged += Vector3.Distance(transform.position, newPosition);
                m_Rigidbody.MovePosition(newPosition);
                return false;
            }
        });

        isCharging = false;
        GetComponent<PlayerController>().isAcceptingInput = true;
        chargingMesh.SetActive(false);
        normalMesh.SetActive(true);

        yield return null;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Enemy>() && isCharging) {
            other.GetComponent<Enemy>().GetHitByCharge();
        }
    }
}
