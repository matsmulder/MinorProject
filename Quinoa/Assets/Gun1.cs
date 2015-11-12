using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Gun1 : NetworkBehaviour {
    private Rigidbody rb; //the 3D model of the gun as rigidbody
    public Rigidbody prefabBullet; //the 3D model of the bullet as rigidbody
    public float bulletSpeed; //the travelling speed of the bullet
    private Vector3 bulletDir;

    private bool shootFlag; //
    public float fireRate; //the fire rate of the gun

    public string fire; //rebindable control string to fire

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        shootFlag = true;
	}
	
	// Update is called once per frame
	void Update () {



        if (Input.GetButton(fire)) //PLACEHOLDER, to be changed to left mouse button; left mouse button is still buggy
        {
            if (shootFlag)
            {
                shootFlag = false;
                StartCoroutine(bullet());
            }
        }
	}

    IEnumerator bullet()
    {
        //instantiate bullet
        Rigidbody clone = Instantiate(prefabBullet, new Vector3(rb.position.x, rb.position.y, rb.position.z + 2), cameraMovement.tf.rotation) as Rigidbody;

        //make a ray from the viewpoint to the middle of the screen; this will determine the direction of the bullets
        Ray shootDir = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f, 0));
        bulletDir = shootDir.direction;
        clone.velocity = shootDir.direction * bulletSpeed;
        yield return new WaitForSeconds(fireRate);
        shootFlag = true;
    }
}
