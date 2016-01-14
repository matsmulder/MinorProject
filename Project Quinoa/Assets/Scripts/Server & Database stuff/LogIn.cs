using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LogIn : MonoBehaviour {
	public DataController dc;

	public Text username, password;

	public GameObject WrongInput, NewAccount;

	public void tryLogIn(){
		NewAccount.SetActive (false);
		bool valid = dc.checkCredentials (username.text, password.text);

		username.text = "";
		password.text = "";

		if (valid) {
			Debug.Log ("log in credentials are correct");
			// load Network Screen
		} else {
			Debug.Log ("Log in is not correct");
			WrongInput.SetActive(true);
		}
	}

	public void addUser(){
		WrongInput.SetActive (false);

		dc.makeAccount (username.text, password.text);
		Debug.Log ("Accout has been added to the database");

		NewAccount.SetActive (true);
	}
}
