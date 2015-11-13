using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class playerGenerator : MonoBehaviour {

    public Rigidbody playerPrefab;
	// Use this for initialization
	void Start () {
        Network.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, 0);
        Network.Instantiate(playerPrefab, new Vector3(0,0,10), Quaternion.identity, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
