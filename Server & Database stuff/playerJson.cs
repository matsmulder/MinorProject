using UnityEngine;
using System.Collections;

public class playerStat{
	public string name;
	public int kills;
	public int deaths;
	public int won;
	public int lost;

	public playerStat(string name, int kills, int deaths, int won, int lost){
		this.name = name;
		this.kills = kills;
		this.deaths = deaths;
		this.won = won;
		this.lost = lost;
	}

	public string toJson(){
		"{\"Name\":\"" + name + "\"," +
		"\"Kills\":\"" + kills + "\"," +
		"\"Deaths\":\"" + deaths + "\"," +
		"\"Won\":\""+won+"\"," +
		"\"Lost\":\""+lost+"\"}";
	}
}
