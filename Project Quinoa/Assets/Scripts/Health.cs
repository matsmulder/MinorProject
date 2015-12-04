using UnityEngine;
using System.Collections;

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

        if(currentHitPoints <= 0)
        {
            Die();
            Debug.Log("DIE");
        }
    }


    void Die()
    {
        if (GetComponent<PhotonView>().instantiationId == 0) //
        {
            Destroy(gameObject);
        }
        else
        {
            if(GetComponent<PhotonView>().isMine) // this is MY player object
            {
                if (gameObject.tag == "Player") 
                {
                    RandomMatchmaker nm = GameObject.FindObjectOfType<RandomMatchmaker>();
                    nm.standby.SetActive(true);
                   nm.respawnTimer = 2; //set the respawn time to 2 sec
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
