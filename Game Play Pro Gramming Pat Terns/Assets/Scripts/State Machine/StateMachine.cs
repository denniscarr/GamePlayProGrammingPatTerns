using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* In state machine parlance, 'context' refers to the object which is utilizing the state machine. So in our case it may be a player, enemy, etc.
 * We are making the state machine a generic class so that it is easier to reference the context. */
public class StateMachine<TContext> {

    // Keep a reference to the context so that we can access its data.
    private readonly TContext _context;

    // Optional: we can cache the machine's states in a dictionary in case we need to use them again.
    private readonly Dictionary<Type, State> _stateCache = new Dictionary<Type, State>();

    // Keep track of the machine's current state. We allow get access to other classes.
    public State CurrentState { get; private set; }

    // We don't want to change the current state in the middle of an update, so we store a pending state here instead.
    private State _pendingState;

    // A very simple constructor.
    public StateMachine(TContext context) {
        _context = context;
    }


    public void Update() {
        // Handle any pending transitions if an external class called TransitionTo() (Though that probably shouldn't happen)
        PerformPendingTransitionTo();

        // Make sure current state isn't null.
        Debug.Assert(CurrentState != null, "Updating FSM with null current state. Did you forget to transition to a starting state?");

        CurrentState.Update();

        // Handle any pending transition that may have been activated during Update();
        PerformPendingTransitionTo();
    }


    // Queues a transition to a new state.
    public void TransitionTo<TState>() where TState : State {
        _pendingState = GetOrCreateState<TState>();
    }


    // Actually perform a transition.
    private void PerformPendingTransitionTo() {
        if (_pendingState == null) { return; }

        // Have the current state perform any exit behavior.
        if (CurrentState != null) { CurrentState.OnExit(); }

        CurrentState = _pendingState;
        CurrentState.OnEnter();
        _pendingState = null;
    }


    // A helper method for managing the cacheing of state instances.
    private TState GetOrCreateState<TState>() where TState : State {

        // If a state of this type has already been created and cached, reuse it.
        State state;
        if (_stateCache.TryGetValue(typeof(TState), out state)) {
            return (TState)state;
        }

        // If a state of this type has not been created, do so now.
        else {
            // This 'Activator' thing is used to create instances using only a type.
            var newState = Activator.CreateInstance<TState>();
            newState.Parent = this;
            newState.Init();
            // Store this new state in the cache dictionary.
            _stateCache[typeof(TState)] = newState;
            return newState;
        }
    }


    // We define the State class inside the FSM class so that it's tied to the context type of the FSM. This way you can't transition to a state with a different context.
    public abstract class State {

        // We keep track of the state machine controlling us so that we can, for instance, call TransitionTo() from inside it.
        internal StateMachine<TContext> Parent { get; set; }

        // A bit of sugar.
        protected TContext Context { get { return Parent._context; } }


        // A convenience method for transitioning from within a state.
        protected void TransitionTo<TState>() where TState : State {
            Parent.TransitionTo<TState>();
        }

        // Called when the state is first created.
        public virtual void Init() { }

        // Called when the state becomes active.
        public virtual void OnEnter() { }

        // Called while the state is active.
        public virtual void Update() { }

        // Called when the state becomes inactive.
        public virtual void OnExit() { }

        // Called when the state machine is cleared.
        public virtual void CleanUp() { }
    }
}
