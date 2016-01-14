using UnityEngine;
using System.Collections;

public class Pause_Menu : MonoBehaviour {
    public GameObject pauseCanvas;
    public bool paused;

	// Use this for initialization
	void Start () {
        paused = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("escape"))
        {
            //Debug.Log("Pressed escape");
            togglePause();
        }
    }

    public void togglePause()
    {

        if (paused)
        {
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //pauseCanvas.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //pauseCanvas.SetActive(true);
        }
        paused = !paused;
    }

    public void quitGame()
    {

    }

}
