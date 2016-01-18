using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class Health : MonoBehaviour {

    public float hitPoints;
    public float currentHitPoints;
    private Calculator calculator;
#pragma warning disable 0414 // Type or member is obsolete
    private RandomMatchmaker rm;
#pragma warning disable 0414 // Type or member is obsolete
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

        if(currentHitPoints <= 0) //only allow negative health for players
        {
            if (RandomMatchmaker.offlineMode)
            {
                Debug.Log("calculator");
                for (int i = 0; i != calculator.getAllTargets(); i++)
                {
                    if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                    {
                        calculator.deleteTarget(i, this.gameObject);
                    }
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
        //RandomMatchmaker rmm = GameObject.FindObjectOfType<RandomMatchmaker>();

        if(GetComponent<SphereCollider>().enabled) //this is a bot
        {
            rm.playerKind = false;
        }
        else if(!GetComponent<SphereCollider>().enabled) //this is a player
        {
            rm.playerKind = true;
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
                //RandomMatchmaker nm = GameObject.FindObjectOfType<RandomMatchmaker>();
                Debug.Log("photonview ismine");
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
                        Debug.Log(rm.standby);
                        rm.standby.SetActive(true);
                    }
                    rm.respawnTimer = 2; //set the respawn time to 2 sec

                    if (RandomMatchmaker.offlineMode)
                    {
                        for (int i = 0; i != calculator.getAllTargets(); i++)
                        {
                            if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                            {
                                calculator.deleteTarget(i, this.gameObject);
                            }
                        }
                    }
                
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Debug.Log("don't know, is this condition even reachable?");
            }
        }
        
        //else if (PhotonNetwork.isMasterClient)
        //{
        //    PhotonNetwork.Destroy(gameObject);
        //}
    }
}
