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

    public static double wallConst = 4;
    public static double stepsizeX = 3.01662;
    public static double stepsizeZ = 2.87266;

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
        double height = 0;
        foreach (GameObject obj in walls)
        {
            Vector3 wallCenter = new Vector3(obj.transform.position.x, obj.transform.position.y - obj.transform.lossyScale.y / 2, obj.transform.position.z);
            Vector3 position;
            if (obj.GetComponent<Collider>().bounds.size.x < obj.GetComponent<Collider>().bounds.size.z)
                {
                    position = new Vector3(point.x, point.y, obj.transform.position.z);
                }
            else
                {
                    position = new Vector3(obj.transform.position.x, point.y, point.z);
                }

            if (obj.transform.position.y + obj.transform.lossyScale.y / 2<10)
            {
                height = obj.transform.position.y + obj.transform.lossyScale.y / 2;
            }
            else
            {
                height = 10;
            }

            GSV = GSV + 2*(height) * Math.Exp(-Math.Pow(Vector3.Distance(wallCenter, position) / wallConst, 2));
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
        //Debug.Log(botList.Count);
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

    public int getMasterBoolCount()
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

    /*public void printer(List<List<GameObject>> targets)
    {
        int counter = 0;
        foreach (List<GameObject> list in targets)
        {
            Debug.Log("List " + counter);
                counter++;
            foreach (GameObject go in list)
            {
                Debug.Log("GameObject");
            }
        }
    }*/
}
