using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [SerializeField] GameObject approachEnemyPrefab;
    [SerializeField] GameObject teleportEnemyPrefab;
    [SerializeField] GameObject speedUpEnemyPrefab;
    [SerializeField] GameObject bossEnemyPrefab;

    [SerializeField] Transform enemyRoot;

    [SerializeField] EnemyWave[] enemyWaves;
    int currentWave = 0;
    List<Enemy> activeEnemies = new List<Enemy>();
    List<Enemy> backgroundEnemies = new List<Enemy>();


    public void Initialize() {
        DestroyAllExistingEnemies();
        currentWave = 0;
        CreateFirstWave();
    }


    public void DestroyAllExistingEnemies() {
        if (activeEnemies.Count > 0) {
            for (int i = activeEnemies.Count - 1; i >= 0; i--) {
                Destroy(activeEnemies[i].gameObject);
            }
        }
        activeEnemies.Clear();
        if (backgroundEnemies.Count > 0) {
            for (int i = backgroundEnemies.Count - 1; i >= 0; i--) {
                Destroy(backgroundEnemies[i].gameObject);
            }
        }
        backgroundEnemies.Clear();
    }


    public void Run() {
        CheckForDeadEnemies();

        // If all enemies have been destroyed, spawn a new wave.
        if (activeEnemies.Count == 0) { CreateNextWave(); }

        for (int i = 0; i < activeEnemies.Count; i++) { activeEnemies[i].Run(); }
        for (int i = 0; i < backgroundEnemies.Count; i++) { backgroundEnemies[i].Run(); }
    }


    public void FixedRun() {
        for (int i = 0; i < activeEnemies.Count; i++) { activeEnemies[i].FixedRun(); }
        for (int i = 0; i < backgroundEnemies.Count; i++) { backgroundEnemies[i].FixedRun(); }
    }


    void CreateFirstWave() {
        // Instantiate all active enemies.
        InstantiateAllEnemiesInWave(enemyWaves[0], false);

        for (int i = 0; i < activeEnemies.Count; i++) {
            activeEnemies[i].Initialize();
            activeEnemies[i].EnterBackgroundInstant();
            activeEnemies[i].SpawnFromBackground();
        }

        // Instantiate all background enemies.
        InstantiateAllEnemiesInWave(enemyWaves[1], true);
        for (int i = 0; i < backgroundEnemies.Count; i++) {
            backgroundEnemies[i].Initialize();
            backgroundEnemies[i].EnterBackground();
        }

        currentWave++;
    }


    void CreateNextWave() {
        // Move all background enemies to foreground.
        for (int i = 0; i < backgroundEnemies.Count; i++) {
            backgroundEnemies[i].SpawnFromBackground();
            activeEnemies.Add(backgroundEnemies[i]);
        }

        backgroundEnemies.Clear();

        // Instantiate enemies from next wave and move them into the background.
        if (enemyWaves[currentWave + 1] != null) {
            EnemyWave nextWave = enemyWaves[currentWave + 1];

            // Instantiate each enemy in the next wave.
            InstantiateAllEnemiesInWave(nextWave, true);

            // Have all enemies in next wave enter background.
            for (int i = 0; i < backgroundEnemies.Count; i++) {
                backgroundEnemies[i].Initialize();
                backgroundEnemies[i].EnterBackground();
            }
        }

        currentWave++;
    }


    void CreateEnemy(GameObject enemyPrefab, bool putInBackground) {
        Vector3 newEnemyPosition = GameManager.GetPointInArena(Vector2.one);
        GameObject newEnemy = Instantiate(enemyPrefab, newEnemyPosition, Quaternion.identity);
        newEnemy.transform.parent = enemyRoot;

        if (putInBackground) { backgroundEnemies.Add(newEnemy.GetComponent<Enemy>()); }
        else { activeEnemies.Add(newEnemy.GetComponent<Enemy>()); }
    }


    void CheckForDeadEnemies() {
        for (int i = 0; i < activeEnemies.Count; i++) {
            if (activeEnemies[i].isDead) {
                Destroy(activeEnemies[i].gameObject);
                activeEnemies.Remove(activeEnemies[i]);
            }
        }
    }


    void InstantiateAllEnemiesInWave(EnemyWave wave, bool putInBackground) {
        for (int i = 0; i < wave.approachEnemies; i++) { CreateEnemy(approachEnemyPrefab, putInBackground); }
        for (int i = 0; i < wave.teleportEnemies; i++) { CreateEnemy(teleportEnemyPrefab, putInBackground); }
        for (int i = 0; i < wave.speedUpEnemies; i++) { CreateEnemy(speedUpEnemyPrefab, putInBackground); }
        for (int i = 0; i < wave.bossEnemies; i++) { CreateEnemy(bossEnemyPrefab, putInBackground); }
    }
}


[Serializable]
public class EnemyWave {
    public int approachEnemies;
    public int teleportEnemies;
    public int speedUpEnemies;
    public int bossEnemies;
}
