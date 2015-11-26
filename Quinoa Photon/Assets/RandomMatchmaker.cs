using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RandomMatchmaker : MonoBehaviour {

    //public GameObject player;
    //public GameObject camera1;
    public GameObject standby;
    public GameObject roomButtonPrefab;
    SpawnSpot[] spawnSpots;
    string type = "random";
    //string roomName = "";
    public Text nameBox;

    public float respawnTimer;

    public bool offlineMode;
    string status;


	// Use this for initialization
	void Start () {
        status = "inMenu";
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        if (offlineMode)
        {
            OnJoinedLobby();            //For future extensions
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
        }
    }

    public void ConnectRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(nameBox.text, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }
	
	// Update is called once per frame
	void Update () {
        if(respawnTimer > 0 )
        {
            respawnTimer -= Time.deltaTime;

            if(respawnTimer <= 0)
            {
                //respawn the player
                SpawnPlayer();
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public void OnJoinedLobby()
    {   
        
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        status = "inGame";
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

    public void BrowseGames()
    {
        GameObject but = (GameObject)Instantiate(roomButtonPrefab, new Vector3(0, 0, 0), new Quaternion());
        

        Debug.Log("Browsing games...");
        foreach (RoomInfo game in PhotonNetwork.GetRoomList())
        {
            //Debug.Log(game.name);
            Debug.Log("woohoo");
        }
    }

    void OnReceivedRoomListUpdate()
    {
        if (status=="browsing")
        {
            BrowseGames();
        }
    }

    public void clearRoomButtons()
    {

    }
    
    public void setStatus(string stat)
    {
        status = stat;
    }

}
