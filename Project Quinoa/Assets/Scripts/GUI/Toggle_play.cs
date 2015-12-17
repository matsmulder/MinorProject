using UnityEngine;
using System.Collections;

public class Toggle_play : MonoBehaviour {
	public GameObject panel_normalplay;
	public GameObject panel_networkplay;
	public GameObject network_button;
	public GameObject normalmode_button;
	private bool isShowing = true;

	public GameObject panel_joinormake;
	public GameObject panel_joingame;
	public GameObject panel_creategame;
	public GameObject panel_joininputfield;
	public GameObject panel_createinputfield;

	public void ChangePlaymode(){
		if (isShowing == true) {
			resetnetworkgame();
			panel_networkplay.SetActive (true);
			panel_normalplay.SetActive (false);
			network_button.SetActive (false);
			normalmode_button.SetActive(true);
			isShowing = false;
		} else {
			panel_networkplay.SetActive (false);
			panel_normalplay.SetActive (true);
			network_button.SetActive(true);
			normalmode_button.SetActive(false);
			isShowing = true;
		}
	}

	public void ToMainMenu(){
		Application.LoadLevel ("mainMenu");
	}

    public void toQuinoaGame()
    {
        Application.LoadLevel("Quinoa");
    }

    public void joingame_input(){
		panel_joinormake.SetActive (false);
		panel_joininputfield.SetActive (true);
	}
	
	public void creategame_input(){
		panel_joinormake.SetActive (false);
		panel_createinputfield.SetActive (true);
	}
	
	public void creategame(){
		panel_createinputfield.SetActive (false);
		panel_creategame.SetActive (true);
	}
	
	public void joingame(){
		panel_joininputfield.SetActive (false);
		panel_joingame.SetActive (true);
	}

	void resetnetworkgame(){
		panel_joinormake.SetActive (true);
		panel_joingame.SetActive (false);
		panel_creategame.SetActive (false);
		panel_joininputfield.SetActive (false);
		panel_createinputfield.SetActive (false);
	}

}