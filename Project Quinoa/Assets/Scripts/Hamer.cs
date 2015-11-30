using UnityEngine;
using System.Collections;

public class Hamer : MonoBehaviour {
	public Canvas Hamburger;
	public Canvas Meat;
	public Canvas Bes;
	public Canvas Shake;
	public Canvas Kip;
	public Canvas Wing;
	public Canvas Coffee;
	public Canvas Boon;

	//Gedrag play Button
	public void Hamer_Play(){
		if (Mouse.get () == true) {
			Hamburger.gameObject.SetActive (false);
			Meat.gameObject.SetActive (true);
		} else {
			Application.LoadLevel (0);
		}
	}
	
	//Gedrag Quit Button
	public void Hamer_Quit(){
		if (Mouse.get () == true) {
			Bes.gameObject.SetActive (false);
			Shake.gameObject.SetActive (true);
		} else {
			Application.Quit();
		}
	}

	//Gedrag Stats Button
	public void Hamer_Stats(){
		if (Mouse.get () == true) {
			Kip.gameObject.SetActive (false);
			Wing.gameObject.SetActive (true);
		} else {
			Application.LoadLevel(0);
		}
	}

	//Gedrag options Button
	public void Hamer_Options(){
		if (Mouse.get () == true) {
			Coffee.gameObject.SetActive (false);
			Boon.gameObject.SetActive (true);
		} else {
			Application.LoadLevel(0);
		}
	}
}
