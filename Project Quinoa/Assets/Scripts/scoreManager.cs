using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreManager : MonoBehaviour {
    public static int numberOfSuperPickups;
    public static int numberOfFastPickups;
    private GameObject[] pickupSuperList, pickupFastList, endgameTextList;
    private PhotonView pv;
    private Pickup[] pu, puTrump, puWholo;
    private int winningTeamID, myTeamID;
    public Text txt;
    private GameObject gameOverCanvas;
    private bool winFlag;

    public Text endGameText;
	// Use this for initialization
	void Start () {
       

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


        if (numberOfFastPickups == 0 && winFlag) //Team Wholo wins
        {
            pv.RPC("EndGame", PhotonTargets.All, 2);
        }

        if (numberOfSuperPickups == 0 && winFlag) //Team Trump wins
        {
            pv.RPC("EndGame", PhotonTargets.All, 1);
        }

	}

    [PunRPC]
    void EndGame(int winningTeamID)
    {
        winFlag = false;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        gameOverCanvas.SetActive(true);

        if (winningTeamID == 0) //set teamID = 0 for time limit
        {
            //endgameTextList[4].SetActive(true);               //Never assigned
            endGameText.text = "TIME!";
        }
        else
        {
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PhotonView>().isMine)
                {
                    myTeamID = player.gameObject.GetComponent<TeamMember>().teamID;
                }
            }

            if (myTeamID == winningTeamID) //you are in the winning team, display win screen
            {
                Debug.Log("win");
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

        endGameText.text += "\nreturning in 5 seconds";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(Reboot(5));   
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
