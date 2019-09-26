using System.Collections.Generic;
using System;
using UnityEngine;
public class InputCommand
{
    public string name { get; }
    public KeyCode key { get; set; }
    public KeyCode defaultKey { get; }

    private List<Action> callbackList;

    public InputCommand(string name , KeyCode defaultKey)
    {
        this.name = name;
        callbackList = new List<Action>();
        this.defaultKey = defaultKey;
    }

    public void addEvent(Action callback)
    {
        callbackList.Add(callback);
    }

    public void removeEvent(Action callback)
    {
        callbackList.Remove(callback);
    }

    public void dispatch()
    {
       foreach(Action callback in callbackList)
       {
            callback.DynamicInvoke();               // make parellel?
       }
    }

}
