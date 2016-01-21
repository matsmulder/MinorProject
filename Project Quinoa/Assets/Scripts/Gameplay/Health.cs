using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Health : MonoBehaviour {

	Sprite image1;
	Sprite image2;
	Sprite image3;
	Sprite image4;
	Sprite image5;
	Sprite image6;
	Sprite image7;
	public GameObject health;

    public float hitPoints;
    public float currentHitPoints;
    private Calculator calculator;
#pragma warning disable 0414 // Type or member is obsolete
    private RandomMatchmaker rm;
#pragma warning disable 0414 // Type or member is obsolete
    // Use this for initialization
    void Start () {
        PhotonNetwork.player.customProperties["Deaths"] = 0;
        PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.customProperties);
        rm = GameObject.FindGameObjectWithTag("scripts").GetComponent<RandomMatchmaker>();
        currentHitPoints = hitPoints;
        calculator = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
		image1 = Resources.Load<Sprite> ("HealthBar1");
		image2 = Resources.Load<Sprite> ("HealthBar2");
		image3 = Resources.Load<Sprite> ("HealthBar3");
		image4 = Resources.Load<Sprite> ("HealthBar4");
		image5 = Resources.Load<Sprite> ("HealthBar5");
		image6 = Resources.Load<Sprite> ("HealthBar6");
		image7 = Resources.Load<Sprite> ("HealthBar7");
    }
	
	// Update is called once per frame
	void Update () {
		if (currentHitPoints == 201f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image1;
			
		};
		if (currentHitPoints < 172f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image2;
			
		};
		if (currentHitPoints < 144f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image3;
			
		};
		if (currentHitPoints < 115f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image4;
			
		};
		if (currentHitPoints < 85f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image5;
			
		};
		if (currentHitPoints < 56f) {
			Debug.Log ("minder dan 120");
			health.GetComponent<Image>().sprite = image6;
			
		};
		if (currentHitPoints < 28f) {
			Debug.Log("minder dan 50");
			health.GetComponent<Image>().sprite = image7;
		};
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
                //apparently here Hugo got a nullreferenceexception after a double kill...
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
                        //rm.standby.SetActive(true);
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
