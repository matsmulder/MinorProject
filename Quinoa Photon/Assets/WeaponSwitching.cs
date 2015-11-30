using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeaponSwitching : MonoBehaviour {

    /// <summary>
    /// a standarized list for weaponIDs and order of childs
    /// It is important to adhere to this list to prevent weird bugs and glitches like guns shooting each others bullets
    /// 0: Sniper rifle
    /// 1: Laser gun
    /// 2: plasma gun
    /// 3: rocket launcher
    /// 4: grenade launcher
    /// ... to be continued
    ///      
    /// </summary>

    public string sniperRifle, laserGun, plasmaGun, rocketLauncher, grenadeLauncher;

    GameObject[] weaponList;

    void Start () {

        //make a list of all childs of weaponHolder: the guns!
        weaponList = new GameObject[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            weaponList[i] = child.gameObject;
            i++;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //when a key for switching weapons is pressed,
        //deactivate all weapons and only activate the specified 
        if(Input.GetKeyDown(sniperRifle) || Input.GetKeyDown(laserGun) || Input.GetKeyDown(plasmaGun) || Input.GetKeyDown(rocketLauncher) || Input.GetKeyDown(grenadeLauncher))
        {
           for(int i=0; i < weaponList.Length; i++)
            {
                weaponList[i].SetActive(false);
            }
        }

        

        if (Input.GetKeyDown(sniperRifle))
        {
            weaponList[0].SetActive(true);
        }

        if (Input.GetKeyDown(laserGun))
        {
            weaponList[1].SetActive(true);
        }

        if (Input.GetKeyDown(plasmaGun))
        {
            weaponList[2].SetActive(true);
        }

        if (Input.GetKeyDown(rocketLauncher))
        {

        }

        if (Input.GetKeyDown(grenadeLauncher))
        {

        }
        //if (Input.GetKeyDown(sniperRifle) { to be continued...

        //}
    }
}
