using System.Collections.Generic;
using System;
using UnityEngine;
public class InputCommand
{
    public string name { get; }
    public KeyCode key { get; set; }
    public KeyCode defaultKey { get; }

    private List<Action> keyCallbackList;
    private List<Action> upCallbackList;
    private List<Action> downCallbackList;

    public InputCommand(string name , KeyCode defaultKey)
    {
        this.name = name;
        keyCallbackList = new List<Action>();
        upCallbackList = new List<Action>();
        downCallbackList = new List<Action>();
        this.defaultKey = defaultKey;
    }

    public void addKeyEvent(Action callback)
    {
        keyCallbackList.Add(callback);
    }

    public void addUpEvent(Action callback)
    {
        upCallbackList.Add(callback);
    }
    public void addDownEvent(Action callback)
    {
        downCallbackList.Add(callback);
    }

    public void removeUpEvent(Action callback)
    {
        upCallbackList.Remove(callback);
    }

    public void removeKeyEvent(Action callback)
    {
        keyCallbackList.Remove(callback);
    }

    public void dispatchDown()
    {
       foreach(Action callback in downCallbackList)
       {
            callback.DynamicInvoke();               // make parellel?
       }
    }

    public void dispatchKey()
    {
        foreach (Action callback in keyCallbackList)
        {
            callback.DynamicInvoke();               // make parellel?
        }
    }

    public void dispatcUp()
    {
        foreach (Action callback in upCallbackList)
        {
            callback.DynamicInvoke();               // make parellel?
        }
    }

}
