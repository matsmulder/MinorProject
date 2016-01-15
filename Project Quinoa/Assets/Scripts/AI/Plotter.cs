using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

public class Plotter : MonoBehaviour
{
    private Calculator calculator;
    private static List<Vector3> map;
    public GameObject SVTracker;
    //public Bot bot;

    //private static double cpGSV = 5;
    //private static double cGSV = 0.15;
    //private static double cLSV = 2;

    public void Start()
    {
        Debug.Log("I LIVE!");
        calculator = GetComponent<Calculator>();
        Bot bot = new Bot();
        if (calculator==null)
        {
            Debug.Log("calculator is null!");
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<Bot>().enabled==true)
            {
                bot = player.GetComponent<Bot>();
            }
        }
        if(bot==null)
        {
            Debug.Log("bot is null!");
        }
        map = calculator.getMap();
        if (map == null)
        {
            Debug.Log("map is null!");
        }
        foreach (Vector3 point in map)
        {
            //Instantiate(SVTracker, new Vector3(point.x, (float)(cGSV*calculator.GSV(point)), point.z), Quaternion.identity);
            //Instantiate(SVTracker, new Vector3(point.x, (float)(cLSV*bot.LSV(point)+30), point.z), Quaternion.identity);
            Instantiate(SVTracker, new Vector3(point.x, (float)(bot.SV(point)+30), point.z), Quaternion.identity);
        }
    }
}

