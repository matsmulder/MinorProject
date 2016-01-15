using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class Health : MonoBehaviour {

    public float hitPoints;
    private float currentHitPoints;
    private Calculator calculator;
    private RandomMatchmaker rm;
	// Use this for initialization
	void Start () {
        rm = GameObject.FindGameObjectWithTag("scripts").GetComponent<RandomMatchmaker>();
        currentHitPoints = hitPoints;
		PhotonNetwork.player.customProperties["Deaths"] = 0;
		PhotonNetwork.player.SetCustomProperties (PhotonNetwork.player.customProperties);
        calculator = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
	
	public float getHealthPoints(){
		return currentHitPoints;
	}

    [PunRPC]
    public void TakeDamage(float amount)
    {
        currentHitPoints -= amount;
        Debug.Log(currentHitPoints);

        if(currentHitPoints <= 0 && gameObject.tag != "fastfood" && gameObject.tag != "superfood") //only allow negative health for players
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    calculator.deleteTarget(i, this.gameObject);
                }
            }
            Die();
        }

        if(currentHitPoints == 0 && (gameObject.tag == "fastfood" || gameObject.tag == "superfood")) //only call Die() when hitpoints of pickup are equal to zero
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    calculator.deleteTarget(i, this.gameObject);
                }
            }
            Die();
        }
    }


    void Die()
    {
        Debug.Log("DIE");
        //Analytics.CustomEvent(string customEventName, IDictionary < string, object > eventData);
       Analytics.CustomEvent("DIE", new Dictionary<string, object>
       {
           {"died object", gameObject.tag },
           {"position", gameObject.transform.position }
       });
        RandomMatchmaker rmm = GameObject.FindObjectOfType<RandomMatchmaker>();

        if(GetComponent<SphereCollider>().enabled) //this is a bot
        {
            rmm.playerKind = false;
        }
        else if(!GetComponent<SphereCollider>().enabled) //this is a player
        {
            rmm.playerKind = true;
        }
        else
        {
            Debug.LogWarning("sphere collider does not exist");
        }
        
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
				
					PhotonNetwork.player.customProperties["Deaths"] = (int)PhotonNetwork.player.customProperties["Deaths"] + 1;
					PhotonNetwork.player.SetCustomProperties (PhotonNetwork.player.customProperties);

                    int team = gameObject.GetComponent<TeamMember>().teamID;
                    if (team == 1)
                    {
                        PhotonNetwork.room.customProperties["FFDeaths"] = (int)PhotonNetwork.room.customProperties["FFDeaths"] + 1;
                    }
                    else if (team == 2)
                    {
                        PhotonNetwork.room.customProperties["SFDeaths"] = (int)PhotonNetwork.room.customProperties["SFDeaths"] + 1;
                    }
                    else
                    {
                        Debug.LogWarning("Something somewhere went terribly wrong");
                    }
                    PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);
                    //if (!RandomMatchmaker.offlineMode)
                    if(GetComponent<SphereCollider>().enabled == false) //only activate standby camera for real player upon dying
                    {
                        nm.standby.SetActive(true);
                    }
                    nm.respawnTimer = 2; //set the respawn time to 2 sec

                    for (int i = 0; i != calculator.getAllTargets(); i++)
                    {
                        if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                        {
                            calculator.deleteTarget(i, this.gameObject);
                        }
                    }
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
