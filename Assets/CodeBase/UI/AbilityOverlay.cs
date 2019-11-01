using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityOverlay : MonoBehaviour
{
    [SerializeField]
    private AbilitySymbol _fireSybol = default;
    [SerializeField]
    private AbilitySymbol _iceSymbol = default;
    [SerializeField]
    private AbilitySymbol _airSymbol = default;
    [SerializeField]
    private AbilitySymbol _earthSymbol = default;

 

    public void UpdateAbilitySymbol(ActiveAbility ability, AbilitySymbolState state)
    {
        switch(ability)
        {
            case ActiveAbility.EARTH:
                _earthSymbol.CurrentState = state;
                return;
            case ActiveAbility.FIRE:
                _fireSybol.CurrentState = state;
                return;
            case ActiveAbility.ICE:
                _iceSymbol.CurrentState = state;
                return;
            case ActiveAbility.WIND:
                _airSymbol.CurrentState = state;
                return;
        }
    }
}
