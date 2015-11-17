using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class playerMovement : NetworkBehaviour {
    public static Rigidbody rb;
    public Rigidbody playerPrefab;
    //public static Transform tf;

    public float dodgeTimeout; // the time you have to wait before performing a dodge again
    private float previousHeight; // variable to store the height position of the player
    private bool lookupHeight; // flag for looking up the height position of the player
    private bool touchingFix; // flag for accessing control statements without touching ground
    public float thresholdHeight; // amount of difference between cached height an actual height triggers touchingFix
    private float previousTime; //holds the time in seconds, used for tracking time without collision on ramps
    public float maxHP;         //Initial HP
    private float currentHP;    //Current HP

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

    //flags for checking if player has contact with the ground; this to avoid air-jumping
    private bool touchingGround;
    private bool touchingRamp;

    //variables to tune the single-tap dodge move
    public float dodgeSpeed, dodgeHeight;

    //flag for ability to spawn players
    public static bool SpawnFlag;

	// Use this for initialization
	void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        //tf = GetComponent<Transform>();
        sensitivity = 120; // initialize at default sensitivity; to be tweakable live in the future
        dodgeFlag = true;
        SpawnFlag = true;
        currentHP = maxHP;

        Debug.Log(rb.rotation.eulerAngles);
    }
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            return;
        }

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

        //if (Input.GetButtonDown("Fire1"))
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //this fixes unwanted rotation caused by contact with a ramp
        if (mouseMovementX == 0)
        {
            rb.angularVelocity = Vector3.zero;
        }

        //Set the mouse visible and unlocked when the esc key is pressed
        if (Input.GetKey("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        //check for key presses and adapt movement of player but only if the ground is touched
        if (touchingGround || touchingRamp || touchingFix)
        {
            if (touchingFix) Debug.Log("yeah");
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
                directionz = 0;
            }


            //when no movement keys are pressed, the player is restored into a rest state
            //also, when a dodge is finished, the player is restored to it's original movement
            if ((!Input.GetKey(forward) && !Input.GetKey(left) && !Input.GetKey(backward) && !Input.GetKey(right)))
            {
                directionx = 0;
                directionz = 0;
                directiony = 0;
                //rb.velocity = Vector3.zero;
            }

            //save previous height position; this is useful for making the player jump or dodge when slightly above the ground
            //(hopefully) solves the 'ramp bug': unable to jump/dodge on ramps
            previousHeight = transform.position.y;
            lookupHeight = true;

            //the famous Unreal Tournament single-tap dodge
            if (Input.GetKeyDown(dodge) && dodgeFlag)
            {
                rb.AddRelativeForce(new Vector3(directionx*dodgeSpeed,dodgeHeight, directionz *dodgeSpeed));
                StartCoroutine(DodgeTimeout());
            }

            //add upwards force upon pressing the jump key
            if (Input.GetKeyDown(jump))
            {
                rb.AddForce(Vector3.up * jumpHeight);
            }

            if (touchingFix)
            {
                //previousTime = Time.fixedTime;
               // Debug.Log(previousTime);
            }

            if (touchingGround) //|| (Time.fixedTime - previousTime > 0.2 && touchingFix)
            {
                touchingFix = false;
            }

        }
        else
        {
            if(Mathf.Abs(previousHeight - transform.position.y) < thresholdHeight)
            {
                //previousTime = Time.fixedTime;
                touchingFix = true;
                Debug.Log("ramp, this one is for you");
            }
            else
            {
                //touchingFix = false;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {

        //velocity is set to zero upon collision with walls; add the tags of pickups, bullets etc as: !gameObject.CompareTag(tag)
        //this fixes the dodge bug
        if (!col.gameObject.CompareTag("bullet") && !col.gameObject.CompareTag("ramp"))
        {
            rb.velocity = Vector3.zero;
        }
        else if (col.gameObject.CompareTag("bullet"))
        {
            Debug.Log("You got hit, noob");
            currentHP -= 10;
            if (currentHP <= 0)
            {
                Debug.Log("U dead m4te");
                respawn();

            }
        }

    }

    private void respawn()
    {
        Debug.Log("Respawning...");
        transform.position = new Vector3(0, 0, 0);
    }


    //check for collision
    void OnCollisionStay(Collision col)
    {
        //activate flag only when contact between ground and player is true
        if(col.gameObject.CompareTag("ground"))
        {
            touchingGround = true;
        }

        if(col.gameObject.CompareTag("ramp"))
        {
            touchingRamp = true;
        }
        else
        {
            //touchingGround = false;
        }
    }

    //check for exit collision to avoid to being able to move in the air
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            touchingGround = false;
        }

        if(col.gameObject.CompareTag("ramp"))
        {
            touchingRamp = false;
        }
    }

    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator DodgeTimeout()
    {
        dodgeFlag = false;
        yield return new WaitForSeconds(dodgeTimeout);
        dodgeFlag = true;
    }
}