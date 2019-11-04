﻿using UnityEngine;
using System;
public class WorldEdge : MonoBehaviour
{
    // Start is called before the first frame update
    private float _height;
    private float _width;
    private float _cameraPanValue;
    private bool _isVertical;
    private Vector2 _position;
    private bool _isActive;

    public void Initialize(float height, float width, Vector2 postion , float camerPanValue, bool isVertical = false)
    {
        _width = width;
        _height = height;
        _cameraPanValue = camerPanValue;
        _isVertical = isVertical;
        _position = postion;
        _isActive = true;
    }

    public void setActive(bool status)
    {
        _isActive = status;
    }

    void Start()
    {

        transform.localScale = new Vector3(_width, _height, 1);
        Controller.instance.stateMachine.AddStateListener((System.Object response) => { _isActive = false; }, EngineState.PLAYER_DEAD);
        Camera.main.GetComponent<LevelCamera>().panCompleteEvent.AddListener((System.Object response) => { _isActive = true;});

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x + _position.x, Camera.main.transform.position.y + _position.y, 1);
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (_isActive)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (_isVertical)
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + collision.Distance(this.GetComponent<BoxCollider2D>()).distance * _cameraPanValue, Camera.main.transform.position.z);
                else
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + collision.Distance(this.GetComponent<BoxCollider2D>()).distance * _cameraPanValue, Camera.main.transform.position.y, Camera.main.transform.position.z);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("CamBlocker") && !_isVertical)
                _isActive = false;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("CamBlocker"))
            _isActive = true;
    }

}