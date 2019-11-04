using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OptionsMenu : MonoBehaviour
{
    private bool _isListening;
    private string _preferenceToSet;

    public void Update()
    {
        if (Input.anyKeyDown && _isListening)
        {
 
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    Debug.Log("KeyCode set: " + kcode);
                    _isListening = false;
                    Debug.Log("Keycode string: "  + kcode.ToString());
                    SetKeyForPreference(kcode.ToString());
                }
                   
            }

        }
    }

    public void UI_SetKeyForJump()
    {
        UpdatePreferenceSelection(PlayerInputProfile.jump);
    }

    public void UpdatePreferenceSelection(string newPreference)
    {
        if (newPreference == _preferenceToSet)
        {
            //Same button selected, cancel 
            _preferenceToSet = null;
            _isListening = false;
        }
        else
        {
            _preferenceToSet = newPreference;
            _isListening = true;
        }
    }

    public void SetKeyForPreference(string input)
    {
        Debug.Log("Setting " + _preferenceToSet + " with " + input);
        PlayerPrefs.SetString(_preferenceToSet, input);
        PlayerPrefs.Save();
        _preferenceToSet = null;
    }


}
