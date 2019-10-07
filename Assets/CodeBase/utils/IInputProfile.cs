using System.Collections;
using System.Collections.Generic;
using System;

public interface IInputProfile 
{
    void checkInput();
    void addListener(InputEvent eventType ,string eventName, Action callback);
        
}
