using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/Audio Profile")]
public class AudioProfile : ScriptableObject
{
    public List<AudioData> audioData;
    public string profileKey;
}

[Serializable]
public class AudioData
{
    public string accessKey;
    public AudioClip clip;
    public bool loop;

}
