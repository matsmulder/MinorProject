using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LogIn : MonoBehaviour {
	public DataController dc;

	public Text username, password;

	public void tryLogIn(){
		dc.checkCredentials (username.text, password.text);
	}

	public void addUser(){
		dc.makeAccount (username.text, password.text);
	}
}
