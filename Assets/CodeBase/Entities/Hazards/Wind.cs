using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public float windForceX, windForceY;

    const string PLAYER_TAG = "Player";

    private void OnTriggerEnter2D(Collider2D other) {
        WindRoutine(other);
    }
    private void OnTriggerStay2D(Collider2D other) {
        WindRoutine(other);
    }
    private void OnTriggerExit2D(Collider2D other) {
        //remove wind's effect on player velocity
        other.gameObject.GetComponent<Player>().windModifierX = 0;
        other.gameObject.GetComponent<Player>().windModifierY = 0;
    }

    void WindRoutine(Collider2D other) {
        if (other.tag == PLAYER_TAG) {
            if (other.gameObject.GetComponent<Player>().currentAbility != ActiveAbility.ICE) {
                //affect velocity in the player class
                other.gameObject.GetComponent<Player>().windModifierX = 1.0f * windForceX;
                other.gameObject.GetComponent<Player>().windModifierY = 1.0f * windForceY;
                //then apply the force
                if (other.gameObject.GetComponent<Rigidbody2D>().velocity.x == 0) {
                    //other.gameObject.GetComponent<Player>().stopHorizontalMovement();
                }
                if (other.gameObject.GetComponent<Rigidbody2D>().velocity.y == 0) {
                    //other.gameObject.GetComponent<Player>().stopVerticalMovement();
                }
            }
            else {
                //remove wind's effect on player velocity
                other.gameObject.GetComponent<Player>().windModifierX = 0;
                other.gameObject.GetComponent<Player>().windModifierY = 0;
            }
        }
    }
}
