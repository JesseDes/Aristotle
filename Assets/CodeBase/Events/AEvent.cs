using System;
using System.Collections.Generic;
using UnityEngine;

public class AEvent : IEvent
{
    public int size { get { return _listerners.Count; } }
    private List<Action<System.Object>> _listerners;
    readonly bool readOnly;
    String _key;

    public AEvent(bool readOnly = false , String key = null)
    {
        this.readOnly = readOnly;
        _listerners = new List<Action<System.Object>>();
        _key = key;
    }

    public AEvent(AEvent tocopy)
    {
        this.readOnly = tocopy.readOnly;
        _listerners = new List<Action<System.Object>>();
        _key = tocopy._key;

        foreach (Action<System.Object> action in tocopy._listerners)
            _listerners.Add(action);   
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

    public void Dispatch(System.Object data = null, string key = null)
    {

        if (!readOnly || _key == key)
        {
            foreach (Action<System.Object> callback in _listerners)
                callback(data);
        }
        else
            Debug.LogWarning("Readonly event attempted to be dispatched by non owner");
    }

}

