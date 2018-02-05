using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Enemy Stats")]
public class EnemyStats : ScriptableObject {

    public float maxHealth;
    public float maxSpeed;
    public float accelerateSpeed;
    public float stunTime;
}
