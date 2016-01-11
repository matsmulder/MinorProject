using UnityEngine;
using System.Collections;

public class playerStat{
	private string name;
	private int kills;
	private int deaths;
	private int won;
	private int lost;

	public playerStat(string name, int kills, int deaths, int won, int lost){
		this.name = name;
		this.kills = kills;
		this.deaths = deaths;
		this.won = won;
		this.lost = lost;
	}

	public string toJson(){
		return ("{\"Name\":\"" + name + "\"," +
		"\"Kills\":\"" + kills + "\"," +
		"\"Deaths\":\"" + deaths + "\"," +
		"\"Won\":\""+won+"\"," +
		"\"Lost\":\""+lost+"\"}");
	}
}
