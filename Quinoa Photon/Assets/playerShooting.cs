using UnityEngine;
using System.Collections;

public class playerShooting : MonoBehaviour {

    public float fireRate;
    private bool fireFlag;
    public float damage;
    //public LineRenderer laserBeam;
    public float shootingRange;

    // Use this for initialization

    FXmanager fxManager;


	void Start () {
        fireFlag = true;
        fxManager = GameObject.FindObjectOfType<FXmanager>();

        if(fxManager == null)
        {
            Debug.Log("no fxmanager found");
        }
        //laserBeam = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	      if(Input.GetButton("Fire1") && fireFlag) //player shooting
        {
            Fire();
        }
	}

    void Fire()
    {
        StartCoroutine(Shoot());
    }


    IEnumerator Shoot()
    {
        fireFlag = false;
        //////////////////////////////

        Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
        Transform hitTransform;
        Vector3 hitPoint;

        hitTransform = FindClosestHitInfo(ray, out hitPoint);

       
        //laserBeam.SetPosition(0, transform.position); // position of the player

        if(hitTransform != null)
        {
            //laserBeam.SetPosition(1, hitTransform.position); //set the target point of the laser beam to the transform of the nearest hit point
            //Debug.Log(hitTransform.name);
            Health h = hitTransform.GetComponent<Health>();

            while(h == null && hitTransform.parent) //cycle through childs until the parent which holds the Health script is found
            {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            if(h != null)
            {
                h.GetComponent<PhotonView>().RPC("TakeDamage",PhotonTargets.All, damage);
                //this line is the equivalent of h.TakeDamage(damage) but synchronized
            }

            if(fxManager != null)
            {
                fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, hitPoint);
            }
        }
        else
        {
            //nothing is hit
            if (fxManager != null)
            {
                fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, Camera.main.transform.forward * shootingRange);
            }
        }


        //laserBeam.enabled = true; //enable visibility of laser beam
        ////////////////////////////////
        yield return new WaitForSeconds(fireRate);
        fireFlag = true;
        ///////////////////////////////
        //laserBeam.enabled = false; //disable visibility of laser beam
        //laserBeam.SetPosition(1, transform.position); //reset beam by taking the player position as second position
    }

    Transform FindClosestHitInfo(Ray ray, out Vector3 hitPoint)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray);

        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach(RaycastHit hit in hits)
        {
            if(hit.transform != this.transform && (closestHit == null || hit.distance < distance))
            {
                //something is hit that is the first thing and not us
                
                closestHit = hit.transform;

                distance = hit.distance;
                hitPoint = hit.point;

            }
        }
        //set end of laser beam to the point of impact
        if(closestHit == null)
        {
            Debug.Log("nullshoot");
            hitPoint = Camera.main.transform.forward * shootingRange;
        }
        //laserBeam.SetPosition(1, hitPoint);

        return closestHit;
    }

}
