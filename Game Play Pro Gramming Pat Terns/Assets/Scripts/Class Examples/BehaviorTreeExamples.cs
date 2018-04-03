using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClassExamples {

    /* A 'Node' is the basic building block of a behavior tree. It just needs to report success or failure (via a bool) when it's updated.
     * It takes a context variable so that we know what class/object to operate on. */
    public abstract class Node<T> {
        public abstract bool Update(T context);
    }

    
    /* A tree is actually just another type of node. The important thing about a tree is that it contains a reference to the 'root' node, where behavior
     * tree evaluation begins. Because trees are a type of node, we can add them to other trees as sub trees. */
    public class Tree<T> : Node<T> {

        private readonly Node<T> _root;

        public Tree(Node<T> root) {
            _root = root;
        }

        public override bool Update(T context) {
            return _root.Update(context);
        }
    }


    /* OUTER NODES */
    /* These are the nodes where the evaluation of the behavior tree ends, and an action is chosen. */

    // A do node actually performs an action.
    public class Do<T> : Node<T> {

        // We could use this delegate to carry out the action dictated by this node. Alternatively, it could be better to use a unique subclass instead.
        public delegate bool NodeAction(T Context);

        private readonly NodeAction _action;

        public Do(NodeAction action) {
            _action = action;
        }

        public override bool Update(T context) {
            return _action(context);
        }
    }


    /* Condition nodes are outer nodes which test something.
     * As above, we can use the delegate version as below or create unique subclasses. */
    public class Condition<T> : Node<T> {

        private readonly Predicate<T> _condition;

        public Condition(Predicate<T> condition) {
            _condition = condition;
        }

        public override bool Update(T context) {
            return _condition(context);
        }
    }


    /* INNER/DECISION NODES */
    /* An inner node has several child or branch nodes. They define the structure/logic of the tree, but they don't
     * actually do anything. */
    public abstract class BranchNode<T> : Node<T> {

        protected Node<T>[] Children { get; private set; }

        // 'params' is a keyword which lets you call a function either by supplying an array of the given type, or
        // with a comma separated list of values that can be any length (including 0).
        // So we could call this function like this: new BranchNode();
        // or like this: new BranchNode(child1, child2, child3);
        // or like this: Node<T>[] children = {child1, child2, child3}; ... new BranchNode(children);
        protected BranchNode(params Node<T>[] children) {
            Children = children;
        }
    }


    /* A selector is a branch node which returns true if one of its children succeeds and returns false if all of its children failed.
     * It's normally used to select from a series of ranked actions IE: Try eating popcorn, if this fails, try eating dirt */
     public class Selector<T> : BranchNode<T> {

        public Selector(params Node<T>[] children) : base(children) {}

        // Return true if at least one child returns true.
        public override bool Update(T context) {
            foreach (var child in Children) {
                if (child.Update(context)) { return true; }
            }
            return false;
        }
    }


    /* A sequence is a type of branch node which returns true only if all of it's children return true.
     * Used for 'checklists'. (If there's popcorn nearby and I'm hungry, eat popcorn.) */
    public class Sequence<T> : BranchNode<T> {

        public Sequence(params Node<T>[] children) : base(children) {}

        // Return false if any child returns false.
        public override bool Update(T context) {
            foreach (var child in Children) {
                if (!child.Update(context)) { return false; }
            }
            return true;
        }
    }


    /* A Decorator is a branch node which is just there to act as a modifier for another node. */
    public abstract class Decorator<T> : BranchNode<T> {

        protected Node<T> Child { get; private set; }

        protected Decorator(Node<T> child) {
            Child = child;
        }
    }

    
    /* An example of a common Decorator 'Not', which reverses the output of its child. */
    public class Not<T> : Decorator<T> {

        public Not(Node<T> child) : base(child) {}

        public override bool Update(T context) {
            return !Child.Update(context);
        }
    }
}
