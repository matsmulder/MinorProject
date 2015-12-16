using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RandomMatchmaker : MonoBehaviour {

    //public GameObject player;
    //public GameObject camera1;
    public GameObject standby;
    public GameObject roomButtonPrefab;
    SpawnSpot[] spawnSpots;
    SpawnSpot[] spawnSpotsNoTeam; //free for all spawnspots
    SpawnSpot[] spawnSpotsSuper; //hipster spawnspots
    SpawnSpot[] spawnSpotsFast; //fastfood spawnspots

    private int indNoTeam = 0, indFast = 0, indSuper = 0;
    string type = "random";
    //string roomName = "";
    public Text nameBox;

    public float respawnTimer;

    public bool offlineMode;
    status stat;
    private bool pickedTeam = false;

    private int teamID;

    //[MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }

    private string roomName = "Room01";
    private int maxPlayer = 5;
    private Vector2 scrollPosition;

    // Use this for initialization
    void Start () {
        DeletePlayerPrefs();
        stat = status.inMenu;

        //allocate space for spawnspots
        //the length of SpawnSpotsFast and SpawnSpotsSuper is half of the total number of SpawnSpots
        //this is because there are always the same number of spawnspots per team
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        spawnSpotsFast = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsSuper = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsNoTeam = new SpawnSpot[spawnSpots.Length];
        foreach(SpawnSpot sp in spawnSpots)
        {
            if (sp.teamid == 0)
            {
                spawnSpotsNoTeam[indNoTeam] = sp;
                indNoTeam++;
            }
            if (sp.teamid == 1) //teamid 0 for free for all, 1 for fastfood and 2 for superfood
            {

                spawnSpotsFast[indFast] = sp;
                indFast++;
            }
            else if (sp.teamid == 2)
            {

                spawnSpotsSuper[indSuper] = sp;
                indSuper++;
            }
        }
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
        Debug.Log("yeas");
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(nameBox.text, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }

    //override
    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("CountFF: " + (int)PhotonNetwork.room.customProperties ["CountFF"]);
        Debug.Log(PhotonNetwork.room.customProperties.Keys);
        // RESPAWN
        if (respawnTimer > 0 )
        {
            respawnTimer -= Time.deltaTime;

            if(respawnTimer <= 0)
            {
                //respawn the player
                SpawnPlayer(teamID);
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
                if(stat == status.browsing){

                    GUILayout.Space(20);
                    GUI.color = Color.red;
                    GUILayout.Box("Game Rooms");
                    GUI.color = Color.white;
                    GUILayout.Space(20);

                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Width(400), GUILayout.Height(300));

                    foreach (RoomInfo game in PhotonNetwork.GetRoomList()) // Each RoomInfo "game" in the amount of games created "rooms" display the fallowing.
                    {

                        GUI.color = Color.green;
                        GUILayout.Box(game.name + " " + game.playerCount + "/" + game.maxPlayers); //Thus we are in a for loop of games rooms display the game.name provide assigned above, playercount, and max players provided. EX 2/20
                        GUI.color = Color.white;

                        if (GUILayout.Button("Join Room"))
                        {

                            PhotonNetwork.JoinRoom(game.name); // Next to each room there is a button to join the listed game.name in the current loop.
                        }
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndArea();
                }
            }
            else if (stat == status.inGame)
            {
                //player has no team assigned
                if(GUILayout.Button("Team Fastfood"))
                {
                    SpawnPlayer(1);
					int prevValue = (int) PhotonNetwork.room.customProperties["CountFF"];
                    PhotonNetwork.room.customProperties["CountFF"] = prevValue + 1;
                    PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);
                }
                if(GUILayout.Button("Team Superfood"))
                {
                    SpawnPlayer(2);
                }
                if(GUILayout.Button("Random Select"))
                {
                    SpawnPlayer(Random.Range(1, 3)); //random number 1 or 2
                }
                if(GUILayout.Button("No Team"))
                {
                    SpawnPlayer(0);
                }
                if(GUILayout.Button("Join Random Game"))
                {
                    ConnectRandom();
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

        //RoomOptions newRoomOptions = new RoomOptions();
        //newRoomOptions.isOpen = true;
        //newRoomOptions.isVisible = true;
        //newRoomOptions.maxPlayers = (byte) maxPlayer;
        //newRoomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        //newRoomOptions.customRoomProperties["CountFF"] = 0;
        //newRoomOptions.customRoomProperties["CountSF"] = 0;
        //newRoomOptions.customRoomPropertiesForLobby = new string[] { "CountFF", "CountSF" };

        string[] roomPropsInLobby = { "CountFF", "CountSF" };
        ExitGames.Client.Photon.Hashtable customRoomProps = new ExitGames.Client.Photon.Hashtable() { { "CountFF", 0 }, { "CountSF", 0 } };
        
        Debug.Log(roomName);
        //PhotonNetwork.CreateRoom(roomName, newRoomOptions, TypedLobby.Default);

        PhotonNetwork.CreateRoom(roomName, true, true, maxPlayer, customRoomProps, roomPropsInLobby);
    }

    void OnJoinedRoom()
    {
        stat = status.inGame;
        //SpawnPlayer();

    }

    void SpawnPlayer(int teamID)
    {
        this.teamID = teamID;
        if(spawnSpots == null)
        {
            return;
        }

        //determine where to spawn
        SpawnSpot mySpawnSpot;
        string prefabName;
        if(teamID == 1) //fastfood
        {
            mySpawnSpot = spawnSpotsFast[Random.Range(0, (int)(spawnSpots.Length * 0.5))];
            prefabName = "playerHuman";
        }
        else if(teamID == 2) //superfood
        {
            mySpawnSpot = spawnSpotsSuper[Random.Range(0, (int)(spawnSpots.Length * 0.5))];
            prefabName = "playerHipster";
        }
        else //no team food
        {
            mySpawnSpot = spawnSpotsNoTeam[Random.Range(0, spawnSpots.Length)];
            prefabName = "player4";
        }

        //mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject player = PhotonNetwork.Instantiate(prefabName , mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0); //local player spawned
        player.GetComponent<playerMovement>().enabled = true;
        player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<playerShooting>().enabled = true;
        player.transform.FindChild("Main Camera").gameObject.SetActive(true);

        //set teamID, TODO: set colour
        player.GetComponent<PhotonView>().RPC("SetTeamID", PhotonTargets.AllBuffered, teamID);

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

            if (roomName != "" && maxPlayer > 0) // if the room name has a name and max players are larger then 0
            {
                PhotonNetwork.CreateRoom(roomName, true, true, maxPlayer); // then create a photon room visible , and open with the maxplayers provide by user.

            }
        }

    }


//RectTransform rt = but.GetComponent<RectTransform>();
//            rt.anchorMin = new Vector2(0.5f, 1);
//            rt.anchorMax = new Vector2(0.5f, 1);
//            rt.anchoredPosition = new Vector2(0, -10-30*i);
//            but.GetComponentInChildren<Text>().text = game.name + " (" + game.playerCount + "/" + game.maxPlayers + ")";
//            Button b = but.GetComponent<Button>();
//            b.onClick.AddListener(() =>
//            //{
//                //JoinRoom(but.GetComponentInChildren<Text>().text);
//               // GameObject cv = GameObject.Find("Canvas");
//                //foreach (Transform childTF in cv.transform)
//                //{
//                    //if (childTF.CompareTag("menu_only"))
//                    //{
//                       // childTF.gameObject.SetActive(false);
//                    //}
//                //}
//            });
            
//        }
//    }

    void OnReceivedRoomListUpdate()
    {
        if (stat==status.browsing)
        {
            BrowseGames();
        }
    }

    public void clearRoomButtons()
    {

    }
    
    public void setStatus(status st)
    {
        stat = st;
    }

}


public enum status
{
    inGame,
    browsing,
    inMenu,
}