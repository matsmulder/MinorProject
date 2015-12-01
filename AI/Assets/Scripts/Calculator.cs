using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

public class Calculator:MonoBehaviour
{
    private static Calculator singleton=null;
    private static List<Vector3> map;
    private static double[] mapGSV;

    public static double wallConst = 4;
    public static double stepsize = 0.5;
    public static double cGSV = 0.70;
    public static double cLSV = 2;

    private Calculator()
    {
        map = makeMap();
        mapGSV = new double[map.Count];
        for (int i = 0; i != mapGSV.Length; i++)
        {
            mapGSV[i] = GSV(map[i]);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static Calculator getCalculator()
    {
        if(singleton==null)
        {
            singleton = new Calculator();
        }
        return singleton;
    }

    public List<Vector3> getMap()
    {
        return map;
    }

    private List<Vector3> makeMap()
    {
        List<Vector3> points = new List<Vector3>();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach(GameObject obj in floors)
        {
            for (double i = -obj.GetComponent<Renderer>().bounds.size.x / 2; i <= obj.GetComponent<Renderer>().bounds.size.x / 2; i = i + stepsize) 
            {
                for (double j = -obj.GetComponent<Renderer>().bounds.size.z / 2; j <= obj.GetComponent<Renderer>().bounds.size.z / 2; j = j + stepsize) 
                {
                    points.Add(new Vector3((float)(i + obj.transform.position.x), obj.transform.position.y, (float)(j + obj.transform.position.z)));
                }
            }
        }
        return points;
    }

    public double GSV(Vector3 point)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        double GSV = 0;
        foreach (GameObject obj in walls)
        {
            Vector3 wallCenter = new Vector3(obj.transform.position.x, obj.transform.position.y - obj.transform.lossyScale.y / 2, obj.transform.position.z);
            Vector3 position;
            if (obj.GetComponent<Renderer>().bounds.size.x < obj.GetComponent<Renderer>().bounds.size.z)
                {
                    position = new Vector3(point.x, point.y, obj.transform.position.z);
                }
            else
                {
                    position = new Vector3(obj.transform.position.x, point.y, point.z);
                }
            GSV = GSV + 2*(obj.transform.position.y + obj.transform.lossyScale.y / 2) * Math.Exp(-Math.Pow(Vector3.Distance(wallCenter, position) / wallConst, 2));
        }
        return GSV;
    }

    public double getGSV(Vector3 point)
    {
        return mapGSV[map.IndexOf(point)];
    }

    public double getGSVconstant()
    {
        return cGSV;
    }

    public double getLSVconstant()
    {
        return cLSV;
    }
}
