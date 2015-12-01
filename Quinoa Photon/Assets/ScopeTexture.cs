using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class ScopeTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire2"))
        {
            GetComponent<Image>().enabled = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
