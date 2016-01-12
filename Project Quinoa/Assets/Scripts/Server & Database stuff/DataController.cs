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

	public static void sentDBData(){
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
	}

	//public static playerJson getDBData(string name){
		// ini web
		//var httpWebRequest = (HttpWebRequest)WebRequest.Create ("https://drproject.twi.tudelft.nl:8082/getStats");
		//httpWebRequest.ContentType = "text/json";
		//httpWebRequest.Method = "POST";

		// write away to the node.js 
		//var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
		//string json = "{\"user\":" + name + "};";
		//streamWriter.Write(json);
		//streamWriter.Flush();
		//streamWriter.Close();

		// ini res
		//var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse ();
		//var streamReader = new StreamReader (httpResponse.GetResponseStream ());
		//string userStats = streamReader.ReadToEnd;
		
		// ini JSON parser
		//JavaScriptSerializer json_serializer = new JavaScriptSerializer();
		//playerJson playerStats = new playerJson;
			
		// handle the userStats data
		//return playerStats;
	//}

	public static bool checkCredentials(string name, string password){
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

	public static void makeAccount(string name, string password){
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
}
