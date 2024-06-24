using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RetroRush
{
    public enum State
    {
        Menu,
        Start,
        PlayerDied,
        WinLevel,
        GameOver
    }
    
    public interface IGameState
    {
        public IReadOnlyReactiveProperty<State> State { get; }
        public void SetState(State state);
    }
    
    public class GameState : IGameState
    {
        public IReadOnlyReactiveProperty<State> State
            => _State;

        public IReactiveProperty<State> _State = new ReactiveProperty<State>();
        
        public void SetState(State state)
        {
            _State.Value = state;
        }
    }
}
