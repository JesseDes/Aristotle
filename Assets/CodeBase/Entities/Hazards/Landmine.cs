using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    public GameObject explosion;
    bool detonating = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == PLAYER_TAG && !detonating) {
            detonating = true;
            Invoke("Detonate", 0.5f);
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == PLAYER_TAG && !detonating) {
            detonating = true;
            Invoke("Detonate", 0.5f);
        }
    }

    public void Detonate() {
        GameObject e = Instantiate(explosion, transform);
        e.transform.localPosition = Vector3.zero;
        e.transform.parent = null;
        Destroy(this.gameObject);
    }
}
