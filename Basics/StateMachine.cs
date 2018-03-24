using System;

namespace Basics
{
    public class StateMachine<T> where T : IState
    {
        protected T state;

        public bool InState<U>() where U : IState => state is U;
        public string CurrentStateName => state?.GetType().Name;

        public void ChangeState(T _newState)
        {
            state?.Finish();
            if (Equals(_newState, default))
                throw new NullReferenceException("Cannot change to a null state");
            state = _newState;
            state.Start();
        }

        public void Reset()
        {
            state?.Finish();
            state = default;
        }

        public virtual void Update() => state?.Update();

        public bool IsFinished() => Equals(state, default);
    }

    // Update -- Fired every frame of the game.
    // Start  -- Fired once when the state is transitioned to.
    // Finish -- Fired as the state concludes.
    public interface IState
    {
        void Start();
        void Update();
        void Finish();
    }
}