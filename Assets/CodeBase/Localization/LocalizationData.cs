using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Localization Data")]
public class LocalizationData : ScriptableObject
{
    public List<LanguageData> LanguageData;
}

[Serializable]
public class LanguageData
{
    public string LanguageCode;
    public LocalizationKeyData Data;
}
