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
	// Use this for initialization
	void Start () {
       pickupFastList = GameObject.FindGameObjectsWithTag("fastfood");
        pickupSuperList = GameObject.FindGameObjectsWithTag("superfood");

        endgameTextList = GameObject.FindGameObjectsWithTag("endgametext");
        gameOverCanvas = GameObject.FindGameObjectWithTag("gameovercanvas");

        foreach(GameObject endgametext in endgameTextList)
        {
            Debug.Log(endgametext.name);
            endgametext.SetActive(false);
        }
                gameOverCanvas.SetActive(false);

        winFlag = true;
        pv = GetComponent<PhotonView>();
        //pu = FindObjectsOfType<Pickup>();
        //int i = 0, j = 0, k = 0;
        //foreach (Pickup child in pu)
        //{
        //    if(child.tm.teamID == 1) //members of team Trump
        //    {
        //        puTrump[j] = pu[i];
        //        j++;
        //        Debug.Log("Trump");
        //    }
        //    if(child.tm.teamID == 2) //members of team Wholo
        //    {
        //        puWholo[k] = pu[i];
        //        k++;
        //        Debug.Log("wholo");
        //    }
        //    i++;
        //}
        numberOfFastPickups = pickupFastList.Length;
        numberOfSuperPickups = pickupSuperList.Length;
	}
	
	// Update is called once per frame
	void Update () {


        if (numberOfFastPickups == 0 && winFlag) //Team Wholo wins
        {
            //StartCoroutine(Win("Wholo"));
            //pu.EndGame();
            pv.RPC("EndGame", PhotonTargets.All, 2);
        }

        if (numberOfSuperPickups == 0 && winFlag) //Team Trump wins
        {
            //StartCoroutine(Win("Trump"));
            //pu.EndGame();
            pv.RPC("EndGame", PhotonTargets.All, 1);
        }

	}

    IEnumerator Win(string teamName)
    {
        Debug.Log("Team " + teamName + " is the winner!");
        yield return new WaitForSeconds(2);
        Application.LoadLevel("Game_Over");
    }

    [PunRPC]
    void EndGame(int winningTeamID)
    {
        winFlag = false;
		PlayerPrefs.SetInt("TeamID",myTeamID);
        //txt.text = winningTeamID.ToString();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        Debug.Log(players.Length);

        foreach(GameObject player in players)
        {
            if (players[i].GetComponent<PhotonView>().isMine)
            {
                myTeamID = players[i].gameObject.GetComponent<TeamMember>().teamID;
            }
            i++;
        }

        gameOverCanvas.SetActive(true);

        if(myTeamID == winningTeamID) //you are in the winning team, display win screen
        {
            Debug.Log("win");
            if(myTeamID == 1) //member of team Trump, display winning screen
            {
                endgameTextList[1].SetActive(true);
            }
            if(myTeamID == 2) //member of team Wholo, display winning screen
            {
                endgameTextList[0].SetActive(true);
            }
        }
        else //you are in the losing team, display lose screen
        {
            Debug.Log("lose");
            if (myTeamID == 1) //member of team Trump, display losing screen
            {
                endgameTextList[2].SetActive(true);
            }
            if (myTeamID == 2) //member of team Wholo, display losing screen
            {
                endgameTextList[3].SetActive(true);
            }
        }

        StartCoroutine(Reboot(5));
            
        }

    IEnumerator Reboot(float waitingTime)
    {
        PhotonNetwork.LeaveRoom();
        yield return new WaitForSeconds(waitingTime);
        Application.LoadLevel(Application.loadedLevel);
    }
}
