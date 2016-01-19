using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LogIn : MonoBehaviour {
	public DataController dc = new DataController();

	public Text username;
	public InputField password;

	public GameObject WrongInput, NewAccount, LogInCanvas ;
	
	public void tryLogIn(){
		NewAccount.SetActive (false);
	
		bool valid = dc.checkCredentials (username.text, password.text);

		if (valid) {
			Debug.Log ("log in credentials are correct");
			// go to the main main menu and set PhotonPlayer.Name
			// disable Log In screen.
		} else {
			Debug.Log ("Log in is not correct");
			WrongInput.SetActive(true);
		}
	}

	public void addUser(){

		Debug.Log (password.text);
		dc.makeAccount (username.text, password.text);
		Debug.Log ("Accout has been added to the database");
		NewAccount.SetActive (true);
	}
}
