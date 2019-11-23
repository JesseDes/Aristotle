using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public void UI_OpenLink(string url)
    {
        Application.OpenURL(url);
    }
}
