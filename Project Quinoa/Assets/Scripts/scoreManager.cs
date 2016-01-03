using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scoreManager : MonoBehaviour {
    public static int numberOfSuperPickups;
    public static int numberOfFastPickups;
    private GameObject[] pickupSuperList, pickupFastList;
    private PhotonView pv;
    private Pickup[] pu, puTrump, puWholo;
    private int winningTeamID, myTeamID;
    public Text txt;

	// Use this for initialization
	void Start () {
       pickupFastList = GameObject.FindGameObjectsWithTag("fastfood");
        pickupSuperList = GameObject.FindGameObjectsWithTag("superfood");

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


        if (numberOfFastPickups == 0) //Team Wholo wins
        {
            //StartCoroutine(Win("Wholo"));
            //pu.EndGame();
            pv.RPC("EndGame", PhotonTargets.All, 2);
        }

        if (numberOfSuperPickups == 0) //Team Trump wins
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

        if(myTeamID == winningTeamID) //you are in the winning team, display win screen
        {
            //txt.text = "your team wins!";
			PlayerPrefs.SetInt("Won",1);
			//Application.LoadLevel("Game_Over");
        }
        else //you are in the losing team, display lose screen
        {
			//txt.text = "your team loses";
			PlayerPrefs.SetInt("Won",0);
			//Application.LoadLevel("Game_Over");
        }
    }
}
