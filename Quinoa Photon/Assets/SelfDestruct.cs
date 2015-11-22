using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

    public float selfDestructTime;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        selfDestructTime -= Time.deltaTime;
        if(selfDestructTime < 0 )
        {
            PhotonView pv = GetComponent<PhotonView>();
            if(pv != null && pv.instantiationId !=0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
	}
}
