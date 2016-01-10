using UnityEngine;
using System.Collections;

public class NoScope : MonoBehaviour {

    public int zoomFOV;
    public int normalFOV;

	Texture crosshairTexture;
	Rect crosshairRect;
	private bool isTrigger = true;

	// Use this for initialization
	void Start () {
		float crosshairSize = Screen.width * 0.1f;
		crosshairTexture = Resources.Load ("Textures/crosshair") as Texture;
		crosshairRect = new Rect (Screen.width / 2 - crosshairSize / 2, Screen.height / 2 - crosshairSize / 2,
		                          crosshairSize, crosshairSize); 
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire2"))
        {
            Camera.main.fieldOfView = zoomFOV;
			isTrigger = false;
        }
        if(Input.GetButtonUp("Fire2"))
        {
            Camera.main.fieldOfView = normalFOV;
			isTrigger = true;
        }
	}

	void onGui(){
		if (isTrigger) {
			GUI.DrawTexture(crosshairRect,crosshairTexture);
		}
	}
}
