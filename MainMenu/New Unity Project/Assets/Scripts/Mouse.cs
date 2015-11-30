using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {
	public Texture cursorImage;
	public static bool clicked = false;

	
	void Update () {

	
	}

	public void ChangeMouse(){
		if (clicked == false) {
			Cursor.visible = false;
			clicked = true; 
		} else {
			Cursor.visible = true;
			clicked = false;
		}
	}

	void OnGUI(){
		if (clicked == true) {
			Vector3 mousePos = Input.mousePosition;
			Rect pos = new Rect (mousePos.x, Screen.height - mousePos.y, cursorImage.width, cursorImage.height);
			GUI.Label (pos, cursorImage);
		}
	}

	public static bool get(){
		return clicked;
	}
}
	
