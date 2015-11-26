using UnityEngine;
using System.Collections;


public class RandomMatchmaker : MonoBehaviour {

    //public GameObject player;
    //public GameObject camera1;
    public GameObject standby;
    SpawnSpot[] spawnSpots;
    string type = "random";
    string name = "";

    public bool offlineMode;
	// Use this for initialization
	void Start () {

        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();

        //PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    public void ConnectRandom()
    {
        type = "random";
        if (offlineMode)
        {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }
    }

    public void JoinRoom(string name)
    {
        type = "room";
        this.name = name;
        if (offlineMode)
        {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }
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
        if (type == "random")
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else if (type == "room")
        {
            PhotonNetwork.JoinOrCreateRoom(this.name, new RoomOptions(), TypedLobby.Default);
        }
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


        GameObject player = PhotonNetwork.Instantiate("player4", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0); //local player spawned
        player.GetComponent<playerMovement>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<playerShooting>().enabled = true;
        player.transform.FindChild("Main Camera").gameObject.SetActive(true);
        //GameObject camera1 = PhotonNetwork.Instantiate("MainCamera", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standby.SetActive(false);

    }
}
