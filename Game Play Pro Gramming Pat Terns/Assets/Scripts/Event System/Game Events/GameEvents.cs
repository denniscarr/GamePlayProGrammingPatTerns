using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents {
    public class EnemyDied : GameEvent {
        public EnemyDied() { }
    }

    public class EnemyRecoveredFromStun : GameEvent {
        public EnemyRecoveredFromStun() { }
    }

    public class EnemyStunned : GameEvent {
        public EnemyStunned() { }
    }
}