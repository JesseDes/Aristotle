using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputProfile : IInputProfile
{
    protected List<InputCommand> keyLoadList; 

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
        foreach(KeyCode key in keyDict.Keys)
        {
            if (Input.GetKey(key))
                keyDict[key].dispatch();
        }   
    }

    public void addListener(string eventName, Action callback)
    {
        foreach (KeyCode key in keyDict.Keys)
        {
            if (keyDict[key].name == eventName)
            {
                keyDict[key].addEvent(callback);
                break;
            }
        }
    }




}
