using System;


public interface IEvent
{
    void AddListener(Action<Object> listener);
    bool RemoveListener(Action<Object> listener);
    void RemoveAll();
    void Dispatch(Object data = null);
}

