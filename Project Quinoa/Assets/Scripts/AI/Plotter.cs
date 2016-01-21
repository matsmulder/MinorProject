using UnityEngine;
using System.Collections.Generic;

public class Plotter : MonoBehaviour
{
    private Calculator calculator;
    private static List<Vector3> map;
    public GameObject SVTracker;

    //private static double cpGSV = 30;
    //private static double cGSV = 0.6;
    //private static double cLSV = 4;

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
            //var tracker = (GameObject)Instantiate(SVTracker, new Vector3(point.x, (float)(cGSV*calculator.GSV(point)+55), point.z), Quaternion.identity);
            //var tracker = (GameObject)Instantiate(SVTracker, new Vector3(point.x, (float)(cLSV*bot.LSV(point)+55), point.z), Quaternion.identity);
            //var tracker = (GameObject)Instantiate(SVTracker, new Vector3(point.x,(float)(cpGSV * (1 - Vector3.Distance(point, new Vector3(64f, -30f, -43f)) / (123.458))+55),point.z), Quaternion.identity);
            var tracker = (GameObject)Instantiate(SVTracker, new Vector3(point.x, (float)(bot.SV(point)+55), point.z), Quaternion.identity);
            if (tracker.transform.position.y > 70)
            {
                tracker.GetComponent<Renderer>().material.color = new Color((tracker.transform.position.y-70)/7,1-(tracker.transform.position.y-70)/7,0);
            }
            else
            {
                tracker.GetComponent<Renderer>().material.color = new Color(0,1-(tracker.transform.position.y-70)/-7,(tracker.transform.position.y-70)/-7,0);
            }
        }
    }
}

