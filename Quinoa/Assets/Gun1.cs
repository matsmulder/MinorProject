using UnityEngine;
using System.Collections;

public class Gun1 : MonoBehaviour {
    private Rigidbody rb; //the 3D model of the gun as rigidbody
    public Rigidbody prefabBullet; //the 3D model of the bullet as rigidbody
    public float bulletSpeed; //the travelling speed of the bullet
    private Vector3 bulletDir;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) //PLACEHOLDER, to be changed to left mouse button; left mouse button is still buggy
        {
            //StartCoroutine(bullet());
            //instantiate bullet
            Rigidbody clone = Instantiate(prefabBullet, new Vector3(rb.position.x,rb.position.y,rb.position.z + 2), cameraMovement.tf.rotation) as Rigidbody;

            Ray shootDir = Camera.main.ScreenPointToRay(Input.mousePosition); //a ray in the looking direction
            bulletDir = shootDir.direction;

            clone.velocity = shootDir.direction;


        }
	}

    IEnumerator bullet()
    {
        yield return new WaitForSeconds(1);
    }
}
