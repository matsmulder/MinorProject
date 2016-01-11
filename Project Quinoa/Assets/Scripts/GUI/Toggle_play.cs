using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toggle_play : MonoBehaviour {
	public GameObject panel_normalplay;
	public GameObject panel_networkplay;
	public GameObject network_button;
	public GameObject normalmode_button;
	private bool isShowing = false;

	public GameObject panel_joinormake;
	public GameObject panel_joingame;
	public GameObject panel_joininputfield;
	public GameObject panel_createinputfield;

    int currentScreen = 0;

    GameObject[] panels;
    Dictionary<int, GameObject[]> activePanels;

    void Start()
    {
        panels = new GameObject[] {
            panel_joinormake,
            panel_joininputfield,
            panel_joingame,
            panel_createinputfield
        };

        activePanels = new Dictionary<int, GameObject[]>
        {
            {0, new GameObject[] { panel_joinormake } },
            {1, new GameObject[] { panel_joininputfield } },
            {2, new GameObject[] { panel_createinputfield} },
            {3, new GameObject[] { panel_joingame} }
        };
        selectScreen(0);
    }

    public void ChangePlaymode()
    {
        if (isShowing == true)
        {
            resetnetworkgame();
            panel_networkplay.SetActive(true);
            panel_normalplay.SetActive(false);
            network_button.SetActive(false);
            normalmode_button.SetActive(true);
            isShowing = false;
        }
        else
        {
            panel_networkplay.SetActive(false);
            panel_normalplay.SetActive(true);
            network_button.SetActive(true);
            normalmode_button.SetActive(false);
            isShowing = true;
        }
    }

    public void previousMenu(){
        if (currentScreen == 1 || currentScreen == 2)
        {
            selectScreen(0);
        }
        else
        {
            selectScreen(currentScreen - 1);
        }
	}

    public void selectScreen(int screen)
    {
        //Debug.Log("Current screen: " + currentScreen);
        //Debug.Log("Screen: " + screen);
        /**
        0: main screen (with join game/creategame buttons
        1: joingame_input
        2: creategame_input
        3: joingame
        -1: Main menu
        */
        if (screen == -1)
        {
            Application.LoadLevel("MainMenu");
        }
        else
        {
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            }
            foreach (GameObject panel in activePanels[screen])
            {
                //Debug.Log(panel);
                panel.SetActive(true);
                //Debug.Log(panel.activeInHierarchy);
                //Debug.Log(panel.activeSelf);
            }
            //if (screen == 1)
            //{

            //}
        }
        //Debug.Log("joininput: "+panel_joininputfield.activeInHierarchy);
        currentScreen = screen;
    }

    public void joingame_input(){
        Debug.LogWarning("Using joingame_input instead of selectScreen!");
        panel_joinormake.SetActive (false);
		panel_joininputfield.SetActive (true);
	}
	
	public void creategame_input(){
        Debug.LogWarning("Using creategame_input instead of selectScreen!");
        panel_joinormake.SetActive (false);
		panel_createinputfield.SetActive (true);
	}
	
	public void joingame(){
        Debug.LogWarning("Using joingame instead of selectScreen!");
		panel_joininputfield.SetActive (false);
		panel_createinputfield.SetActive (false);
		panel_joingame.SetActive (true);
	}

	void resetnetworkgame(){
        Debug.LogWarning("Using resetnetworkgame instead of selectScreen!");
		panel_joinormake.SetActive (true);
		panel_joingame.SetActive (false);
		panel_joininputfield.SetActive (false);
		panel_createinputfield.SetActive (false);
	}

}