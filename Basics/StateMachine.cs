namespace Basics
{
    public class StateMachine
    {
        private State currentState;

        public bool InState<T>() { return currentState is T; }

        public void ChangeState(State _newState)
        {
            if (currentState != null)
                currentState.OnFinish();
            currentState = _newState;
            currentState.machine = this;
            currentState.OnStart();
        }

        public void Reset()
        {
            if (currentState != null)
                currentState.OnFinish();
            currentState = null;
        }

        public void Update()
        {
            if (currentState != null)
                currentState.OnUpdate();
        }

        public bool IsFinished()
        {
            return currentState == null;
        }
    }

    // OnUpdate -- Fired every frame of the game.
    // OnStart  -- Fired once when the state is transitioned to.
    // OnFinish -- Fired as the state concludes.
    public class State
    {
        public StateMachine machine;

        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnFinish() { }

        // States may call ConcludeState on themselves to end their processing.
        public void ConcludeState() { machine.Reset(); }
    }
}