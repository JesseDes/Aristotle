using UnityEngine;
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
    private bool _isBlocked;

    public void Initialize(float height, float width, Vector2 postion , float camerPanValue, bool isVertical = false)
    {
        _width = width;
        _height = height;
        _cameraPanValue = camerPanValue;
        _isVertical = isVertical;
        _position = postion;
        _isActive = true;
        _isBlocked = false;
    }

    public void setActive(bool status)
    {
        _isActive = status;
    }

    void Start()
    {

        transform.localScale = new Vector3(_width, _height, 1);
        Camera.main.GetComponent<LevelCamera>().panStartEvent.AddListener(Disable);
        Camera.main.GetComponent<LevelCamera>().panCompleteEvent.AddListener(Enable);

    }

    private void Disable(System.Object response)
    {
        _isActive = false;
    }

    private void Enable(System.Object response)
    {
        _isActive = true;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x + _position.x, Camera.main.transform.position.y + _position.y, 1);
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("CamBlocker") && !_isVertical)
            _isBlocked = true;

        if (_isActive && !_isBlocked)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (_isVertical)
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + collision.Distance(this.GetComponent<BoxCollider2D>()).distance * _cameraPanValue * Time.deltaTime, Camera.main.transform.position.z); 
                else
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + collision.Distance(this.GetComponent<BoxCollider2D>()).distance * _cameraPanValue * Time.deltaTime, Camera.main.transform.position.y, Camera.main.transform.position.z);
            }

        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("CamBlocker"))
            _isBlocked = false;
    }

}
