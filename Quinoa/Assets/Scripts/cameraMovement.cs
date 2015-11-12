using UnityEngine;
using System.Collections;

public class cameraMovement : MonoBehaviour
{

    public GameObject player; // player GameObject to allow interaction of camera and player
    public static Transform tf;
    private Vector3 offset;

    private float rotationZ = 0f;
    private float sensitivityZ = 2f;

    // Use this for initialization
    void Start()
    {
        tf = GetComponent<Transform>();
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;

        if (transform.rotation.eulerAngles.x < 90 || (transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360) || ((transform.rotation.eulerAngles.x == 90) && (playerMovement.mouseMovementY >= 0)) || ((transform.rotation.eulerAngles.x == 270) && (playerMovement.mouseMovementY <= 0))) //clamp rotation to avoid upside-down view
        { 
            transform.Rotate(new Vector3(-1 * playerMovement.mouseMovementY * Time.deltaTime * playerMovement.sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        }

        if(transform.rotation.eulerAngles.x == 90)
        {

        }
    }

}
