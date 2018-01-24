using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float fireRate = 0.2f;

    float fireTimer = 0f;
    AudioSource m_AudioSource;

    private void Awake() {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        fireTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && fireTimer >= fireRate) {
            m_AudioSource.Play();
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            fireTimer = 0f;
        }
    }
}
