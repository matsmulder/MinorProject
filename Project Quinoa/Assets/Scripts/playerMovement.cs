using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class playerMovement : MonoBehaviour {
    public static Rigidbody rb;
    //public static Transform tf;

    public float dodgeTimeout; // the time you have to wait before performing a dodge again
    private float previousHeight; // variable to store the height position of the player
    //private bool lookupHeight; // flag for looking up the height position of the player
    //private bool touchingFix; // flag for accessing control statements without touching ground
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
    //public string left, right, backward, forward, jump, dodge;
    private Dictionary<string, string> keys;
    //Default keys
    private Dictionary<string, string> defaultKeys;

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

        //Loading keyconfig
        defaultKeys = new Dictionary<string, string>
        {
            { "left","a" },
            { "right","d" },
            {"forward","w" },
            { "backward","s" },
            {"jump","space" },
            {"dodge", "left shift" },

        };
        keys = new Dictionary<string, string>();
        try
        {
            StreamReader f = new StreamReader("keys.cfg");
            string line;
            while ((line = f.ReadLine()) != null)
            {
                string[] splitted = line.Split(':');
                if (!defaultKeys.ContainsKey(splitted[0]))
                {
                    Debug.Log("Key not used: " + splitted[0]);
                }
                else
                {
                    keys[splitted[0]] = splitted[1];
                }

            }
            f.Close();
            using (StreamWriter sw = File.AppendText("keys.cfg"))
            {
                foreach (string key in defaultKeys.Keys)
                {
                    if (!keys.ContainsKey(key))
                    {
                        //Add key to file if it doesn't exist
                        sw.Write(key + ":" + defaultKeys[key] + "\n");
                        //Add key to settings
                        keys[key] = defaultKeys[key];
                    }
                }
            }
        }
        catch (IOException)
        {
            //File doesn't exist yet, create it
            using (StreamWriter sw = File.CreateText("keys.cfg"))
            {
                foreach (string key in defaultKeys.Keys)
                {
                    sw.Write(key + ":" + defaultKeys[key] + "\n");
                    keys[key] = defaultKeys[key];
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {


        //MOVE!
        Vector3 direction = new Vector3(directionx*walkingSpeed*Time.deltaTime, directiony*walkingSpeed*Time.deltaTime, directionz*walkingSpeed*Time.deltaTime);
        transform.Translate(direction);

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
            Debug.Log("Pressed escape");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        
        //check for key presses and adapt movement of player but only if the ground is touched
        if (touchingGround || touchingRamp)
            //|| touchingFix)
        {
            //move forward
            if (Input.GetKey(keys["forward"]))
            {
                directionz = 1;
            }


            //move left
            if (Input.GetKey(keys["left"]))
            {
                directionx = -1;
            }


            //move backward
            if (Input.GetKey(keys["backward"]))
            {
                directionz = -1;
            }

            //move right
            if (Input.GetKey(keys["right"]))
            {
                directionx = 1;
            }

            //stop moving in certain direction when a key is no longer pressed
            if (Input.GetKeyUp(keys["forward"]) || Input.GetKeyUp(keys["left"]) || Input.GetKeyUp(keys["backward"]) || Input.GetKeyUp(keys["right"]))
            {
                directionx = 0;
                directionz = 0;
            }


            //when no movement keys are pressed, the player is restored into a rest state
            //also, when a dodge is finished, the player is restored to it's original movement
            if ((!Input.GetKey(keys["forward"]) && !Input.GetKey(keys["left"]) && !Input.GetKey(keys["backward"]) && !Input.GetKey(keys["right"])))
            {
                directionx = 0;
                directionz = 0;
                directiony = 0;
                //rb.velocity = Vector3.zero;
            }

            //save previous height position; this is useful for making the player jump or dodge when slightly above the ground
            //(hopefully) solves the 'ramp bug': unable to jump/dodge on ramps
            previousHeight = transform.position.y;

            //lookupHeight = true;
            //the famous Unreal Tournament single-tap dodge
            if (Input.GetKeyDown(keys["dodge"]) && dodgeFlag)
            {
                rb.AddRelativeForce(new Vector3(directionx*dodgeSpeed,dodgeHeight, directionz *dodgeSpeed));
                StartCoroutine(DodgeTimeout());
            }

            //add upwards force upon pressing the jump key
            if (Input.GetKeyDown(keys["jump"]))
            {
                rb.AddForce(Vector3.up * jumpHeight);
            }

            //if (touchingFix)
            //{
            //    //previousTime = Time.fixedTime;
            //   // Debug.Log(previousTime);
            //}

            //if (touchingGround) //|| (Time.fixedTime - previousTime > 0.2 && touchingFix)
            //{
            //    touchingFix = false;
            //}

        }
        else
        {
            //if(Mathf.Abs(previousHeight - transform.position.y) < thresholdHeight)
            //{
            //    //previousTime = Time.fixedTime;
            //    touchingFix = true;
            //}
            //else
            //{
            //    //touchingFix = false;
            //}
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
        //if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
        {
            //Debug.Log("touching the ground again");
            touchingGround = true;
        }
        


        if (col.gameObject.CompareTag("ramp"))
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
        //if (!Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
        {
            //Debug.Log("not touching ground anymore");
            touchingGround = false;
        }

        if (col.gameObject.CompareTag("ramp"))
        {
            touchingRamp = false;
        }
    }

    void OnDestroy()
    {
        if (PhotonView.Get(this).isMine)
        {
            Debug.Log("Destroyed");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    IEnumerator DodgeTimeout()
    {
        dodgeFlag = false;
        yield return new WaitForSeconds(dodgeTimeout);
        dodgeFlag = true;
    }

   // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
   // {
   //     if (stream.isWriting)
   //     {
   //         // We own this player: send the others our data
   //         stream.SendNext(transform.position);
   //         stream.SendNext(transform.rotation);
   //     }
   //     else
   //     {
   //         // Network player, receive data
   //         this.transform.position = (Vector3)stream.ReceiveNext();
   //         this.transform.rotation = (Quaternion)stream.ReceiveNext();
   //     }
   // }
}