using System;
public class ControllerStateMachine
{
    public EngineState state { get { return _stateMachine._currentState.name; } }

    private StateMachine<EngineState> _stateMachine;
    private AEvent _StateChangeEvent;

    public ControllerStateMachine()
    {
        _stateMachine = new StateMachine<EngineState>();
        _stateMachine.RegisterState(new State<EngineState>(EngineState.ACTIVE));
        _stateMachine.RegisterState(new State<EngineState>(EngineState.CUTSCENES));
        _stateMachine.RegisterState(new State<EngineState>(EngineState.LOADING_STATE));
        _stateMachine.RegisterState(new State<EngineState>(EngineState.MENU));
        _stateMachine.RegisterState(new State<EngineState>(EngineState.PLAYER_DEAD));
        _stateMachine.RegisterState(new State<EngineState>(EngineState.STAGE_END));
        _StateChangeEvent = new AEvent();

        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_START, (Object response) => { CheckState(EngineEvents.ENGINE_GAME_START); });
        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_PAUSE, (Object response) => { CheckState(EngineEvents.ENGINE_GAME_PAUSE); });
        Controller.instance.AddEventListener(EngineEvents.ENGINE_GAME_OVER, (Object response) => { CheckState(EngineEvents.ENGINE_GAME_OVER); });
        Controller.instance.AddEventListener(EngineEvents.ENGINE_LOAD_LEVEL, (Object response) => { CheckState(EngineEvents.ENGINE_LOAD_LEVEL); });
        Controller.instance.AddEventListener(EngineEvents.ENGINE_STAGE_COMPLETE, (Object response) => { CheckState(EngineEvents.ENGINE_STAGE_COMPLETE); });
    }

    public void AddStateListener(Action<Object> callback , EngineState? state = null)
    {
        if(state == null)
        {
            _StateChangeEvent.AddListener(callback);
            return;
        }

        State<EngineState> foundState = _stateMachine.Find(state.Value);
        if (foundState != null)
            foundState.AddListener(callback);
    }

    public void RemoveStateLisntener(Action<Object> callback , EngineState? state = null)
    {
        if (state == null)
        {
            _StateChangeEvent.RemoveListener(callback);
            return;
        }

        State<EngineState> foundState = _stateMachine.Find(state.Value);
        if (foundState != null)
            foundState.RemoveListener(callback);
    }

    private void CheckState(EngineEvents type)
    {
        switch (type)
        {
            case EngineEvents.ENGINE_GAME_START: _stateMachine.SetState(EngineState.ACTIVE); break;
            case EngineEvents.ENGINE_STAGE_COMPLETE: _stateMachine.SetState(EngineState.STAGE_END); break;
            case EngineEvents.ENGINE_LOAD_LEVEL: _stateMachine.SetState(EngineState.LOADING_STATE); break;
            case EngineEvents.ENGINE_GAME_PAUSE: _stateMachine.SetState(EngineState.MENU); break;
            case EngineEvents.ENGINE_GAME_OVER: _stateMachine.SetState(EngineState.PLAYER_DEAD); break;

            default: return;
        }

        _StateChangeEvent.Dispatch(type);
    }
}

