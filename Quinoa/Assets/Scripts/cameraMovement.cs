using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class cameraMovement : NetworkBehaviour
{

    public GameObject player; // player GameObject to allow interaction of camera and player
    public static Transform tf;
    private Vector3 offset;

    // Use this for initialization
    void Start()
    {
        tf = GetComponent<Transform>();
        offset = transform.position - player.transform.position;
        Debug.Log(isLocalPlayer);

    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            //return;
        }

        transform.position = player.transform.position + offset;

        if (transform.rotation.eulerAngles.x < 90 || (transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360) || ((transform.rotation.eulerAngles.x == 90) && (playerMovement.mouseMovementY >= 0)) || ((transform.rotation.eulerAngles.x == 270) && (playerMovement.mouseMovementY <= 0))) //clamp rotation to avoid upside-down view
        {
            Debug.Log(transform.rotation.eulerAngles.x);
            transform.Rotate(new Vector3(-1 * playerMovement.mouseMovementY * Time.deltaTime * playerMovement.sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        }

        if(transform.rotation.eulerAngles.x == 90)
        {

        }
    }

}
