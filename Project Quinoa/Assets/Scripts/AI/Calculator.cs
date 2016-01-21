using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Calculator:MonoBehaviour
{
    private List<Vector3> map;
    private double[] mapGSV;
    private List<List<GameObject>> allTargets = new List<List<GameObject>>();

    private static int masterMoveBoolLength = 25;
    private List<Bot> botList = new List<Bot>();
    private bool[] masterMoveBool = new bool[masterMoveBoolLength];

    private static double wallConst = 1;
    private static double stepsizeX = 3.01662;
    private static double stepsizeZ = 2.87266;

    public Calculator()
    {    }

    public void Start()
    {
        map = makeMap();
        mapGSV = new double[map.Count];
        for (int i = 0; i != mapGSV.Length; i++)
        {
            mapGSV[i] = GSV(map[i]);
        }
        masterMoveBool[masterMoveBool.Length-1] = true;
    }

    public void Update()
    {
        for (int i = 0; i != masterMoveBool.Length; i++)
        {
            if (masterMoveBool[i])
            {
                StartCoroutine(masterClock(i));
            }
        }
    }

    public List<Vector3> getMap()
    {
        return map;
    }

    private List<Vector3> makeMap()
    {
        List<Vector3> points = new List<Vector3>();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("ground");
        foreach(GameObject obj in floors)
        {
            for (double i = -obj.GetComponent<Collider>().bounds.size.x / 2; i <= obj.GetComponent<Collider>().bounds.size.x / 2; i = i + stepsizeX) 
            {
                for (double j = -obj.GetComponent<Collider>().bounds.size.z / 2; j <= obj.GetComponent<Collider>().bounds.size.z / 2; j = j + stepsizeZ) 
                {
                    points.Add(new Vector3((float)(i + obj.transform.position.x), obj.transform.position.y, (float)(j + obj.transform.position.z)));
                }
            }
        }
        return points;
    }

    public double GSV(Vector3 point)
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        double GSV = 0;
        double height = 10;
        foreach (GameObject obj in walls)
        {
            Vector3 wallCenter = new Vector3(obj.transform.position.x, point.y, obj.transform.position.z);            
            Vector3 pointPrime = new Vector3((float)((point.x - wallCenter.x) * Math.Cos(-obj.transform.rotation.y * Mathf.Deg2Rad) - (point.z - wallCenter.z) * Math.Sin(-obj.transform.rotation.y * Mathf.Deg2Rad)), point.y-wallCenter.y, (float)((point.x - wallCenter.x) * Math.Sin(-obj.transform.rotation.y * Mathf.Deg2Rad) + (point.z - wallCenter.z) * Math.Cos(-obj.transform.rotation.y * Mathf.Deg2Rad)));
            if (obj.GetComponent<Collider>().bounds.size.y < 10)
            {
                height = obj.GetComponent<Collider>().bounds.size.y;
            }
            else
            {
                height = 10;
            }

            GSV = GSV - (height) * (1 - Math.Pow(1.9 * Math.Sqrt(Math.Pow(pointPrime.x / (obj.GetComponent<Collider>().bounds.size.x/2), 2) + Math.Pow(pointPrime.z / (obj.GetComponent<Collider>().bounds.size.z/2), 2)) / wallConst, 2)) * Math.Exp(-Math.Pow(Math.Sqrt(Math.Pow(pointPrime.x / (obj.GetComponent<Collider>().bounds.size.x/2), 2) + Math.Pow(pointPrime.z / (obj.GetComponent<Collider>().bounds.size.z/2), 2)) / wallConst, 2));
        }
        return GSV;
    }

    public double getGSV(Vector3 point)
    {
        return mapGSV[map.IndexOf(point)];
    }

    IEnumerator masterClock(int i)
    {
        masterMoveBool[i] = false;
        foreach (Bot bot in botList)
        {
            bot.setMoveBool(i);
        }

        yield return new WaitForSeconds(1/masterMoveBool.Length);

        if (i == masterMoveBool.Length - 1)
        {
            masterMoveBool[0] = true;
        }
        else
        {
            masterMoveBool[i + 1] = true;
        }
    }

    public int getMasterBoolLength()
    {
        return masterMoveBool.Length;
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
    public int addBot(Bot bot)
    {
        List<GameObject> targets = new List<GameObject>();
        allTargets.Add(targets);
        botList.Add(bot);
        return allTargets.Count-1;
    }
}
