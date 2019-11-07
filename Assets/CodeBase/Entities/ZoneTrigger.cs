using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public CheckPoint newCheckpoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            Camera.main.GetComponent<LevelCamera>().FullScreenPan(collision.GetComponent<Player>().facingDireciont);
            Model.instance.SetCheckPoint(newCheckpoint);

        }
    }

}
