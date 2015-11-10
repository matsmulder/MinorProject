using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {
    public static Rigidbody rb;
    public static float mouseMovementX,mouseMovementY; // mouse input variable which changes the rotation of the player
    public static float sensitivity; // mouse sensitivity
    public float jumpHeight; //the amount of force to add to the player when jumping, this results in a certain jump height

    //direction variables of the player
    private float directionx, directiony, directionz;
    //private float direction;
    public float walkingSpeed;

    //placeholder strings for controlling the direction of the player; this is done to make the inputs rebindable
    public string left, right, backward, forward, jump, dodge;

    //flag for checking if player has contact with the ground; this to avoid air-jumping
    private bool touchingGround;

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
        //lock rotation of player to around the y-axis only, this to avoid 'rolling off' edges
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

        //check for key presses and adapt movement of player

        //rotate with mouse input
        mouseMovementX = Input.GetAxis("Mouse X");
        mouseMovementY = Input.GetAxis("Mouse Y");

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

        //the famous Unreal Tournament single-tap dodge
        if(Input.GetKey(dodge))
        {
            Debug.Log("dodge" + rb.velocity.ToString());
            rb.AddForce(rb.velocity);
        }

        //when no movement keys are pressed, the player is restored into a rest state
        if (Input.anyKey == false)
        {
            directionx = 0;
            directionz = 0;
        }

        if(Input.GetKeyDown(jump) && touchingGround)
        {
            rb.AddForce(Vector3.up*jumpHeight);
        }
        Debug.Log(rb.velocity);
    }

    void OnCollisionStay(Collision col)
    {
        if(col.gameObject.CompareTag("ground"))
        {
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            touchingGround = false;
        }
    }


}
