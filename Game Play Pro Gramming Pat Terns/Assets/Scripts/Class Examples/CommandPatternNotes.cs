using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassNotes {

    /*
        * A command can be thought of as a function call made into an object.
        * 
        * DoSomething(int x);
        * DoSomething(1);
        * 
        * turns into...
        * 
        * public class DoSomething {
        *    int _x;
        *    public DoSomething(int x) { _x = x; }
        *    public ActuallyDoSomething() {...}
        * }
        * 
    */

    // All the base class needs is as method for executing the command.
    public abstract class Command {
        public abstract void Execute();
    }

    // You can then create subclasses for the commands you need.
    public class DestroyGameObject : Command {
        private GameObject _object;
        public DestroyGameObject(GameObject go) { _object = go; }
        public override void Execute() {
            UnityEngine.GameObject.Destroy(_object);
        }
    }


    // Example: Prioritized Actions
    // Potential uses: fighting game attack priority (if two actions have the same frame, which one 'wins'?)

    public abstract class Attack {
        protected GameObject Character { get; set; }
        public int Priority { get; protected set; }
        public abstract void Perform();
    }


    public class Punch : Attack {
        public override void Perform() {
            // punch
        }
    }


    public class AttackManager {

        List<Attack> _pendingAttacks = new List<Attack>();

        public void QueueAttack(Attack attack) { _pendingAttacks.Add(attack); }

        public void Update() {

            // Sort attacks by priority, and then call in the new order.
            _pendingAttacks.Sort((Attack a, Attack b) => { return a.Priority - b.Priority; });
            foreach(var attack in _pendingAttacks) {
                attack.Perform();
            }

            _pendingAttacks.Clear();
        }
    }


    public abstract class Character {

        AttackManager attackManager;

        public void OnButtonPress() {
            attackManager.QueueAttack(new Punch());
        }
    }
}