using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {
	public Texture cursorImage;
	bool MouseChanged = false;

	public GameObject Button_hamburger;
	public GameObject Button_meat;
	public GameObject Button_blueberry;
	public GameObject Button_shake;
	public GameObject Button_coffee;
	public GameObject Button_boon;
	public GameObject Button_chicken;
	public GameObject Button_wing;

	public void PlayGame(){
		if (MouseChanged == true) {
			Button_hamburger.SetActive (false);
			Button_meat.SetActive (true);
		} else {
			Application.LoadLevel (3);
		}
	}

	public void StatsGame(){
		if (MouseChanged == true) {
			Button_blueberry.SetActive (false);
			Button_shake.SetActive (true);
		} else {
			Application.LoadLevel (4);
		}
	}

	public void OptionsGame(){
		if (MouseChanged == true) {
			Button_coffee.SetActive (false);
			Button_boon.SetActive (true);
		} else {
			Application.LoadLevel (2);
		}
	}

	public void QuitGame(){
		if (MouseChanged == true) {
			Button_chicken.SetActive (false);
			Button_wing.SetActive (true);
		} else {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
		}
	}


	public void ChangeMouse(){
		if (MouseChanged == false) {
			Cursor.visible = false;
			MouseChanged = true;
		} 
		else {
			Cursor.visible = true;
			MouseChanged = false;
		}
	}



	void OnGUI(){
		if (MouseChanged == true) {
			Vector3 mousePos = Input.mousePosition;
			Rect pos = new Rect (mousePos.x, Screen.height - mousePos.y, cursorImage.width, cursorImage.height);
			GUI.Label (pos, cursorImage);
		}
}
}
