using System;
using System.Collections.Generic;

public class AEvent : IEvent
{
    private List<Action<System.Object>> _listerners;
    readonly bool readOnly;


    public AEvent(bool readOnly = false)
    {
        this.readOnly = readOnly;
        _listerners = new List<Action<System.Object>>();
    }

    public void AddListener(Action<System.Object> listener)
    {
        if (!_listerners.Contains(listener))
            _listerners.Add(listener);
    }

    public bool RemoveListener(Action<System.Object> listener)
    {
        return _listerners.Remove(listener);
    }

    public void RemoveAll()
    {
        _listerners = new List<Action<System.Object>>();
    }

    public void Dispatch(System.Object data = null)
    {
        foreach (Action<System.Object> callback in _listerners)
                callback(data);
        
    }

}

