using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject _canvas;

    private bool _isColliding = false;
    private PlayerInputProfile inputProfile;

    private void Update()
    {
        if (_isColliding)
            inputProfile.checkInput();
    }

    public void Start()
    {
        inputProfile = new PlayerInputProfile();
        inputProfile.addListener(InputEvent.Down, PlayerInputProfile.shift, ShowLore);
        inputProfile.addListener(InputEvent.Up, PlayerInputProfile.shift, HideLore);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isColliding = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isColliding = false;
        HideLore();
    }

    private void ShowLore()
    {
        _canvas.SetActive(true);
    }

    private void HideLore()
    {
        _canvas.SetActive(false);
    }
}
