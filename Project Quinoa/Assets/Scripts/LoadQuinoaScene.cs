using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class LoadQuinoaScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoadScene()
    {
        Application.LoadLevel("Quinoa");
    }

	public void LoadGameStats(){
		Application.LoadLevel ("StatsMenu");
	}

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
