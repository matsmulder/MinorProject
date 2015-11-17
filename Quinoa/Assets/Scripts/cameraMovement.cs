using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class cameraMovement : NetworkBehaviour
{

    //public GameObject player; // player GameObject to allow interaction of camera and player
    public static Transform tf;
    private float previousRotationY;
    private float previousRotationX;

    // Use this for initialization
    void Start()
    {
        tf = GetComponent<Transform>();
        previousRotationY = 0;
        previousRotationX = 0;

        //fix camera orientation
        //transform.rotation = playerMovement.rb.rotation;
        //transform.Rotate(new Vector3(0, 90, 0));
        //Debug.Log(transform.rotation.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            //return;
        }


        previousRotationY = transform.rotation.eulerAngles.y;
        previousRotationX = transform.rotation.eulerAngles.x;

        //if-statement with angle conditions to prevent overshooting vertical camera rotation position
        if (transform.rotation.eulerAngles.x < 90 || (transform.rotation.eulerAngles.x > 270 && transform.rotation.eulerAngles.x <= 360) || ((transform.rotation.eulerAngles.x == 90) && (playerMovement.mouseMovementY >= 0)) || ((transform.rotation.eulerAngles.x == 270) && (playerMovement.mouseMovementY <= 0))) //clamp rotation to avoid upside-down view
        {
            transform.Rotate(new Vector3(-1 * playerMovement.mouseMovementY * Time.deltaTime * playerMovement.sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        }


        if(Mathf.Abs(transform.rotation.eulerAngles.y - previousRotationY) >= 170) // bij een verticale mouse overshoot maakt de y-rotatie een sprong van ongeveer 180 graden
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
        }

        //follow the player movement
        transform.position = playerMovement.rb.position;

        //follow player rotation
        transform.Rotate(new Vector3(0, playerMovement.mouseMovementX * Time.deltaTime * playerMovement.sensitivity, 0), Space.World);
        transform.rotation = playerMovement.rb.rotation;
    }

}
