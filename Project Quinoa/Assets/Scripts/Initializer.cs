using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Initializer : MonoBehaviour {
#if UNITY_EDITOR
    [MenuItem("Play/Play Game")]                //Add play game button to menu, loads correct scene
    public static void PlayGame()
    {
        EditorApplication.SaveScene(EditorApplication.currentScene);
        EditorApplication.OpenScene("Assets/_Scenes/init.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    
    
    // Use this for initialization
    void Start () {
        Application.LoadLevel("MainMenu");
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("m"))
        {
            Debug.Log(AudioListener.volume);
            AudioListener.volume = Mathf.Abs(AudioListener.volume-1);
        }
	}
}
