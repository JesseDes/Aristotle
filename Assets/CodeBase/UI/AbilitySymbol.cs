using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AbilitySymbolState
{
    Locked,
    Available, 
    Unavailable
}

public class AbilitySymbol : MonoBehaviour
{
    [SerializeField]
    private Sprite _lockedSprite;
    
    [SerializeField]
    private Sprite _availableSprite;

    [SerializeField]
    private Sprite _unavailableSprite;

    [SerializeField]
    private Image _mainImage;

    private AbilitySymbolState _currState = AbilitySymbolState.Locked;
    private Dictionary<AbilitySymbolState, Sprite> _imageDict = new Dictionary<AbilitySymbolState, Sprite>();

    public AbilitySymbolState CurrentState
    {
        get
        {
            return _currState;
        }
        set
        {
            _currState = value;
            UpdateImage();
        }
    }

    public void Start()
    {
        _imageDict.Add(AbilitySymbolState.Locked, _lockedSprite);
        _imageDict.Add(AbilitySymbolState.Available, _availableSprite);
        _imageDict.Add(AbilitySymbolState.Unavailable, _unavailableSprite);

        UpdateImage();
    }

    public void UpdateImage()
    {
        if (_imageDict.ContainsKey(_currState))
        {
            _mainImage.sprite = _imageDict[_currState];
        }
    }
}
