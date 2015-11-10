using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {
    public static Rigidbody rb;
    public static float mouseMovementX,mouseMovementY; // mouse input variable which changes the rotation of the player
    public static float sensitivity; // mouse sensitivity

    //direction variables of the player
    private float directionx, directiony, directionz;
    //private float direction;
    public float walkingSpeed;

    //placeholder strings for controlling the direction of the player; this is done to make the inputs rebindable
    public string left, right, backward, forward;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        sensitivity = 120; // initialize at default sensitivity; to be tweakable live in the future
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 direction = new Vector3(directionx*walkingSpeed*Time.deltaTime, directiony*walkingSpeed*Time.deltaTime, directionz*walkingSpeed*Time.deltaTime);
        transform.Translate(direction);

        //two separate rotations, a global and local one to fix bugs
        transform.Rotate(new Vector3(0,mouseMovementX*Time.deltaTime*sensitivity,0), Space.World); // rotate the camera when moving the mouse left and right
        //transform.Rotate(new Vector3(-1 *mouseMovementY * Time.deltaTime * sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        
 
        //check for key presses and adapt movement of player


        //move forward
        if (Input.GetKey(forward))
        {
            directionz = 1;
        }

        //move left
        if (Input.GetKey(left))
        {
            directionx = -1;
        }

        //move backward
        if (Input.GetKey(backward))
        {
            directionz = -1;
        }

        //move right
        if (Input.GetKey(right))
        {
            directionx = 1;
        }

        if (Input.anyKey == false)
        {
            directionx = 0;
            directionz = 0;
        }

        //rotate with mouse input
        mouseMovementX = Input.GetAxis("Mouse X");
        mouseMovementY = Input.GetAxis("Mouse Y");


        print(Input.GetAxis("Mouse X").ToString());
    }


}
