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
    public GameObject GSVTracker;
    public GameObject LSVTracker;
    public GameObject SVTracker;
    public Bot bot;

    private static double cGSV = 0.70;
    private static double cLSV = 2;

    public void Awake()
    {
        calculator=Calculator.getCalculator();
        map = calculator.getMap();
        foreach(Vector3 point in map)
        {
            //Instantiate(GSVTracker, new Vector3(point.x, (float)calculator.GSV(point), point.z), Quaternion.identity);
            //Instantiate(LSVTracker, new Vector3(point.x, (float)(2*bot.LSV(point)), point.z), Quaternion.identity);
            Instantiate(SVTracker, new Vector3(point.x, (float)(cGSV*calculator.GSV(point) + cLSV*bot.LSV(point)+2), point.z), Quaternion.identity);
        }
    }
}
