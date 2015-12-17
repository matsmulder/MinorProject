using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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
    string type = "Random";
    public Text nameBox;
    public DateTime startTime;

    public float respawnTimer;
    private bool ready = false;
    public bool offlineMode;
    status stat;
    private bool once = true;
    public bool allPickedUp;
    private int teamID;
    private int restTimeMin;
    private int restTimeSec;
    private double restTimeMinDouble;
    private int totalRoundTime = 10;

    //[MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }

    private string roomName = "Room01";  // <- This should be a Random room name.
    private int maxPlayer = 2;
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
    void Update() {
        Debug.Log("CountFF: " + PhotonNetwork.room.customProperties["CountFF"]);

        //Checks if ten minutes have passed or all pick ups has been picked up.
        restTimeMinDouble = totalRoundTime - (DateTime.UtcNow - startTime).TotalMinutes;
        if (restTimeMinDouble > 0 || allPickedUp)
        {
            // load score screen, wait and then return to the Lobby
        }
        restTimeMin = (int) Math.Truncate(restTimeMinDouble);
        restTimeSec = (int) (restTimeMinDouble - restTimeMin) * 60;

        // Checks if all players are ready (if so, spawn all players)
        bool allready = true;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if((bool) player.customProperties["Ready"] == false)
            {
                allready = false;
                startTime = DateTime.UtcNow;
                PhotonNetwork.room.customProperties["StartingTime"] = startTime;
                PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);
                stat = status.inGame;
                break;
            }
        }

        // Initial spawn
        if (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers && allready && once)                //if the room is full and all players are ready, spawn the players.
        {
            SpawnPlayer(teamID);
            once = false;
        }
            
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
        //GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        if(PhotonNetwork.connected == false)
        {
            //not yet connected, ask for online/offline <UPCOMING>
        }

        if (PhotonNetwork.connected == true)
        {
            if (stat == status.browsing)
            {

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
            else if (stat == status.inLobby)
            {
                //player has no team assigned
                if (GUILayout.Button("Team Fastfood"))
                {
                    if (checkJoinConditions(1))
                    {
                        teamID = 1;
                        UpdateCustomProperties("CountFF", true);
                    }
                }
                if (GUILayout.Button("Team Superfood"))
                {
                    if (checkJoinConditions(2))
                    {
                        teamID = 2;
                        UpdateCustomProperties("CountSF", true);
                    }
                }
                if (GUILayout.Button("Random Select"))
                {
                    int rand = UnityEngine.Random.Range(1, 3);                  // UnityEngine.Random number 1 or 2
                    if (rand == 1)
                    {
                        if (checkJoinConditions(1))
                        {
                            teamID = 1;
                            UpdateCustomProperties("CountFF", true);
                        }
                        else
                        {
                            teamID = 2;
                            UpdateCustomProperties("CountSF", true);
                        }
                    }
                    else
                    {
                        if (checkJoinConditions(2))
                        {
                            teamID = 2;
                            UpdateCustomProperties("CountSF", true);
                        }
                        else
                        {
                            teamID = 1;
                            UpdateCustomProperties("CountFF", true);
                        }
                    }
                }
                if (GUILayout.Button("No Team"))
                {
                    //SpawnPlayer(0);                                 // not yet implemented
                }
                if (GUILayout.Button("Join Random Game"))
                {
                    ConnectRandom();
                }

                if (ready)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("I'm Ready!");         ////Ready Label
                    if (GUILayout.Button("Not ready"))      ////Ready Button
                    {
                        if (teamID == 1)
                        {
                            UpdateCustomProperties("CountSF", false);
                        }
                        else
                        {
                            UpdateCustomProperties("CountFF", false);
                        }

                    }
                }
            }
            else if (stat == status.inGame)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Remaining time: " + restTimeMin + " Min " + restTimeSec + " Sec");
            }
        }
    }

    public void OnJoinedLobby()
    {   
        
    }

    public void UpdateCustomProperties (string customProp, bool increment)
    {
        if (increment)
        {
            int prevValue = (int)PhotonNetwork.room.customProperties[customProp];
            PhotonNetwork.room.customProperties[customProp] = prevValue + 1;
            PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);
            ready = true;
            PhotonNetwork.player.customProperties["Ready"] = true;
            PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.customProperties);
        }
        else
        {
            int prevValue = (int)PhotonNetwork.room.customProperties[customProp];
            PhotonNetwork.room.customProperties[customProp] = prevValue - 1;
            PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);
            ready = false;
            PhotonNetwork.player.customProperties["Ready"] = false;
            PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.customProperties);
        }
    }

    public bool checkJoinConditions(int ID)
    {
        if(ID == 1)
        {
            if((int) PhotonNetwork.room.customProperties["CountFF"] < 0.5*maxPlayer && PhotonNetwork.room.playerCount < maxPlayer)
            {
                return true;
            }
        }
        else if(ID == 2){
            if ((int)PhotonNetwork.room.customProperties["CountSF"] < 0.5*maxPlayer && PhotonNetwork.room.playerCount < maxPlayer)
            {
                return true;
            }
        }
        else
        {
            return false;
        }
        return false;
    }

    void OnPhotonRandomJoinFailed()
    {
        string[] roomPropsInLobby = { "CountFF", "CountSF", "StartingTime" };
        ExitGames.Client.Photon.Hashtable customRoomProps = new ExitGames.Client.Photon.Hashtable() { { "CountFF", 0 }, { "CountSF", 0 } };

        PhotonNetwork.CreateRoom(roomName, true, true, maxPlayer, customRoomProps, roomPropsInLobby);
    }

    void OnJoinedRoom()
    {
        stat = status.inLobby;
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
            mySpawnSpot = spawnSpotsFast[UnityEngine.Random.Range(0, (int)(spawnSpots.Length * 0.5))];
            prefabName = "playerHuman";
        }
        else if(teamID == 2) //superfood
        {
            mySpawnSpot = spawnSpotsSuper[UnityEngine.Random.Range(0, (int)(spawnSpots.Length * 0.5))];
            prefabName = "playerHipster";
        }
        else //no team food
        {
            mySpawnSpot = spawnSpotsNoTeam[UnityEngine.Random.Range(0, spawnSpots.Length)];
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
    inLobby,
    browsing,
    inMenu,
    inGame,
}