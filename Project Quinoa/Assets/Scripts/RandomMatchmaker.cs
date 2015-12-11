using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEditor;

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

    // Use this for initialization
    void Start () {
        DeletePlayerPrefs();
        stat = status.inMenu;

        //allocate space for spawnspots
        //the length of SpawnSpotsFast and SpawnSpotsSuper is half of the total number of SpawnSpots
        //this is because there are always the same number of spawnspots per team
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Debug.Log(spawnSpots.Length + "spawnSpots length");
        spawnSpotsFast = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsSuper = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsNoTeam = new SpawnSpot[spawnSpots.Length];
        foreach(SpawnSpot sp in spawnSpots)
        {
            if (sp.teamid == 0)
            {
                Debug.Log(indNoTeam + "indNoTeam");
                spawnSpotsNoTeam[indNoTeam] = sp;
                indNoTeam++;
            }
            if (sp.teamid == 1) //teamid 0 for free for all, 1 for fastfood and 2 for superfood
            {
                Debug.Log(indFast + "indFast");
                spawnSpotsFast[indFast] = sp;
                indFast++;
            }
            else if (sp.teamid == 2)
            {
                Debug.Log(indNoTeam + "indNoTeam");
                spawnSpotsSuper[indSuper] = sp;
                indSuper++;
            }
            Debug.Log(sp.teamid + "teamid");
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

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }
	
	// Update is called once per frame
	void Update () {
        if(respawnTimer > 0 )
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

            }
            else if (stat == status.inGame)
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
        if(teamID == 1) //fastfood
        {
            mySpawnSpot = spawnSpotsFast[Random.Range(0, (int)(spawnSpots.Length * 0.5))];
        }
        else if(teamID == 2) //superfood
        {
            mySpawnSpot = spawnSpotsSuper[Random.Range(0, (int)(spawnSpots.Length * 0.5))];
        }
        else //no team food
        {
            mySpawnSpot = spawnSpotsNoTeam[Random.Range(0, spawnSpots.Length)];
        }

        //mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];




        GameObject player = PhotonNetwork.Instantiate("player4", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0); //local player spawned
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

            RectTransform rt = but.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -10-30*i);
            but.GetComponentInChildren<Text>().text = game.name + " (" + game.playerCount + "/" + game.maxPlayers + ")";
            Button b = but.GetComponent<Button>();
            b.onClick.AddListener(() =>
            {
                JoinRoom(but.GetComponentInChildren<Text>().text);
                GameObject cv = GameObject.Find("Canvas");
                foreach (Transform childTF in cv.transform)
                {
                    if (childTF.CompareTag("menu_only"))
                    {
                        childTF.gameObject.SetActive(false);
                    }
                }
            });
            
        }
    }

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