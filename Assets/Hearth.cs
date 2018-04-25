using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hearth : MonoBehaviour {

    public float moveSpeed = 0.1f;
    private float fadeTimer = 1f;
    public float fadeMultiplier = 2f;
    private new SpriteRenderer renderer;

    private void Start()
    {
        // Search and add player, as the other methods are not working with this prefab
        renderer = GetComponent<SpriteRenderer>();
        // Set orientation and move direction 
        //GameObject _temp = GameObject.Find("TEMP"); // Automatically connect reference. Used because this script is modular
        //transform.SetParent(_temp.transform);
    }
    // Update is called once per frame
    void Update()
    {
        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime * fadeMultiplier;
        }
        renderer.color = new Color(1, 1, 1, fadeTimer);
        if (fadeTimer <= 0)
        {
            //Debug.Log("Smoke destroyed.");
            Destroy(gameObject);
        }
        // Move smoke
        transform.position += transform.up * moveSpeed;
    }
}