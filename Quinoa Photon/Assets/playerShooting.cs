using UnityEngine;
using System.Collections;

public class playerShooting : MonoBehaviour {

    public float fireRate;
    private bool fireFlag;
    public float damage;

	// Use this for initialization
	void Start () {
        fireFlag = true;
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


        if(hitTransform != null)
        {
            Debug.Log(hitTransform.name);
            Health h = hitTransform.GetComponent<Health>();

            while(h == null && hitTransform.parent)
            {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<Health>();
            }

            if(h != null)
            {
                h.TakeDamage(damage);
            }
        }



        ////////////////////////////////
        Debug.Log("shoot");
        yield return new WaitForSeconds(fireRate);
        fireFlag = true;
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

        return closestHit;
    }

}
