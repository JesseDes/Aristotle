using UnityEngine;
using System;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    private bool _isListening;
    private string _preferenceToSet;

    [SerializeField]
    private TextMeshProUGUI _jumpText;
    [SerializeField]
    private TextMeshProUGUI _leftText;
    [SerializeField]
    private TextMeshProUGUI _rightText;
    [SerializeField]
    private TextMeshProUGUI _upText;
    [SerializeField]
    private TextMeshProUGUI _downText;
    [SerializeField]
    private TextMeshProUGUI _windText;
    [SerializeField]
    private TextMeshProUGUI _earthText;
    [SerializeField]
    private TextMeshProUGUI _fireText;
    [SerializeField]
    private TextMeshProUGUI _iceText;

    public void Start()
    {
        UpdateText();
    }

    public void Update()
    {
        if (Input.anyKeyDown && _isListening)
        {
 
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(code))
                {
                    _isListening = false;
                    SetKeyForPreference(code.ToString());
                }
                   
            }

        }
    }

    private void UpdateText()
    {
        _jumpText.text = PlayerPrefs.GetString(PlayerInputProfile.jump, "" + PlayerInputProfile.Default_jump);
        _leftText.text = PlayerPrefs.GetString(PlayerInputProfile.moveLeft, "" + PlayerInputProfile.Default_moveLeft);
        _rightText.text = PlayerPrefs.GetString(PlayerInputProfile.moveRight, "" + PlayerInputProfile.Default_moveRight);
        _upText.text = PlayerPrefs.GetString(PlayerInputProfile.moveUp, "" + PlayerInputProfile.Default_moveUp);
        _downText.text = PlayerPrefs.GetString(PlayerInputProfile.moveDown, "" + PlayerInputProfile.Default_moveDown);
        _fireText.text = PlayerPrefs.GetString(PlayerInputProfile.toggleFire, "" + PlayerInputProfile.Default_ToggleFire);
        _iceText.text = PlayerPrefs.GetString(PlayerInputProfile.toggleIce, "" + PlayerInputProfile.Default_ToggleIce);
        _windText.text = PlayerPrefs.GetString(PlayerInputProfile.toggleWind, "" + PlayerInputProfile.Default_ToggleWind);
        _earthText.text = PlayerPrefs.GetString(PlayerInputProfile.toggleEarth, "" + PlayerInputProfile.Default_ToggleEarth);
    }

    public void UI_SetKeyForJump()
    {
        UpdatePreferenceSelection(PlayerInputProfile.jump);
    }

    public void UI_SetKeyForLeft()
    {
        UpdatePreferenceSelection(PlayerInputProfile.moveLeft);
    }

    public void UI_SetKeyForRight()
    {
        UpdatePreferenceSelection(PlayerInputProfile.moveRight);
    }

    public void UI_SetKeyForUp()
    {
        UpdatePreferenceSelection(PlayerInputProfile.moveUp);
    }

    public void UI_SetKeyForDown()
    {
        UpdatePreferenceSelection(PlayerInputProfile.moveDown);
    }

    public void UI_SetKeyForIce()
    {
        UpdatePreferenceSelection(PlayerInputProfile.toggleIce);
    }

    public void UI_SetKeyForFire()
    {
        UpdatePreferenceSelection(PlayerInputProfile.toggleFire);
    }

    public void UI_SetKeyForWind()
    {
        UpdatePreferenceSelection(PlayerInputProfile.toggleWind);
    }

    public void UI_SetKeyForEarth()
    {
        UpdatePreferenceSelection(PlayerInputProfile.toggleEarth);
    }

    public void UI_ResetAll()
    {
        //Clear all related keys, default binding will be selected instead
        PlayerPrefs.DeleteKey(PlayerInputProfile.jump);
        PlayerPrefs.DeleteKey(PlayerInputProfile.moveUp);
        PlayerPrefs.DeleteKey(PlayerInputProfile.moveDown);
        PlayerPrefs.DeleteKey(PlayerInputProfile.moveLeft);
        PlayerPrefs.DeleteKey(PlayerInputProfile.moveRight);
        PlayerPrefs.DeleteKey(PlayerInputProfile.toggleEarth);
        PlayerPrefs.DeleteKey(PlayerInputProfile.toggleFire);
        PlayerPrefs.DeleteKey(PlayerInputProfile.toggleIce);
        PlayerPrefs.DeleteKey(PlayerInputProfile.toggleWind);
        PlayerPrefs.Save();
        UpdateText();
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
            //different button selected, replace
            _preferenceToSet = newPreference;
            _isListening = true;
        }
    }

    public void SetKeyForPreference(string input)
    {
        PlayerPrefs.SetString(_preferenceToSet, input);
        PlayerPrefs.Save();
        _preferenceToSet = null;
        UpdateText();
    }
}
