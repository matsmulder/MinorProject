using UnityEngine;
using System.Collections;

public class playerShooting : MonoBehaviour {

    public string fire;
    private bool fireFlag;
    public float shootingRange;
    WeaponData weaponData;

    // Use this for initialization

    FXmanager fxManager;


    void Start() {

        fireFlag = true;
        fxManager = FindObjectOfType<FXmanager>();

        if(fxManager == null)
        {
            Debug.Log("no fxmanager found");
        }
	}
	
	// Update is called once per frame
	void Update () {
	      if(Input.GetButton(fire) && fireFlag) //player shooting
        {
            Fire();
        }
    }

    void Fire()
    {
       
        //if (weaponData == null)
        {
            weaponData = gameObject.GetComponentInChildren<WeaponData>();
            Debug.Log(weaponData.fireRate + "firerate");
        }

        StartCoroutine(Shoot());
    }


    IEnumerator Shoot()
    {
        //DON'T EDIT
        fireFlag = false;
        //////////////////////////////
        //edit from here



        //hit detection with hitscan, used for sniper rifle and laser gun
        if (weaponData.weaponID == 0 || weaponData.weaponID == 1 || weaponData.weaponID == 2) // if the weapon used is a sniper rifle or laser gun
        {

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Transform hitTransform; //transform of the hit object (hitscan only for now)
            Vector3 hitPoint; //the exact coordinates of the hit face; used as endpoint of 'laser beam' ray and for ricochets in the future

            hitTransform = FindClosestHitInfo(ray, out hitPoint);// find out which object was hit with raycasting; used for weapons with hitscan (instant fire weapons)


            //------------- apply health changes-------------------------------------------------------------
            if (hitTransform != null)
            {
                Health h = hitTransform.GetComponent<Health>();

                while (h == null && hitTransform.parent) //cycle through childs until the parent which holds the Health script is found
                {
                    hitTransform = hitTransform.parent;
                    h = hitTransform.GetComponent<Health>();
                }

                if (h != null) //only execute when the hit object has a health script
                {
                    TeamMember tm = hitTransform.GetComponent<TeamMember>(); //lookup the team information of the hit object
                    TeamMember myTm = this.GetComponent<TeamMember>();

                    if (tm == null || tm.teamID == 0 || myTm == null || myTm.teamID == 0 || tm.teamID != myTm.teamID)
                    {
                        //execute if: hitTransform or the player itself has no team info, is of teamID 0 (no team, independent faction, deathmatch mode) or the teamIDs are different
                        //this line is the equivalent of h.TakeDamage(damage) but synchronized
                        h.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, weaponData.damage);
                    }

                }
                //-----------------------------------------------------------------------------------------------


                if (fxManager != null) //'sanity check': is there an fxManager? (yes of course, execute Gun FX function)
                {
                    DoGunFX(hitPoint);
                }
            }
            else
            {
                //nothing is hit
                if (fxManager != null)
                {
                    //fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, Camera.main.transform.position, Camera.main.transform.forward * shootingRange);
                    DoGunFX(Camera.main.transform.forward * shootingRange);
                }
            }
        }


        ////////////////////////////////
        yield return new WaitForSeconds(weaponData.fireRate);
        fireFlag = true;
        //DON'T EDIT
    }

    void DoGunFX(Vector3 hitPoint) {

        //sniper rifle: do an RPC call to SniperBulletFX which will create a bullet trail ray, sound and particle effects
        if (weaponData.weaponID == 0 || weaponData.weaponID == 1 || weaponData.weaponID == 2) // only create a ray for hitscan-based weapons
        {
            Debug.Log(GetComponent<PhotonView>());
            fxManager.GetComponent<PhotonView>().RPC("SniperBulletFX", PhotonTargets.All, weaponData.transform.position, hitPoint);
        }

        if(weaponData.weaponID == 3) //plasma gun
        {

        }

        if(weaponData.weaponID == 4) //rocket launcher
        {

        }
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
            hitPoint = Camera.main.transform.forward * shootingRange;
        }
        //laserBeam.SetPosition(1, hitPoint);

        return closestHit;
    }

}
