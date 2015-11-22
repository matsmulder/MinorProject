using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class Crosshair : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Image>().enabled = true; // enable crosshair on initialization
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
