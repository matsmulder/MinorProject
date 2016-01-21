using UnityEngine;
using System.Collections;

public class Pause_Menu : MonoBehaviour {
    public GameObject pauseCanvas;
    public bool paused;
    private RandomMatchmaker rm;

	// Use this for initialization
	void Start () {
        paused = false;
        rm = GameObject.FindObjectOfType<RandomMatchmaker>();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(rm.stat);
        if (rm.stat == status.inGame || RandomMatchmaker.offlineMode)
        {
            if (Input.GetKeyDown("escape"))
            {
                togglePause();
            }
        }
        
    }

    public void togglePause()
    {
        if (paused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseCanvas.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseCanvas.SetActive(true);
        }
        paused = !paused;
    }

    public void quitGame()
    {
        PhotonNetwork.LeaveRoom();
        Application.LoadLevel("MainMenu");
    }

}
