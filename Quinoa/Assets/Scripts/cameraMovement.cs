using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class cameraMovement : NetworkBehaviour
{

    public GameObject player; // player GameObject to allow interaction of camera and player
    public static Transform tf;
    private Vector3 offset;
    private float previousRotationY;
    private float previousRotationX;

    // Use this for initialization
    void Start()
    {
        tf = GetComponent<Transform>();
        offset = transform.position - player.transform.position;
        Debug.Log(isLocalPlayer);
        previousRotationY = 0;
        previousRotationX = 0;

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

        previousRotationY = transform.rotation.eulerAngles.y;
        previousRotationX = transform.rotation.eulerAngles.x;

        //if-statement with angle conditions to prevent overshooting vertical camera rotation position
        if (transform.rotation.eulerAngles.x < 90 || (transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360) || ((transform.rotation.eulerAngles.x == 90) && (playerMovement.mouseMovementY >= 0)) || ((transform.rotation.eulerAngles.x == 270) && (playerMovement.mouseMovementY <= 0))) //clamp rotation to avoid upside-down view
        {
            transform.Rotate(new Vector3(-1 * playerMovement.mouseMovementY * Time.deltaTime * playerMovement.sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        }


        if(Mathf.Abs(transform.rotation.eulerAngles.y - previousRotationY) >= 170)
        {
            Vector3 tmp = transform.eulerAngles;

            if (previousRotationX > 270 && previousRotationX <= 360) // overshoot aan de bovenkant
            {
                tmp.x = 270;
            }
            else
            {
                tmp.x = 90;
            }
            transform.eulerAngles = tmp;
            Debug.Log("overshoot");
            Debug.Log(previousRotationX);
        }

        if(transform.rotation.eulerAngles.x == 90)
        {

        }
    }

}
