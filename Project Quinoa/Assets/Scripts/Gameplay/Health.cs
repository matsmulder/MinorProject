﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class Health : MonoBehaviour {

    public float hitPoints;
    private float currentHitPoints;

	// Use this for initialization
	void Start () {
        currentHitPoints = hitPoints;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    [PunRPC]
    public void TakeDamage(float amount)
    {
        currentHitPoints -= amount;
        Debug.Log(currentHitPoints);

        if(currentHitPoints <= 0 && gameObject.tag != "fastfood" && gameObject.tag != "superfood") //only allow negative health for players
        {
            Die();
        }

        if(currentHitPoints == 0 && (gameObject.tag == "fastfood" || gameObject.tag == "superfood")) //only call Die() when hitpoints of pickup are equal to zero
        {
            Die();
        }
    }


    void Die()
    {
        //Analytics.CustomEvent(string customEventName, IDictionary < string, object > eventData);
       Analytics.CustomEvent("DIE", new Dictionary<string, object>
       {
           {"died object", gameObject.tag },
           {"position", gameObject.transform.position }
       });
        if (GetComponent<PhotonView>().instantiationId == 0) //
        {
            Destroy(gameObject);
        }
        else
        {
            if (GetComponent<PhotonView>().isMine) // this is MY player object
            {
                if (gameObject.tag == "Player")
                {
                    RandomMatchmaker nm = GameObject.FindObjectOfType<RandomMatchmaker>();

                    nm.standby.SetActive(true);
                    nm.respawnTimer = 2; //set the respawn time to 2 sec
                }

                if (gameObject.tag == "fastfood")
                {
                    scoreManager.numberOfFastPickups--;
                }
                if (gameObject.tag == "superfood")
                {
                    scoreManager.numberOfSuperPickups--;
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
        //else if (PhotonNetwork.isMasterClient)
        //{
        //    PhotonNetwork.Destroy(gameObject);
        //}
    }
}
