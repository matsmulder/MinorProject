using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
//using UnityEditor;

public class GameRoomManager : MonoBehaviour {

    //public GameObject player;
    //public GameObject camera1;
    public GameObject standby;
    public GameObject roomButtonPrefab;
    SpawnSpot[] spawnSpots;
    SpawnSpot[] spawnSpotsNoTeam; //free for all spawnspots
    SpawnSpot[] spawnSpotsSuper; //hipster spawnspots
    SpawnSpot[] spawnSpotsFast; //fastfood spawnspots
    List<GameRoom> gamePropsList = new List<GameRoom>();

    private int indNoTeam = 0, indFast = 0, indSuper = 0;
    string type = "random";
    public Text nameBox;

    public float respawnTimer;
    public bool offlineMode;
    status stat;
    private bool pickedTeam = false;
    private int teamID = 10;

    //[MenuItem("Edit/Reset Playerprefs")]
    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }
   
    private string roomName = "Room01";
    private RoomInfo gameRoom;
    private int maxPlayer = 6;
    private Vector2 scrollPosition;

    // Use this for initialization
    void Start () {
        DeletePlayerPrefs();
        stat = status.inMenu;

        //allocate space for spawnspots
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        spawnSpotsFast = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];                         //the length of SpawnSpotsFast and SpawnSpotsSuper is half of the total number of SpawnSpots
        spawnSpotsSuper = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];                        //this is because there are always the same number of spawnspots per team
        spawnSpotsNoTeam = new SpawnSpot[spawnSpots.Length];
        foreach(SpawnSpot sp in spawnSpots)
        {
            if (sp.teamid == 0)                 //teamid 0 for free for all
            {
                spawnSpotsNoTeam[indNoTeam] = sp;
                indNoTeam++;
            }
            if (sp.teamid == 1)                 // 1 for fastfood 
            {

                spawnSpotsFast[indFast] = sp;
                indFast++;
            }
            else if (sp.teamid == 2)            // 2 for superfood
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

        // initialisation
        //RoomInfo[] gameRooms = PhotonNetwork.GetRoomList();

        //// checks if our gamePropsList is up to date. (removal will happen after a game has finished)
        //foreach (RoomInfo game in gameRooms) 
        //{
        //    foreach(GameRoom index in gamePropsList){
        //        if (index.getRoomName().Equals(game.name)){
        //            break;
        //        }
        //        else{
        //            gamePropsList.Add(new GameRoom(game.name));
        //        }
        //    }
        //}

        // Initial spawn (when person has a team selected and the amount of players in the room is equal to 6)
        //if (teamID != 10 && PhotonNetwork.room.playerCount == 6)
        //{
        //    SpawnPlayer(teamID);
        //}

        //// Respawn
        //if (respawnTimer > 0 )
        //{
        //    respawnTimer -= Time.deltaTime;

        //    if(respawnTimer <= 0)
        //    {
        //        //respawn the player
        //        SpawnPlayer(teamID);
        //    }
        //}
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
            //if(pickedTeam)
            if (true)
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
                    if( checkConditions(1))
                    {
                        teamID = 1;
                    }
                }
                if(GUILayout.Button("Team Superfood"))
                {
                    if (checkConditions(2))
                    {
                        teamID = 2;
                    }
                }
                if(GUILayout.Button("Random Select"))
                {
                    int rand = Random.Range(1, 3);      //random number 1 or 2

                    if (checkConditions(rand))
                    {
                        teamID = rand;
                    }
                }
                if(GUILayout.Button("No Team"))
                {
                    if (checkConditions(0))
                    {
                        teamID = 0;
                    }
                }
                //if(GUILayout.Button("Join Random Game"))
                //{
                //    ConnectRandom();
                //}

            }

        }
    }

    public bool checkConditions(int teamID){
        // initialisation
        RoomInfo currentRoom = PhotonNetwork.room;

        foreach (GameRoom room in gamePropsList)
        {
            if (room.getRoomName().Equals(currentRoom.name))
            {
                if(teamID == 1){
                    if(room.getAmountFF() < 3){
                        room.incrementAmountFF();
                        return true;
                    }
                }
                else if (teamID == 2){
                    if (room.getAmountSF() < 3)
                    {
                        room.incrementAmountSF();
                        return true;
                    }

                }
                else{                                       // this is the deathmatch variant. and for extension.
                    return false;
                }
               
            }
        }
  
        return false;
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
        stat = status.inGame;
        //SpawnPlayer();

    }

    void SpawnPlayer(int teamID)
    {
        this.teamID = teamID;
        if(spawnSpots == null)
        {
            Debug.Log("no spawnspots found");
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
    //rt.anchorMin = new Vector2(0.5f, 1);
    //rt.anchorMax = new Vector2(0.5f, 1);
    //rt.anchoredPosition = new Vector2(0, -10-30*i);
    //but.GetComponentInChildren<Text>().text = game.name + " (" + game.playerCount + "/" + game.maxPlayers + ")";
    //        Button b = but.GetComponent<Button>();
    //b.onClick.AddListener(() =>
    //        //{
    //            //JoinRoom(but.GetComponentInChildren<Text>().text);
    //           // GameObject cv = GameObject.Find("Canvas");
    //            //foreach (Transform childTF in cv.transform)
    //            //{
    //                //if (childTF.CompareTag("menu_only"))
    //                //{
    //                   // childTF.gameObject.SetActive(false);
    //                //}
    //            //}
    //        });
            
    //    }
    //}

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