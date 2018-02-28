using System;

namespace Basics
{
    public class StateMachine
    {
        private IState state;

        public bool InState<T>() => state is T;

        public void ChangeState(IState _newState)
        {
            state?.Finish();
            if (_newState == null)
                throw new NullReferenceException("Cannot change to a null state");
            state = _newState;
            state.Start();
        }

        public void Reset()
        {
            state?.Finish();
            state = null;
        }

        public void Update() => state?.Update();

        public bool IsFinished() => state == null;
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