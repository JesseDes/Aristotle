using System.Collections.Generic;

/// <summary>
/// StateMachine Holds States and allows you to find/Share/Set State Objects
/// </summary>
/// <typeparam name="T">Identifier Type for states (Your Enum, String)</typeparam>

public class StateMachine<T>
{
    public State<T> _currentState { get; private set; }


    private List<State<T>> _stateList;

    public StateMachine()
    {
        _stateList = new List<State<T>>();   
    }

    public void RegisterState(State<T> state)
    {
        if (!_stateList.Contains(state))
            _stateList.Add(state);
    }

    public bool SetState(T state)
    {
        foreach (State<T> registerdState in _stateList)
        {
            if(EqualityComparer<T>.Default.Equals(registerdState.name,state))
            {
                _currentState = registerdState;
                _currentState.Dispatch();
                return true;
            }
        }
        return false;
    }

    public State<T> Find(T state)
    {
        foreach (State<T> registerdState in _stateList)
            if (EqualityComparer<T>.Default.Equals(registerdState.name, state))
                return registerdState;


        return null;
    }
}
