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

    //private static double cpGSV = 5;
    private static double cGSV = 0.30;
    //private static double cLSV = 2;

    public void Start()
    {
        calculator = GetComponent<Calculator>();
        Bot bot = new Bot();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<Bot>().enabled==true)
            {
                bot = player.GetComponent<Bot>();
            }
        }
        map = calculator.getMap();
        foreach (Vector3 point in map)
        {
            //Instantiate(SVTracker, new Vector3(point.x, (float)(cGSV*calculator.GSV(point)+55), point.z), Quaternion.identity);
            //Instantiate(SVTracker, new Vector3(point.x, (float)(cLSV*bot.LSV(point)+55), point.z), Quaternion.identity);
            Instantiate(SVTracker, new Vector3(point.x, (float)(bot.SV(point)+55), point.z), Quaternion.identity);
        }
    }
}

