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
            collision.gameObject.GetComponentInParent<Player>().windForce = _windForce;
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Player") 
        {
            if (collision.gameObject.GetComponentInParent<Player>().currentAbility != ActiveAbility.ICE &&
                !collision.gameObject.GetComponentInParent<Player>().isHuggingWall())
            {
                if(collision.gameObject.GetComponentInParent<Player>().currentAbility != ActiveAbility.WIND)
                    collision.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(_windForce * 2);
                else
                    collision.gameObject.GetComponentInParent<Rigidbody2D>().AddForce(_windForce * 3);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.gameObject.GetComponentInParent<Player>().windForce = Vector2.zero;
    }
}
