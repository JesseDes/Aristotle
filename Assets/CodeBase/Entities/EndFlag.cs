﻿using UnityEngine;
using System;


public class EndFlag : MonoBehaviour
{
    public float animationTime;
    public float MessageTime;
    public GameObject effectPrefab;
    public Vector3 playerDestination;
    public GameObject _cavnas;
    
    private GameObject _player;
    private float _lerpTimer;
    private Vector3 _startPos;
    private float _postTimer;
    private bool _isPost;

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

        if(_isPost)
        {
            _postTimer += Time.deltaTime;

            if (_postTimer >= MessageTime)
                onCutsceneEnd();
        }
    }

    private void showMessage()
    {
        _isPost = true;
        _cavnas.SetActive(true);
    }

    private void onCutsceneEnd()
    {
        Debug.Log("level end");
        _isPost = false;
        Controller.instance.Dispatch(EngineEvents.ENGINE_CUTSCENE_END);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Controller.instance.Dispatch(EngineEvents.ENGINE_CUTSCENE_START);

        _player = collision.gameObject;
        _startPos = _player.transform.position;

        if (effectPrefab.gameObject.GetComponent<ParticleSystem>() != null)
        {
            GameObject animation = Instantiate(effectPrefab, _player.gameObject.transform);
            Invoke("showMessage", animation.gameObject.GetComponent<ParticleSystem>().main.duration);
        }
    }
}
