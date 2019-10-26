using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script to be attached to every stage hazard. They will all kill the player unless they
/// have the ice powerup selected.
/// </summary>
public class HazardGeneric : MonoBehaviour
{
    const string PLAYER_TAG = "Player";
    public bool iceImmune; //does having ice power make player immune to this hazard?

    private void OnTriggerEnter2D(Collider2D other) {
        KillPlayer(other);
    }
    private void OnTriggerStay2D(Collider2D other) {
        KillPlayer(other);
    }

    void KillPlayer(Collider2D other) {
        if (other.tag == PLAYER_TAG) {
            if (!iceImmune || other.gameObject.GetComponent<Player>().currentAbility != ActiveAbility.ICE) {
                //kill player
                //<TODO>: code for handling player death

                //for now, only destroy player object..
             
                Destroy(other.gameObject);
            }
        }
    }
}
