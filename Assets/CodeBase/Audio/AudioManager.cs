using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manages AudioProfiles allowing you to set and load in multiple audio profiles 
/// </summary>
public class AudioManager : MonoBehaviour
{
    private Dictionary<string,Dictionary<string, AudioData>> audioProfileList;
    private AudioSource _audioPlayer;
    
    public int volume = 1;

    public void init()
    {
        audioProfileList = new Dictionary<string, Dictionary<string, AudioData>>();
        _audioPlayer = gameObject.AddComponent<AudioSource>();
        Controller.instance.stateMachine.AddStateListener(clearLevelAudio, EngineState.LOADING_STATE);
    }

    public void LoadProfile(AudioProfile profile)
    {
        if (audioProfileList == null)
            Debug.Log("why??");
        audioProfileList.Add(profile.profileKey, new Dictionary<string, AudioData>());
        foreach (AudioData data in profile.audioData)
            audioProfileList[profile.profileKey].Add(data.accessKey, data);
    }

    public void ClearProfiles(string profileKey)
    {
        audioProfileList.Remove(profileKey);
    }

    public void PlaySound(string profileKey, string audioKey ,Vector3 location)
    {
        AudioSource.PlayClipAtPoint(audioProfileList[profileKey][audioKey].clip, new Vector3(location.x, location.y, 0), volume);
    }

    public void PlayBackgroundMusic(string profileKey, string audioKey)
    {

        _audioPlayer.clip = audioProfileList[profileKey][audioKey].clip;
        _audioPlayer.loop = true;
        _audioPlayer.Play();
        
    }

    public void setVolume(int level)
    {
        _audioPlayer.volume = level;
        volume = level;
    }

    private void clearLevelAudio(System.Object response)
    {
        _audioPlayer.Stop();

        List<string> keyList = new List<string>(audioProfileList.Keys);
        foreach (var key in keyList)
            if (key != Model.instance.globalAudio.profileKey)
                audioProfileList.Remove(key);

    }
}
