using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : MonoBehaviour
{
    string playerTag = "Player";
    float resistance = 10f;

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == playerTag && other.gameObject.GetComponent<Player>().currentAbility.Equals(ActiveAbility.ICE)) {
            if (other.gameObject.GetComponent<Rigidbody2D>().velocity.y < -resistance) {
                //TODO: add visual effects later...

                //break the rock if crashing downwards into it fast enough
                Destroy(this.gameObject);
            }
        }
    }
}
