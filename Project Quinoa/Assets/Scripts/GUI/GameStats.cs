using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStats : MonoBehaviour {
	public Text playerName;
	public DataController dc = new DataController();
	public playerJson stats;

	public Text won;
	public Text lost;
	public Text deaths;
	public Text kills;
	public Text kdRatio;

	public void toMainMenu(){
		Application.LoadLevel ("MainMenu");
	}

	void Start () {
		// set the username to your username. 
		playerName.text = PlayerPrefs.GetString ("Name");

		stats = dc.getDBData (PlayerPrefs.GetString ("Name"));

		int tempWon = stats.getWon();
		Debug.Log ("tempWon= " + tempWon);
		int tempLost = stats.getLost();
		Debug.Log ("tempLost= " + tempLost);
		int tempDeaths = stats.getDeaths();
		Debug.Log ("tempDeaths= " + tempDeaths);
		int tempKills = stats.getKills();
		Debug.Log ("tempKills= " + tempKills);
		float tempKD;
		if (stats.getDeaths() == 0) {
			tempKD = Mathf.Infinity;
		} else if(stats.getKills() == 0){
			tempKD = 0;
		} else{
			tempKD = stats.getKills() / stats.getKills();
		}

		// update the values of the
		won.text = tempWon.ToString ();
		lost.text = tempLost.ToString ();
		deaths.text = tempDeaths.ToString ();
		kills.text = tempKills.ToString ();
		kdRatio.text = tempKD.ToString ();
	}
}
