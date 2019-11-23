using System.Collections;
using System;
using UnityEngine;

public class WindTunnel : MonoBehaviour
{
    public Vector2 windDirection;
    public float windStrength;

    private Vector2 _windForce;

    private void Start()
    {
        _windForce = windDirection.normalized * windStrength;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<Player>().windForce = _windForce;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Player") 
        {
            if (collision.gameObject.GetComponent<Player>().currentAbility != ActiveAbility.ICE &&
                !collision.gameObject.GetComponent<Player>().isHuggingWall())
            {
                if(collision.gameObject.GetComponent<Player>().currentAbility != ActiveAbility.WIND)
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(_windForce * 2);
                else
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(_windForce * 3);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponent<Player>().windForce = Vector2.zero;
    }
}
