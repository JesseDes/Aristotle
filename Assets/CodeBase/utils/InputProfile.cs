using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum InputEvent
{
     Down = 0,
     Key = 1,
     Up = 2

}


public class InputProfile : IInputProfile
{
    protected List<InputCommand> keyLoadList; 
    private bool _inputEnabled = true;
    private Dictionary<KeyCode, InputCommand> keyDict;

    public InputProfile()
    {
        keyLoadList = new List<InputCommand>();
    }

    protected void assignKeys(List<InputCommand> keyLoadList)
    {
        keyDict = new Dictionary<KeyCode, InputCommand>();

        foreach (InputCommand command in keyLoadList)
        {
            KeyCode current_key = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(command.name, "" + command.defaultKey)) ;
            keyDict.Add(current_key, command);
        }
    }

    public void checkInput()
    {
        if (Input.anyKeyDown && _inputEnabled)
        {
            foreach (KeyCode key in keyDict.Keys)
            {
                if (Input.GetKeyDown(key))
                    keyDict[key].dispatchDown();
            }
        }

        if (Input.anyKey && _inputEnabled)
        {
            foreach (KeyCode key in keyDict.Keys)
            {
                if (Input.GetKey(key))
                    keyDict[key].dispatchKey();
            }
        }

        if (_inputEnabled)
        {
            foreach (KeyCode key in keyDict.Keys)
            {
                if (Input.GetKeyUp(key))
                    keyDict[key].dispatcUp();
            }
        }
    }

    public void DisableInput()
    {
        _inputEnabled = false;
    }

    public void EnableInput()
    {
        _inputEnabled = true;
    }

    public void addListener(InputEvent eventType, string eventName, Action callback)
    {
        foreach (KeyCode key in keyDict.Keys)
        {
            if (keyDict[key].name == eventName)
            {
                if (eventType == InputEvent.Down)
                    keyDict[key].addDownEvent(callback);
                else if (eventType == InputEvent.Key)
                    keyDict[key].addKeyEvent(callback);
                else if (eventType == InputEvent.Up)
                    keyDict[key].addUpEvent(callback);

                break;
            }
        }
    }




}
