using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawn : MonoBehaviour {

    public GameObject gameManager;
    public GameObject slimePrefab;

    void Start () {
        List<GameObject> slimeSpawns = gameManager.GetComponent<GameManager>().slimeSpawns;
        slimeSpawns.Add(gameObject);
        name = "SlimeSpawn"; 
    }
    void Update () {
		
	}
    // EXECUTED FROM GAME MANAGER
    public void SpawnSlime()
    {
        //Debug.Log("Slime Instantiated via SlimeSpawn!");
        Instantiate(slimePrefab, transform.position, Quaternion.identity);
    }
}
