using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Calculator:MonoBehaviour
{
    private static Calculator singleton=null;
    private static List<Vector3> map;
    private static double[] mapGSV;
    private static List<List<GameObject>> allTargets = new List<List<GameObject>>();
    //private List<Bot> botList = new List<Bot>();
    //private static bool[] moveBool = new bool[20];

    public static double wallConst = 4;
    public static double stepsize = 1;

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

    /*public void Start()
    {
        map = makeMap();
        mapGSV = new double[map.Count];
        for (int i = 0; i != mapGSV.Length; i++)
        {
            mapGSV[i] = GSV(map[i]);
        }
        moveBool[0] = true;
    }

    public void Update()
    {
        for (int i = 0; i != moveBool.Length - 1; i++)
        {
            if (moveBool[i])
            {
                foreach (Bot bot in botList)
                {
                    bot.setMoveBool(i, true);
                }
                StartCoroutine(secondMoveDelay((1 / moveBool.Length), i));
            }
        }
        if (moveBool[moveBool.Length - 1])
        {
            foreach (Bot bot in botList)
            {
                bot.setMoveBool(moveBool.Length - 1, true);
            }
            StartCoroutine(secondMoveDelay((1 / moveBool.Length), moveBool.Length - 1));
        }
    }

    IEnumerator secondMoveDelay(float time, int i)
    {
        if (i == moveBool.Length - 1)
        {
            moveBool[i] = false;
            yield return new WaitForSeconds(time);
            moveBool[0] = true;
        }
        else
        {
            moveBool[i] = false;
            yield return new WaitForSeconds(time);
            moveBool[i + 1] = true;
        }
    }*/

    public int Teaminator(String tag)
    {
        if(tag=="PlayerA")
        {
            return 1;
        }
        else if(tag=="PlayerB")
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public void addTarget(int botIndex, GameObject player)
    {
        allTargets[botIndex].Add(player);
    }

    public void deleteTarget(int botIndex, GameObject player)
    {
        allTargets[botIndex].Remove(player);
    }

    public List<GameObject> getTargets(int botIndex)
    {
        return allTargets[botIndex];
    }

    public int getAllTargets()
    {
        return allTargets.Count;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public int addBot()
    {
        List<GameObject> targets = new List<GameObject>();
        allTargets.Add(targets);
        return allTargets.Count-1;
    }

    /*public int moveBoolCount()
    {
        return moveBool.Length;
    }*/
}
