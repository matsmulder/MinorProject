using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WeaponSwitching : Photon.MonoBehaviour {

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

    bool[] weaponStates;

    GameObject[] weaponList;

    void Start () {

        weaponStates = new bool[transform.childCount];

        //make a list of all childs of weaponHolder: the guns!
        weaponList = new GameObject[transform.childCount];


        int i = 0;
        foreach (Transform child in transform)
        {
            weaponList[i] = child.gameObject;
            //weaponList[i].SetActive(true);
            i++;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //when a key for switching weapons is pressed,
        //deactivate all weapons and only activate the specified 
        
        if (Input.GetKeyDown(sniperRifle))
        {
            Chooser("sniperRifle");
        }

        else if (Input.GetKeyDown(laserGun))
        {
            Chooser("laserGun");
        }

        else if (Input.GetKeyDown(plasmaGun))
        {
            //Chooser("plasmaGun");
        }

        else if (Input.GetKeyDown(rocketLauncher))
        {
            //Chooser("rocketLauncher");
        }

        else if (Input.GetKeyDown(grenadeLauncher))
        {
            //Chooser("grenadeLauncher");
        }

        //else if (Input.GetKeyDown(sniperRifle) { to be continued...

        //}

        //this may cause stuttering
        for (int i=0; i<weaponList.Length; i++)
        {
            //weaponList[i].SetActive(weaponStates[i]);
        }
    }

    [PunRPC]
    void SwitchWeapon(int index)
    {
        if(GetComponent<PhotonView>().isMine)
        Debug.Log(index);
        weaponList[0].SetActive(true);


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            for (int i = 0; i < weaponList.Length; i++)
            {
                stream.SendNext(weaponList[i].activeSelf); //send active state of each weapon
            }
        }
        else
        {
            // Network player, receive data
            for (int i = 0; i < weaponList.Length; i++)
            {
                weaponStates[i] = (bool)stream.ReceiveNext();
                weaponList[i].SetActive(weaponStates[i]);
            }
        }
    }

    public void Chooser(string gun)
    {
        int index = 0;
        if(gun=="sniperRifle")
        {
            index = 0;
        }
        else if(gun=="laserGun")
        {
            index = 1;
        }
        else if(gun=="plasmaGun")
        {
            index = 2;
        }
        else if(gun=="rocketLauncher")
        {
            //index = 3;
        }
        else if(gun=="grenadeLauncher")
        {
            //index = 4;
        }
        for (int i = 0; i < weaponList.Length; i++)
        {
            weaponList[i].SetActive(false);
        }
        weaponList[index].SetActive(true);
    } 
}
