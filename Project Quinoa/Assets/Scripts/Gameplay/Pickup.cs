using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public TeamMember tm;
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

    //void OnCollisionStay(Collision col)
    //{
    //    //pickup is fastfood and has to be picked up by team Wholo
    //    if(col.gameObject.CompareTag("fastfood") && tm.teamID == 2) 
    //    {
    //        h = col.gameObject.GetComponent<Health>();
    //        h.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, healthPickup);
    //        //scoreManager.scoreSuper++;
    //    }

    //    //pickup is superfood and has to be picked up by team Trump
    //    if (col.gameObject.CompareTag("superfood") && tm.teamID == 1)
    //    {
    //        h = col.gameObject.GetComponent<Health>();
    //        h.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, healthPickup);
    //        //scoreManager.scoreFast++;
    //    }



    //}

    public void EndGame()
    {

    }
}
