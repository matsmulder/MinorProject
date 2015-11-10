using UnityEngine;
using System.Collections;

public class cameraMovement : MonoBehaviour
{

    public GameObject player; // player GameObject to allow interaction of camera and player
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;
        transform.Rotate(new Vector3(-1 * playerMovement.mouseMovementY * Time.deltaTime * playerMovement.sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
    }
}
