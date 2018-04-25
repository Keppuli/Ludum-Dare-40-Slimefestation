using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour {

    public GameObject gameManager;
	// Use this for initialization
	void Start () {
        transform.parent = GameObject.Find("STATIC").transform; // Organize Unity UI
        gameManager = GameObject.Find("GameManager");   // Find and set GM
        name = "Splash"; // Set name after instancing
        List<GameObject> splashes = gameManager.GetComponent<GameManager>().splashes;
        splashes.Add(gameObject);
    }

}
