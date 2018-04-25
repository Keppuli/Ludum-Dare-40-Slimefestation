using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontalCollider : MonoBehaviour {

    public GameObject player;
    private void Start()
    {
        // Search and add player
        player = GameObject.Find("Player");
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Slime" || col.gameObject.name == "Buyer")
        {
            //Debug.Log("Slime added for player use. ");
            player.GetComponent<Player>().objInFront = col.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
      
            player.GetComponent<Player>().objInFront = null;
       
    }
}
