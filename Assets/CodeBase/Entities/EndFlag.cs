using UnityEngine;
using System;


public class EndFlag : MonoBehaviour
{
    public float animationTime;
    public float MessageTime;
    public GameObject effectPrefab;
    public Vector3 playerDestination;
    public GameObject _cavnas;
    public int unlockAbility;
    public bool isEnd;
    
    private GameObject _player;
    private float _lerpTimer;
    private Vector3 _startPos;
    private float _postTimer;
    private bool _isPostAnimationPhase;

    private void Start()
    {
        _cavnas.SetActive(false);
    }

    private void Update()
    {
        if(_player != null)
        {
            _lerpTimer += Time.deltaTime;
            _player.gameObject.transform.position = Vector3.Lerp(_startPos, playerDestination, Math.Min(_lerpTimer, animationTime) / animationTime);

            if (effectPrefab.gameObject.GetComponent<ParticleSystem>() == null && _player.gameObject.transform.position == playerDestination)
                showMessage();   
        }

        if(_isPostAnimationPhase)
        {
            _postTimer += Time.deltaTime;

            if (_postTimer >= MessageTime)
                onCutsceneEnd();
        }
    }

    private void showMessage()
    {
        Destroy(_player.gameObject);
        _isPostAnimationPhase = true;
        _cavnas.SetActive(true);
    }

    private void onCutsceneEnd()
    {
        Model.instance.audioManager.StopBackgroundMusic();
        _cavnas.SetActive(false);
        PlayerPrefs.SetInt(SaveKeys.ACTIVE_ABILITIES ,unlockAbility);
        _isPostAnimationPhase = false;
        _lerpTimer = 0;

        if (!isEnd)
        {
            Controller.instance.Dispatch(EngineEvents.ENGINE_CUTSCENE_END);
        }
        else
        {
            View.instance.ShowMainMenu();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Controller.instance.Dispatch(EngineEvents.ENGINE_CUTSCENE_START);
        Model.instance.audioManager.PlayBackgroundMusic(Model.instance.currentLevelProfile.profileKey, "endSong");
        _player = collision.transform.parent.gameObject;
        _startPos = _player.transform.position;

        if (effectPrefab.gameObject.GetComponent<ParticleSystem>() != null)
        {
            GameObject animation = Instantiate(effectPrefab, _player.gameObject.transform);
            Invoke("showMessage", animation.gameObject.GetComponent<ParticleSystem>().main.duration);
        }
    }
}
