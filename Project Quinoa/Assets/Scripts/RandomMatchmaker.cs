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
    public double startTime;
    public float respawnTimer;
    private bool ready = false;
    public bool offlineMode;
    status stat;
    private bool once;
    private bool allready;
    public bool allPickedUp;
    private int teamID;
    private int restTimeMin;
    private int restTimeSec;
    private double restTimeMinDouble;
    public int totalRoundTime;
    private string roomName = "Room01";  // <- This should be a Random room name.
    private int maxPlayer;
    private Vector2 scrollPosition;
	private bool gameStarted;

    // GUI
    public Button createRoom;
    public bool inLobbyScreen;
    public bool inCreateGameScreen;
    public bool inGameScreen;
    public bool inReadyScreen;
	public Text[] lobbyNames;
	public Text lobbyName1;
	public Text lobbyName2;
	public Text lobbyName3;
	public Text[] lobbyPlayers;
	public Text lobbyPlayers1;
	public Text lobbyPlayers2;
	public Text lobbyPlayers3;
	public Button[] lobbyButtons;
	public Button lobbyButton1;
	public Button lobbyButton2;
	public Button lobbyButton3;
	public Text createGameName;
	public Text createGameMaxPlayers;
	public Button createGame;
	public Text currentGameName;
	public Text currentAmountPlayers;
	public Dropdown team;
	public Button play_button;
	public Text readyAmountPlayers;
	public Button notReady;
	public GameObject panel_Setready;
	public GameObject panel_createinputfield;
	public GameObject panel_joininputfield;
	public GameObject canvas_Ready;
	public GameObject teamFull;
	public GameObject canvas_Objective;

    public Text SliderText;
    public Slider numberOfBots;
    private int realNumberOfBots;
    private scoreManager sm;

    private Calculator calc;

    public static bool inRoom = false;

    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }

    // Use this for initialization
    void Start() {
        once = true;
        DeletePlayerPrefs();
        stat = status.inMenu;
        PlayerPrefs.DeleteAll();

        
        //Put the buttons and text from the GameLobby in a 2D array.
        lobbyButtons = new Button[3];
        lobbyButtons[0] = lobbyButton1;
        lobbyButtons[1] = lobbyButton2;
        lobbyButtons[2] = lobbyButton3;
        lobbyPlayers = new Text[3];
        lobbyPlayers[0] = lobbyPlayers1;
        lobbyPlayers[1] = lobbyPlayers2;
        lobbyPlayers[2] = lobbyPlayers3;
        lobbyNames = new Text[3];
        lobbyNames[0] = lobbyName1;
        lobbyNames[1] = lobbyName2;
        lobbyNames[2] = lobbyName3;
        //allocate space for spawnspots
        //the length of SpawnSpotsFast and SpawnSpotsSuper is half of the total number of SpawnSpots
        //this is because there are always the same number of spawnspots per team
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        spawnSpotsFast = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsSuper = new SpawnSpot[(int)(spawnSpots.Length * 0.5)];
        spawnSpotsNoTeam = new SpawnSpot[spawnSpots.Length];
        foreach (SpawnSpot sp in spawnSpots)
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

        sm = GetComponent<scoreManager>();

        if (offlineMode)
        {
            OnJoinedLobby();            //For future extensions
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.4");
        }

        calc = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
    }
    
    public void ConnectRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        //PhotonNetwork.JoinOrCreateRoom(nameBox.text, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }

    //override
    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions() { isVisible = true }, TypedLobby.Default);
    }

    // Update is called once per frame
    void Update() {
	
        // checks status 
        inLobbyScreen = panel_joininputfield.GetActive();
		inCreateGameScreen = panel_createinputfield.GetActive();
		inGameScreen = panel_Setready.GetActive();
		inReadyScreen = canvas_Ready.GetActive();

		if (PhotonNetwork.room != null) {
			if (PhotonNetwork.room.customProperties.ContainsKey ("StartingTime")) {
				restTimeMinDouble = totalRoundTime - (PhotonNetwork.time - (double)PhotonNetwork.room.customProperties ["StartingTime"]) / 60;

				restTimeMin = (int)Math.Truncate (restTimeMinDouble);
				restTimeSec = (int)((restTimeMinDouble - restTimeMin) * 60);
			}
		}

		if (restTimeMinDouble < 0) {

			sm.GetComponent<PhotonView> ().RPC ("EndGame", PhotonTargets.All, 0);

			PhotonNetwork.player.customProperties["Lost"] = 1;
			PhotonNetwork.player.customProperties["Won"] = 0;
			PhotonNetwork.player.SetCustomProperties (PhotonNetwork.player.customProperties);

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			StartCoroutine(Reboot(5));   
			//Application.LoadLevel ("Game_Over");
		} else if (restTimeMinDouble > (double)9.92) {
			canvas_Objective.SetActive (true);
		} else {
			canvas_Objective.SetActive (false);
		}

        // Checks if all players are ready (if so, spawn all players)
        if (once)
        {
            allready = true;
            foreach (PhotonPlayer player in PhotonNetwork.playerList)
            {
                //Debug.Log("player ready?");
                if (player.customProperties.ContainsKey("Ready"))
                {
                    if ((bool)player.customProperties["Ready"] != true)
                    {
                        allready = false;
                        break;
                    }
                }
                else
                {
                    allready = false;
                    break;
                }
            }
        }

		// Initial spawn
		if (PhotonNetwork.room != null) {
			if (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers && allready && once) {  //if the room is full and all players are ready, spawn the players.
				PhotonNetwork.room.customProperties ["StartingTime"] = PhotonNetwork.time;
				PhotonNetwork.room.SetCustomProperties (PhotonNetwork.room.customProperties);
                stat = status.inGame;
                Debug.Log("initial spawn");
				SpawnPlayer(teamID);
				once = false;
                allready = false;
				gameStarted = true;
				canvas_Ready.SetActive(false);
                GameObject.FindGameObjectWithTag("canvas").SetActive(false);
			}
		}
            
		// RESPAWN
		if (respawnTimer > 0) {
			respawnTimer -= Time.deltaTime;

			if (respawnTimer <= 0) {
                //respawn the player
                Debug.Log("respawn");
				SpawnPlayer(teamID);
			}
		}

		/////GUI/////
		//GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		if (PhotonNetwork.connected == false) {
			//not yet connected, ask for online/offline <UPCOMING>
		}

		if (PhotonNetwork.connected == true) {
			if (inLobbyScreen) {
				// update info on the GUI
				if (PhotonNetwork.GetRoomList ().Length > 0) {
					RoomInfo[] roomList = PhotonNetwork.GetRoomList ();
					for (int i = 0; i < PhotonNetwork.GetRoomList().Length; i++) {
						lobbyNames [i].text = roomList [i].name;
						lobbyPlayers [i].text = roomList [i].playerCount.ToString();
					}
				}
			}
			//if (inCreateGameScreen) {
			// 		<- this is done in an public void method and is called when the create game button is clicked.
			//}
			if (inGameScreen) {
                //				currentGameName.text = PhotonNetwork.room.playerCount.ToString();
                currentGameName.text = createGameName.text;
                if (PhotonNetwork.room != null)
                {
                    currentAmountPlayers.text = PhotonNetwork.room.playerCount.ToString();
                }
			}
			if (inReadyScreen) {
				readyAmountPlayers.text = ((int)PhotonNetwork.room.playerCount + "/" + (int)PhotonNetwork.room.maxPlayers);
			}
		}
	}

	public void onPlayButtonClicked(){
	if(team.value == 0)	// fastfood
		{
			if (checkJoinConditions(1))
			{
				teamID = 1;
				UpdateCustomProperties("CountFF", true);
				canvas_Ready.SetActive(true);
			}
			else
			{
				//DisplayDialog("Amount of FastFoodLovers allready full, please choose the other team");
			}
		}
		else{					// superfood
			if (checkJoinConditions(2))
			{
				teamID = 2;
				UpdateCustomProperties("CountSF", true);
				canvas_Ready.SetActive(true);
			}
			else
			{
				//MessageBox.Show("Amount of SuperFoodLovers allready full, please choose the other team");
			}
		}
	}


    //check for offline/online mode
    public void OfflineModeButtonClicked()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom("offline");
        offlineMode = true;
        calc.enabled = true;
        Debug.Log("offline");
    }

    public void OnlineModeButtonClicked()
    {
        PhotonNetwork.offlineMode = false;
        offlineMode = false;
        PhotonNetwork.ConnectUsingSettings("0.5");
        calc.enabled = false;
    }

    public void GetBotSliderValue()
    {
        SliderText.text = ((int)numberOfBots.value).ToString();
    }

    public void onLobbyButtonClicked(int button)
    {
        JoinRoom(lobbyNames[button].text);
        panel_joininputfield.SetActive(false);
        panel_Setready.SetActive(true);
        inRoom = true;
    }

	public void onNotReadyClicked(){
		if (teamID == 1)
		{
			UpdateCustomProperties("CountFF", false);
		}
		else
		{
			UpdateCustomProperties("CountSF", false);
		}
		canvas_Ready.SetActive(false);
	}
	
	public void onClickedCreateGame(){
		int num;
		if (!createGameName.text.Equals ("")) {
            if (int.Parse(createGameMaxPlayers.text) < 2){
				maxPlayer = 2;
			}
			else{
				maxPlayer = int.Parse(createGameMaxPlayers.text);
			}
            string[] roomPropsInLobby = { "CountFF", "CountSF", "StartingTime"};
			ExitGames.Client.Photon.Hashtable customRoomProps = new ExitGames.Client.Photon.Hashtable() { { "CountFF", 0 }, { "CountSF", 0 } };

			PhotonNetwork.CreateRoom(createGameName.text, true, true, maxPlayer, customRoomProps, roomPropsInLobby);

			panel_createinputfield.SetActive(false);
			panel_Setready.SetActive(true);
            inRoom = true;
		}
	}
	
    void OnGUI()
    {
       
		if (gameStarted) {
			GUILayout.Label ("Remaining time: " + restTimeMin + " Min " + restTimeSec + " Sec");
		}
	}

    public void OnJoinedLobby()
    {
        
    }

    public void UpdateCustomProperties(string customProp, bool increment)
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
        maxPlayer = PhotonNetwork.room.maxPlayers;
        if(ID == 1)
        {
            if((int) PhotonNetwork.room.customProperties["CountFF"] < 0.5 * maxPlayer)
            {
                teamFull.SetActive(false);
                return true;
            }
            else
            {
                teamFull.SetActive(true);
                return false;
            }

        }
        else if(ID == 2){
            if ((int)PhotonNetwork.room.customProperties["CountSF"] < 0.5 * maxPlayer)
            {
                teamFull.SetActive(false);
				return true;
			}
            else
            {
                teamFull.SetActive(true);
                return false;
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
        string[] roomPropsInLobby = { "CountFF", "CountSF", "StartingTime"};
        ExitGames.Client.Photon.Hashtable customRoomProps = new ExitGames.Client.Photon.Hashtable() { { "CountFF", 0 }, { "CountSF", 0 } };

        PhotonNetwork.CreateRoom(roomName, true, true, maxPlayer, customRoomProps, roomPropsInLobby);
    }

    void OnJoinedRoom()
    {
        //ALWAY INITIALIZE
        PhotonNetwork.room.customProperties["CountFF"] = 0;
        PhotonNetwork.room.customProperties["CountSF"] = 0;
        PhotonNetwork.player.customProperties["Lost"] = 0;
        PhotonNetwork.player.customProperties["Won"] = 0;
        PhotonNetwork.player.SetCustomProperties(PhotonNetwork.player.customProperties);
        PhotonNetwork.room.SetCustomProperties(PhotonNetwork.room.customProperties);

        stat = status.inLobby;
    }

    public void OnSliderChanged()
    {
        realNumberOfBots = (int)numberOfBots.value;
        SliderText.text = ((int)numberOfBots.value).ToString();
    }


    public void OfflineButtonFastClicked()
    {
        if(calc.enabled!=true)
        {
            calc.enabled = true;
        }
        SpawnPlayer(1);
        SpawnBot(1, realNumberOfBots);

    }
    
    public void OfflineButtonSuperClicked()
    {
        if (calc.enabled != true)
        {
            calc.enabled = true;
        }
        SpawnPlayer(2);
        SpawnBot(2, realNumberOfBots);
    }

    void SpawnPlayer(int teamID)
    {
        Debug.Log("spawn with teamID" + teamID);
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
        player.GetComponent<Bot>().enabled = false;
        player.GetComponent<SphereCollider>().enabled = false;
        player.transform.FindChild("Main Camera").gameObject.SetActive(true);

        //set teamID, TODO: set colour
        player.GetComponent<PhotonView>().RPC("SetTeamID", PhotonTargets.AllBuffered, teamID);

        //GameObject camera1 = PhotonNetwork.Instantiate("MainCamera", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standby.SetActive(false);
    }

    void SpawnBot(int playerTeamID, int numberOfBots)
    {
        Debug.Log("entered spawnBot, numberOfBots:" + numberOfBots);
        int previousTeamID = playerTeamID;
        for (int i = 0; i < numberOfBots; i++)
        {
            Debug.Log("previous team id is: " + previousTeamID);
            if(previousTeamID == 1)//previous spawned instance was of team Trump, spawn Wholo now
            {
                SpawnWholoBot();
                previousTeamID = 2;
            }
            else if(previousTeamID == 2)//previous spawned instance was of team Wholo, spawn Trump now
            {
                SpawnTrumpBot();
                previousTeamID = 1;
            }
        }
    }

    void SpawnTrumpBot()
    {
        SpawnSpot mySpawnSpot = spawnSpotsFast[UnityEngine.Random.Range(0, (int)(spawnSpots.Length * 0.5))];
        GameObject bot = PhotonNetwork.Instantiate("playerHuman", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0); //bot spawned
        bot.GetComponent<SphereCollider>().enabled = true;
        bot.GetComponent<Bot>().enabled = true;
        bot.GetComponent<PhotonView>().RPC("SetTeamID", PhotonTargets.AllBuffered, teamID); //set teamID

    }

    void SpawnWholoBot()
    {
        SpawnSpot mySpawnSpot = spawnSpotsSuper[UnityEngine.Random.Range(0, (int)(spawnSpots.Length * 0.5))];
        GameObject bot = PhotonNetwork.Instantiate("playerHipster", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0); //bot spawned
        bot.GetComponent<SphereCollider>().enabled = true;
        bot.GetComponent<Bot>().enabled = true;
        bot.GetComponent<PhotonView>().RPC("SetTeamID", PhotonTargets.AllBuffered, teamID); //set teamID
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

	IEnumerator Reboot(float waitingTime)
	{
		PhotonNetwork.LeaveRoom();
		yield return new WaitForSeconds(waitingTime);
		Application.LoadLevel(Application.loadedLevel);
		Cursor.visible = true;
		Debug.Log("testarossa");
	}
}

public enum status
{
    inLobby,
    browsing,
    inMenu,
    inGame,
}