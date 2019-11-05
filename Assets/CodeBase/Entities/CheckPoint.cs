using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnAnimPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawn()
    {
        if (spawnAnimPrefab)
        {
            float time;
            time = spawnAnimPrefab.GetComponent<ParticleSystem>().main.duration;
            Instantiate(spawnAnimPrefab, transform);
            Invoke("playerSpawn", time);
        }
        else
            playerSpawn();
    }

    private void playerSpawn()
    {
        Instantiate(playerPrefab,transform).GetComponent<Player>().init();        
    }
}
