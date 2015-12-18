using UnityEngine;
using System.Collections;

public class scoreManager : MonoBehaviour {
    public static int numberOfSuperPickups;
    public static int numberOfFastPickups;
    private GameObject[] pickupSuperList, pickupFastList;
    private PhotonView pv;
    private Pickup[] pu, puTrump, puWholo;

	// Use this for initialization
	void Start () {
        pickupFastList = GameObject.FindGameObjectsWithTag("fastfood");
        pickupSuperList = GameObject.FindGameObjectsWithTag("superfood");
        pv = GetComponent<PhotonView>();
        pu = FindObjectsOfType<Pickup>();
        int i = 0, j = 0, k = 0;
        foreach (Pickup child in pu)
        {
            if(child.tm.teamID == 1) //members of team Trump
            {
                puTrump[j] = pu[i];
                j++;
                Debug.Log("Trump");
            }
            if(child.tm.teamID == 2) //members of team Wholo
            {
                puWholo[k] = pu[i];
                k++;
                Debug.Log("wholo");
            }
            i++;
        }

        numberOfFastPickups = pickupFastList.Length;
        numberOfSuperPickups = pickupSuperList.Length;
	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log(numberOfFastPickups + " fastpickups");
        Debug.Log(numberOfSuperPickups + " superpickups");

        if (numberOfFastPickups == 0) //Team Wholo wins
        {
            //StartCoroutine(Win("Wholo"));
            //pu.EndGame();
            pv.RPC("EndGame", PhotonTargets.All);
        }

        if (numberOfSuperPickups == 0) //Team Trump wins
        {
            //StartCoroutine(Win("Trump"));
            //pu.EndGame();
            pv.RPC("EndGame", PhotonTargets.All);
        }

	}

    IEnumerator Win(string teamName)
    {
        Debug.Log("Team " + teamName + " is the winner!");
        yield return new WaitForSeconds(2);
        Application.LoadLevel(Application.loadedLevel);
    }

    [PunRPC]
    void EndGame()
    {
        
    }
}
