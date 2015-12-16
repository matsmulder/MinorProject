//using UnityEngine;
//using System.Collections;

//public class LevelManager : MonoBehaviour {

//    static int currentLevel;
//    static int levelsUnlocked;
//    static int maxLevel;
//    static menuStatus status;
//    public static List<pickupInfo> obtainedPickups;
//    public static List<Achievement> achievements;
//    public static bool paused;

//#if UNITY_EDITOR
//    [MenuItem("Edit/Reset Playerprefs")]
//    public static void DeletePlayerPrefs() { PlayerPrefs.DeleteAll(); }

//    [MenuItem("Play/Play game")]
//    public static void PlayGame()
//    {
//        EditorApplication.SaveScene(EditorApplication.currentScene);
//        EditorApplication.OpenScene("Assets/_Scenes/init.unity");
//        EditorApplication.isPlaying = true;
//    }
//#endif

//    public static menuStatus Status
//    {
//        get
//        {
//            return status;
//        }

//        set
//        {
//            status = value;
//        }
//    }

//    public static int LevelsUnlocked
//    {
//        get
//        {
//            return levelsUnlocked;
//        }

//        set
//        {
//            levelsUnlocked = value;
//            PlayerPrefs.SetInt("levelsUnlocked", value);
//        }
//    }

//    // Use this for initialization
//    void Start () {
	
//	}
	
//	// Update is called once per frame
//	void Update () {
	
//	}
//}
