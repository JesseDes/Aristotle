using System;
using UnityEngine;
/// <summary>
/// States are Types of Events where they broadcast their identifiers.
/// This State will dispatch NO arguments 
/// </summary>
/// <typeparam name="T">Type of Identifier </typeparam>
public class State<T> : AEvent              
{
    public T name { get; private set; }
 
    public State(T stateName) 
    {
        name = stateName;

    }

}

/// <summary>
/// States are Types of Events where they broadcast their identifiers
/// This State will Dispatch an argument (Make a payload class if you need to push multiple values)
/// </summary>
/// <typeparam name="T">Type of Identifier </typeparam>
/// <typeparam name="X"> This is the Value you wish to dispatch</typeparam>
public class State<T,X> : AEvent
{
    public T name { get; private set; }

    readonly X _returnValue;

    public State(T stateName, X returnValue)
    {
        name = stateName;
        _returnValue = returnValue;
    }

    public void Dispatch()
    {
        Dispatch(_returnValue);
    }

}
