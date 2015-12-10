using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    private TeamMember tm;
    private Health h;
    public float healthPickup;
    private PhotonView pv;

	// Use this for initialization
	void Start () {
        tm = GetComponent<TeamMember>();
        pv = GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerStay(Collider col)
    {
        Debug.Log("test col");
        //pickup is fastfood and has to be picked up by team Wholo
        if(col.gameObject.CompareTag("fastfood") && tm.teamID == 2) 
        {
            Debug.Log("Go Wholo");
            h = col.GetComponent<Health>();
            h.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, healthPickup);
        }

        //pickup is superfood and has to be picked up by team Trump
        if (col.gameObject.CompareTag("superfood") && tm.teamID == 1)
        {
            Debug.Log("Go Trump");
            h = col.GetComponent<Health>();
            h.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, healthPickup);
        }



    }
}
