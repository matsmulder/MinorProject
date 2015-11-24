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

    public static double lengthConst = 2;
    public static double stepsize=0.125;
    public static double cDGSV = 1;
    public static double cOGSV = 0.5;
    public static double cGSV = 2 / 3;
    public static double cLSV = 1 / 3;

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
            for (double i = -obj.transform.localScale.x / 2; i <= obj.transform.localScale.x / 2; i = i + stepsize) 
            {
                for (double j = -obj.transform.localScale.z / 2; j <= obj.transform.localScale.z / 2; j = j + stepsize) 
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
        double DGSV = 0;
        double OGSV = 0;
        double l = lengthConst;
        foreach (GameObject obj in walls)
        {
            Vector3 wallCenter = new Vector3(obj.transform.position.x, obj.transform.position.y - obj.transform.lossyScale.y / 2, obj.transform.position.z);
            Vector3 closestPoint = wallCenter;
            double index = Vector3.Distance(wallCenter, point);
            double min = Math.Min(obj.transform.lossyScale.x,obj.transform.lossyScale.z);
            double max = Math.Max(obj.transform.lossyScale.x, obj.transform.lossyScale.z);
            for (double t = -1; t <= 1; t = t + stepsize)
            {
                Vector3 wallPos = wallCenter + new Vector3((float)(((max - min) * Math.Cos(obj.transform.rotation.y * Math.PI / 180) + min) * t / 2), 0, (float)(((max - min) * Math.Sin(obj.transform.rotation.y * Math.PI / 180) + min) * t / 2));
                
                if (Vector3.Distance(wallPos, point) < Vector3.Distance(closestPoint, point))
                {
                    closestPoint = wallPos;
                    index = Vector3.Distance(wallPos, point);
                }
            }
            DGSV = +(obj.transform.position.y + obj.transform.lossyScale.y / 2) * Math.Exp(-Math.Pow(index / l,2));
            OGSV = +Math.Exp(-Math.Pow(index / (obj.transform.position.y + obj.transform.lossyScale.y / 2),2));
        }
        return cDGSV * DGSV + cOGSV * (-OGSV + 4);
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
