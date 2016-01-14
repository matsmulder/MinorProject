using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;

public class DataController{
	public static string playerName;
	public static int kills;
	public static int deaths;
	public static int won;
	public static int lost;

	public void sentDBData(){
		// retrieve data when calles from the playerCustomProperties
		playerName = (string)PhotonNetwork.player.name;
		kills = (int)PhotonNetwork.player.customProperties ["Kills"];
		deaths = (int)PhotonNetwork.player.customProperties ["Deaths"];
		won = (int)PhotonNetwork.player.customProperties ["Won"];
		lost = (int)PhotonNetwork.player.customProperties ["Lost"];

		// initialisation
		playerJson DBJSON = new playerJson(playerName, kills, deaths, won, lost);
		string json = DBJSON.toJson();

		// ini web
		var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/postStats");
		httpWebRequest.ContentType = "text/json";
		httpWebRequest.Method = "POST";

		// write away to the node.js 
		var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();

		resetData ();
	}

	public playerJson getDBData(string name){
		// ini web
		var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/getStats");
		httpWebRequest.ContentType = "text/json";
		httpWebRequest.Method = "POST";

		// write away to the node.js 
		var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
		string json = "{\"user\":" + name + "};";
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();

		// ini response from the server
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
		var streamReader = new StreamReader (httpResponse.GetResponseStream ());
		string userStats = streamReader.ReadToEnd();
		
		// ini JSON parser with the help of the JSONobject from downloaded from unity asset store.
		JSONObject json2 = new JSONObject(userStats);
		for(int i = 0; i < json2.list.Count; i++){
			string key = (string)json2.keys[i];

			switch(key){
			case "Username":
				Debug.Log("without .str: " + json2[i]);
				Debug.Log("string: " + json2[i].str);
				playerName = json2[i].str;
				break;
			case "AmountKills":
				kills = (int)json2[i].n;
				break;
			case "AmountDeaths":
				deaths = (int)json2[i].n;
				break;
			case "AmountWon":
				won = (int)json2[i].n;
				break;
			case "AmountLost":
				lost = (int)json2[i].n;
				break;
			}
		}

		// make playerJson and return
		playerJson playerStats = new playerJson (playerName, kills, deaths, won, lost);
		resetData ();
		return playerStats;
	}

	public bool checkCredentials(string name, string password){
		// ini web
		var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/checkLogin");
		httpWebRequest.ContentType = "text/json";
		httpWebRequest.Method = "POST";
			
		// write away to the node.js 
		var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
		string json = "{\"User\":" + name + ","
					+ "\"Password\":" + password + "};";
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();
			
		// ini res
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		var streamReader = new StreamReader (httpResponse.GetResponseStream());
		string stream = streamReader.ReadToEnd ();
		bool correctLogin = false;
		if (stream == "false") {
			correctLogin = false;

		} else if (stream == "true") {
			correctLogin = true;
		}
	
		return correctLogin;
	}

	public void makeAccount(string name, string password){
		// ini web
		var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/makeLogin");
		httpWebRequest.ContentType = "text/json";
		httpWebRequest.Method = "POST";
	                        
		// write away to the node.js 
		var streamWriter = new StreamWriter (httpWebRequest.GetRequestStream ());
		string json = "{\"User\":" + name + ","
					+ "\"Password\":" + password + "};";
		streamWriter.Write (json);
		streamWriter.Flush ();
		streamWriter.Close ();
	}

	public void resetData(){
		playerName = "";
		kills = 0;
		deaths = 0;
		won = 0;
		lost = 0;
	}
}
