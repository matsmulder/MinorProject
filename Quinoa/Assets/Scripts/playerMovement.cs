using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour {
    public static Rigidbody rb;
    //public static Transform tf;

    private bool dodgeFlag; //flag holding information when a dodge is performed
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

    //variables to tune the single-tap dodge move
    public float dodgeSpeed, dodgeHeight;

	// Use this for initialization
	void Start () {
        //Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        //tf = GetComponent<Transform>();
        sensitivity = 120; // initialize at default sensitivity; to be tweakable live in the future
	}
	
	// Update is called once per frame
	void Update () {
        //MOVE!
        Vector3 direction = new Vector3(directionx*walkingSpeed*Time.deltaTime, directiony*walkingSpeed*Time.deltaTime, directionz*walkingSpeed*Time.deltaTime);
        transform.Translate(direction);

        // rotate the camera when moving the mouse left and right, up and down
        transform.Rotate(new Vector3(0,mouseMovementX*Time.deltaTime*sensitivity,0), Space.World); 
        
        //transform.Rotate(new Vector3(-1 *mouseMovementY * Time.deltaTime * sensitivity, 0, 0), Space.Self); // rotate the camera when moving the mouse up and down
        //lock rotation of player to around the y-axis only, this to avoid 'rolling off' edges
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));


        //rotate with mouse input
        mouseMovementX = Input.GetAxis("Mouse X");
        mouseMovementY = Input.GetAxis("Mouse Y");

        //this fixes unwanted rotation caused by contact with a ramp
        if(mouseMovementX == 0)
        {
            rb.angularVelocity = Vector3.zero;
        }
        //check for key presses and adapt movement of player but only if the ground is touched
        if (touchingGround)
        {
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

            //stop moving in certain direction when a key is no longer pressed
            if (Input.GetKeyUp(forward) || Input.GetKeyUp(left) || Input.GetKeyUp(backward) || Input.GetKeyUp(right))
            {
                directionx = 0;
            }


            //when no movement keys are pressed, the player is restored into a rest state
            //also, when a dodge is finished, the player is restored to it's original movement
            if (Input.anyKey == false || dodgeFlag || (!Input.GetKey(forward) && !Input.GetKey(left) && !Input.GetKey(backward) && !Input.GetKey(right)))
            {
                directionx = 0;
                directionz = 0;
                directiony = 0;
                rb.velocity = Vector3.zero;
                dodgeFlag = false;
            }


            //the famous Unreal Tournament single-tap dodge
            if (Input.GetKeyDown(dodge))
            {
                rb.AddRelativeForce(new Vector3(directionx*dodgeSpeed,dodgeHeight, directionz *dodgeSpeed));
                dodgeFlag = true;
            }



            if (Input.GetKeyDown(jump))
            {
                rb.AddForce(Vector3.up * jumpHeight);
            }
        }

    }

    //check for collision
    void OnCollisionStay(Collision col)
    {
        //activate flag only when contact between ground and player is true
        if(col.gameObject.CompareTag("ground"))
        {
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
        }
    }

    //check for exit collision to avoid to being able to move in the air
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            touchingGround = false;
        }
    }


}
