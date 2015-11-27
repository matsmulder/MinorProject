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
    private bool pickedTeam = false;


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
                SpawnPlayer(1);
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        if(PhotonNetwork.connected == false)
        {
            //not yet connected, ask for online/offline <UPCOMING>
        }

        if(PhotonNetwork.connected == true)
        {
            //fully connected
            if(pickedTeam)
            {

            }
            else
            {
                //player has no team assigned
                if(GUILayout.Button("Team Fastfood"))
                {
                    SpawnPlayer(1);
                }
                if(GUILayout.Button("Team Superfood"))
                {
                    SpawnPlayer(2);
                }
                if(GUILayout.Button("Random Select"))
                {
                    SpawnPlayer(Random.Range(1, 3));
                }
                if(GUILayout.Button("No Team"))
                {
                    SpawnPlayer(0);
                }

            }

        }
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
        //SpawnPlayer();

    }

    void SpawnPlayer(int teamID)
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

        //set teamID, TODO: set colour
        player.GetComponent<TeamMember>().teamID = teamID;
        MeshRenderer skinColour = player.transform.GetComponentInChildren<MeshRenderer>();
        if(skinColour == null)
        {
            Debug.Log("no mesh renderer found");
        }

        if(teamID==1) // team fastfood
        {
            skinColour.material.color = Color.red;
        }
        if(teamID==2) //team superfood
        {
            skinColour.material.color = Color.green;
        }
        if(teamID==0) //no team
        {
            skinColour.material.color = Color.clear;
        }
        //GameObject camera1 = PhotonNetwork.Instantiate("MainCamera", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standby.SetActive(false);

    }

    public void BrowseGames()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        GameObject but;

        int i = 0;
        foreach (RoomInfo game in rooms)
        {
            but = (GameObject)Instantiate(roomButtonPrefab, new Vector3(0, 0, 0), new Quaternion());
            but.transform.SetParent(GameObject.Find("Canvas").transform);

            RectTransform rt = but.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -10-30*i);
            but.GetComponent<Text>().text = game.name;

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
