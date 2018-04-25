using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Transform player;       //Public variable to store a reference to the player game object
    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    public float smoothSpeed = 0.125f;
    public int cameraZ = -10;

    void FixedUpdate()
    {
        
        Vector3 targetPos = player.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos,smoothSpeed);
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = new Vector3(Mathf.Clamp(smoothedPos.x, 0f, 0f), Mathf.Clamp(smoothedPos.y, -1.3f, 1f), cameraZ);
 
    }
}
