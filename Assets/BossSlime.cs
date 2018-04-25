using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSlime : MonoBehaviour {
    private Rigidbody2D rb2d;
    public Transform player;

	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        MoveToTarget();
    }
    void MoveToTarget()
    {
        //Debug.Log("BossSlime Moving To Target.");

        float distanceX = Mathf.Clamp((player.position.x - transform.position.x), -1f, 1f);
        float distanceY = Mathf.Clamp((player.position.y - transform.position.y), -1f, 1f);
        rb2d.velocity = new Vector2(distanceX, distanceY);
    }
}
