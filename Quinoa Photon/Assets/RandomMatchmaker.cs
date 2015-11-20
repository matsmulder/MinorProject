using UnityEngine;
using System.Collections;

public class RandomMatchmaker : MonoBehaviour {

    public GameObject player;
    //public GameObject camera1;
    public Camera standby;
    SpawnSpot[] spawnSpots;

	// Use this for initialization
	void Start () {

        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("0.1");

        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public void OnJoinedLobby()
    {
            PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        SpawnPlayer();

    }

    void SpawnPlayer()
    {

        if(spawnSpots == null)
        {
            Debug.Log("no spawnspots found");
            return;
        }
        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];


        GameObject player = PhotonNetwork.Instantiate("fpc", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        //GameObject camera1 = PhotonNetwork.Instantiate("MainCamera", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standby.enabled = false;

    }
}
