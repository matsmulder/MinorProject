using UnityEngine;
using System.Collections;

public class scoreManager : MonoBehaviour {
    public static int numberOfSuperPickups;
    public static int numberOfFastPickups;
    private GameObject[] pickupSuperList, pickupFastList;
	// Use this for initialization
	void Start () {
        pickupFastList = GameObject.FindGameObjectsWithTag("fastfood");
        pickupSuperList = GameObject.FindGameObjectsWithTag("superfood");

        numberOfFastPickups = pickupFastList.Length;
        numberOfSuperPickups = pickupSuperList.Length;
	}
	
	// Update is called once per frame
	void Update () {

        if (numberOfFastPickups == 0) //Team Wholo wins
        {
            StartCoroutine(Win("Wholo"));
        }

        if (numberOfSuperPickups == 0) //Team Trump wins
        {
            StartCoroutine(Win("Trump"));
        }

	}

    IEnumerator Win(string teamName)
    {
        Debug.Log("Team " + teamName + " is the winner!");
        yield return new WaitForSeconds(2);
        Application.LoadLevel(Application.loadedLevel);
    }
}
