using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreManager : Photon.MonoBehaviour {
    public static int numberOfSuperPickups;
    public static int numberOfFastPickups;
    private GameObject[] pickupSuperList, pickupFastList, endgameTextList;
    private PhotonView pv;
    private Pickup[] pu, puTrump, puWholo;
    private int winningTeamID, myTeamID;
    public Text txt;
    private GameObject gameOverCanvas;
    private bool winFlag;

	public DataController dc = new DataController();
    private AudioSource audio1;
    public AudioClip victory, defeat;
    public int capturedBurgers, capturedQuinoa;

    public Text endGameText;
	// Use this for initialization
	void Start () {

        audio1 = GetComponent<AudioSource>();
        gameOverCanvas = GameObject.FindGameObjectWithTag("gameovercanvas");
        gameOverCanvas.SetActive(false);

        winFlag = false;
        pv = GetComponent<PhotonView>();

		PhotonNetwork.player.customProperties ["Won"] = 0;
		PhotonNetwork.player.customProperties ["Lost"] = 0;
		PhotonNetwork.player.SetCustomProperties (PhotonNetwork.player.customProperties);
	}
	
    public void InitializePickupList()
    {
        pickupFastList = GameObject.FindGameObjectsWithTag("fastfood");
        pickupSuperList = GameObject.FindGameObjectsWithTag("superfood");

        numberOfFastPickups = pickupFastList.Length;
        numberOfSuperPickups = pickupSuperList.Length;
        winFlag = true;
    }


	// Update is called once per frame
	void Update () {


        //if (numberOfFastPickups == 0 && winFlag) //Team Wholo wins
        //{
        //    pv.RPC("EndGame", PhotonTargets.All, 2);
        //}

        //if (numberOfSuperPickups == 0 && winFlag) //Team Trump wins
        //{
        //    pv.RPC("EndGame", PhotonTargets.All, 1);
        //}

        if(capturedBurgers == RandomMatchmaker.numberOfBurgers && winFlag) //teaw Wholo wins
        {
            pv.RPC("EndGame", PhotonTargets.All, 2);
        }

        if(capturedQuinoa == RandomMatchmaker.numberOfQuinoa && winFlag) //team Trump wins
        {
            pv.RPC("EndGame", PhotonTargets.All, 1);
        }

	}

    public void CapturedPickups(string pickupName, bool captured)
    {
        if(pickupName == "fastfood")
        {
            if(captured)
            {
                //capturedBurgers++;
                if (photonView.isMine)
                {
                    pv.RPC("UpdateCapturedBurgers", PhotonTargets.All, true);
                }
            }
            else
            {
                //capturedBurgers--;
                if (photonView.isMine)
                {
                    pv.RPC("UpdateCapturedBurgers", PhotonTargets.All, false);
                }
            }
        }
        else if(pickupName == "superfood")
        {
            if(captured)
            {
                //capturedQuinoa++;
                if (photonView.isMine)
                {
                    pv.RPC("UpdateCapturedQuinoa", PhotonTargets.All, true);
                }
            }
            else
            {
                //capturedQuinoa--;
                if (photonView.isMine)
                {
                    pv.RPC("UpdateCapturedQuinoa", PhotonTargets.All, false);
                }
            }
        }

        Debug.Log("captured burgers: " + capturedBurgers + "\ncaptured quinoa: " + capturedQuinoa);
    }

    [PunRPC]
    void UpdateCapturedQuinoa(bool increment)
    {
        if(increment)
        {
            capturedQuinoa++;
        }
        else
        {
            capturedQuinoa--;
        }
    }

    [PunRPC]
    void UpdateCapturedBurgers(bool increment)
    {
        if (increment)
        {
            capturedBurgers++;
        }
        else
        {
            capturedBurgers--;
        }
    }

    [PunRPC]
    void EndGame(int winningTeamID)
    {
        winFlag = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        gameOverCanvas.SetActive(true);
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().isMine)
            {
                myTeamID = player.gameObject.GetComponent<TeamMember>().teamID;
            }
        }
        if (winningTeamID == 0) //set teamID = 0 for time limit
        {
            endGameText.text = "TIME!";
            if (capturedQuinoa > capturedBurgers)
            {
                Debug.Log("Fast food won :D");
                if (myTeamID == 1)
                {
                    audio1.PlayOneShot(victory, 1);
                    endGameText.text += " You gained more pickups :D";
                    PhotonNetwork.player.customProperties["Won"] = 1;
                }
                else
                {
                    audio1.PlayOneShot(defeat, 1);
                    endGameText.text += " The burgers took more pickups :(";
                    PhotonNetwork.player.customProperties["Lost"] = 1;
                }
            }
            else if (capturedQuinoa < capturedBurgers)
            {
                Debug.Log("Super food won :D");
                if (myTeamID == 2)
                {
                    audio1.PlayOneShot(victory, 1);
                    endGameText.text += " You gained more pickups :D";
                    PhotonNetwork.player.customProperties["Won"] = 1;
                }
                else
                {
                    audio1.PlayOneShot(defeat, 1);
                    endGameText.text += " The hipsters took more pickups :(";
                    PhotonNetwork.player.customProperties["Lost"] = 1;
                }
            }
            else
            {
                if ((int)PhotonNetwork.room.customProperties["SFDeaths"] > (int)PhotonNetwork.room.customProperties["FFDeaths"])
                {
                    Debug.Log("Fast food won :D (by kills)");
                    if (myTeamID == 1)
                    {
                        audio1.PlayOneShot(victory, 1);
                        endGameText.text += " You overkilled quinoa :D";
                        PhotonNetwork.player.customProperties["Won"] = 1;
                    }
                    else
                    {
                        audio1.PlayOneShot(defeat, 1);
                        endGameText.text += " You died way more than those burgers :(";
                        PhotonNetwork.player.customProperties["Lost"] = 1;
                    }
                }
                else if ((int)PhotonNetwork.room.customProperties["SFDeaths"] < (int)PhotonNetwork.room.customProperties["FFDeaths"])
                {
                    Debug.Log("Super food won :D (by kills)");
                    if (myTeamID == 2)
                    {
                        audio1.PlayOneShot(victory, 1);
                        endGameText.text += " You overkilled the burgers :D";
                        PhotonNetwork.player.customProperties["Won"] = 1;
                    }
                    else
                    {
                        audio1.PlayOneShot(defeat, 1);
                        endGameText.text += " You died way more than those hipsters :(";
                        PhotonNetwork.player.customProperties["Lost"] = 1;
                    }
                }
                else
                {
                    Debug.Log("Tie!");
                    endGameText.text += " It's a tie!";
                }
            }
        }
        else
        {
            if (myTeamID == winningTeamID) //you are in the winning team, display win screen
            {
                Debug.Log("win");
                audio1.PlayOneShot(victory, 1);
                if (myTeamID == 1) //member of team Trump, display winning screen
                {
                    //endgameTextList[1].SetActive(true);
                    endGameText.text = "You crushed the Quinoa!";
					PhotonNetwork.player.customProperties ["Won"] = 1;

                }
                if (myTeamID == 2) //member of team Wholo, display winning screen
                {
                    //endgameTextList[0].SetActive(true);
                    endGameText.text = "You beat the burger!";
					PhotonNetwork.player.customProperties ["Won"] = 1;
                }
            }
            else //you are in the losing team, display lose screen
            {
                Debug.Log("lose");
                audio1.PlayOneShot(defeat, 1);
                if (myTeamID == 1) //member of team Trump, display losing screen
                {
                    //endgameTextList[2].SetActive(true);
                    endGameText.text = "Quinoa smashed you down";
					PhotonNetwork.player.customProperties ["Lost"] = 1;
                }
                if (myTeamID == 2) //member of team Wholo, display losing screen
                {
                    //endgameTextList[3].SetActive(true);
                    endGameText.text = "The burger whoppe(r)d you down!";
					PhotonNetwork.player.customProperties ["Lost"] = 1;
                }
            }
			PhotonNetwork.player.SetCustomProperties (PhotonNetwork.player.customProperties);
        }
		Debug.Log ("Sent Data to DB");
		dc.sentDBData(PhotonNetwork.playerName);

        endGameText.text += "\nreturning in 5 seconds";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(Reboot(7));   
    }

    IEnumerator Reboot(float waitingTime)
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(waitingTime);
        Application.LoadLevel(Application.loadedLevel);
        Cursor.visible = true;
    }
}
