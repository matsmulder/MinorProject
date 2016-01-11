using UnityEngine;
using System.Collections;
using System.IO;

public class DataController{
	public string playerName;
	public int kills;
	public int deaths;
	public int won;
	public int lost;

	public void sentDBData(){
		// initialisation
		playerJson DBJSON = new playerJson (playerName, kills, deaths, won, lost);
		String json = DBObject.toJson();

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

	public void getDBData(string name){
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
		var httpResponse = (HttpResponse)httpWebRequest.GetResponse ();
		var streamReader = new StreamReader (httpResponse.GetResponseStream ());
		string userStats = streamReader.ReadToEnd;

		// handle the userStats data

	}
}
