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

        if(currentHitPoints <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        //Destroy(gameObject);
        if (GetComponent<PhotonView>().instantiationId == 0) //
        {
            Destroy(gameObject);
        }
        else if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
