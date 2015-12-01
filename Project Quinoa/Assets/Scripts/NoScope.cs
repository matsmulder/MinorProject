using UnityEngine;
using System.Collections;

public class NoScope : MonoBehaviour {

    public int zoomFOV;
    public int normalFOV;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire2"))
        {
            Camera.main.fieldOfView = zoomFOV;
        }
        if(Input.GetButtonUp("Fire2"))
        {
            Camera.main.fieldOfView = normalFOV;
        }
	}
}
