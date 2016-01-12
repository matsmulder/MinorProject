using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

public class DataController{
	public string playerName;
	public int kills;
	public int deaths;
	public int won;
	public int lost;

	public void sentDBData(){
		// initialisation
		playerJson DBJSON = new playerJson(playerName, kills, deaths, won, lost);
		string json = DBJSON.toJson();

		// ini web
		var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/postStats");
		httpWebRequest.ContentType = "text/json";
		httpWebRequest.Method = "POST";

		// write away to the node.js 
		var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream();
		streamWriter.Write(json);
		streamWriter.Flush();
		streamWriter.Close();
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

		// ini res
		var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
		var streamReader = new StreamReader (httpResponse.GetResponseStream ());
		string userStats = streamReader.ReadToEnd;
		
		// ini JSON parser
		JavaScriptSerializer json_serializer = new JavaScriptSerializer();
		playerJson playerStats = (playerJson)json_serializer.DeserializeObject(userStats);
			
		// handle the userStats data
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
		bool correctLogin = streamReader.ReadToEnd;

		return correctLogin;
	}
}
