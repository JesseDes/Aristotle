using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : Respawnable
{
    string playerTag = "Player";
    float resistance = 10f;

    [SerializeField]
    private GameObject _vfx;

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == playerTag && other.gameObject.GetComponentInParent<Player>().currentAbility.Equals(ActiveAbility.ICE)) {
            if (other.gameObject.GetComponentInParent<Rigidbody2D>().velocity.y < -resistance) {
                GameObject vfx = Instantiate(_vfx, transform);
                vfx.transform.localPosition = Vector3.zero;
                vfx.transform.parent = null;

                //break the rock if crashing downwards into it fast enough
                Deactivate();
            }
        }
    }
}
