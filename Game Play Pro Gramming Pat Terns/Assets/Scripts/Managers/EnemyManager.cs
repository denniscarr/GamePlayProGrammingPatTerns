using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [SerializeField] GameObject approachEnemyPrefab;
    [SerializeField] GameObject teleportEnemyPrefab;

    [SerializeField] EnemyWave[] enemyWaves;
    int currentWave = 0;
    List<Enemy> enemies = new List<Enemy>();


    public void Initialize() {
        CreateWave();
    }


    public void Run() {
        CheckForDeadEnemies();

        // If all enemies have been destroyed, spawn a new wave.
        if (enemies.Count == 0) { CreateWave(); }

        for (int i = 0; i < enemies.Count; i++) { enemies[i].Run(); }
    }


    public void FixedRun() {
        for (int i = 0; i < enemies.Count; i++) { enemies[i].FixedRun(); }
    }


    void CreateWave() {
        EnemyWave newWave = enemyWaves[currentWave];

        for (int i = 0; i < newWave.approachEnemies; i++) { CreateEnemy(approachEnemyPrefab); }
        for (int i = 0; i < newWave.teleportEnemies; i++) { CreateEnemy(teleportEnemyPrefab); }

        for (int i = 0; i < enemies.Count; i++) { enemies[i].Initialize(); }
        currentWave++;
    }


    void CreateEnemy(GameObject enemyPrefab) {
        Vector3 newEnemyPosition = GameManager.GetPointInArena(Vector2.one);
        GameObject newEnemy = Instantiate(enemyPrefab, newEnemyPosition, Quaternion.identity);
        newEnemy.GetComponent<Enemy>().Spawn();
        enemies.Add(newEnemy.GetComponent<Enemy>());
    }


    void CheckForDeadEnemies() {
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i].isDead) {
                Debug.Log("Destroying enemy");
                Destroy(enemies[i].gameObject);
                enemies.Remove(enemies[i]);
            }
        }
    }
}


[Serializable]
public class EnemyWave {
    public int approachEnemies;
    public int teleportEnemies;
}
