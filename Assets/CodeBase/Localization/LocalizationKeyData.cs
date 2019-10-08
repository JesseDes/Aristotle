using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LanguageData", menuName = "Localization/Language Data")]
public class LocalizationKeyData : ScriptableObject
{
    public List<LocalizationKey> Translations;
}

[Serializable]
public class LocalizationKey
{
    public string Key;
    public string Value;
}
