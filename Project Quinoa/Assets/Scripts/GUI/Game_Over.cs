using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Game_Over : MonoBehaviour {
	float timeWait = 5.0f;
	public GameObject WON_QUINOA;
	public GameObject LOSE_QUINOA;
	public GameObject WON_FAST;
	public GameObject LOST_FAST;
	public GameObject LOST_TIME;
	public Text WaitText;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetInt ("TeamID") == 1) {
			if (PlayerPrefs.GetInt ("Won") == 0) {
				LOST_FAST.SetActive(true);
			}else if(PlayerPrefs.GetInt("Won") == 1){
				WON_FAST.SetActive(true);
			} else {
				LOST_TIME.SetActive(true);
			}
		} 
		else {
			if (PlayerPrefs.GetInt ("Won") == 0) {
				LOSE_QUINOA.SetActive(true);
			}else if(PlayerPrefs.GetInt("Won") == 1){
				WON_QUINOA.SetActive(true);
			} else {
				LOST_TIME.SetActive(true);
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		timeWait -= Time.deltaTime;
		if (timeWait <= 0) {
			// after 5 seconds the mainmenu will be called
			Application.LoadLevel ("StatsMenu");
		}

		// update waiting time UI
		WaitText.text = "Menu will be loaded in " + (int)timeWait + " Seconds";
	}
}
