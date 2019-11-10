using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject spawnAnimPrefab;
    public string ID; //System.Guid.NewGuid().ToString();
    public bool startPoint = false;
    public Vector2 CameraPanPosition;

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
        Instantiate(playerPrefab,transform.position,transform.rotation).GetComponent<Player>().init();
        Controller.instance.Dispatch(EngineEvents.ENGINE_GAME_START);
    }
}
