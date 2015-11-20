using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    private float fieldofViewAngle = 110f;
    private bool playerInSight;

    private int health;
    private int ammo;
    private bool dead;
    private Rigidbody rb;
    private int team;
    private List<Vector3> points;
    private Calculator calculator;
    private double l = 6;
    private SphereCollider col;
    //https://unity3d.com/learn/tutorials/projects/stealth/enemy-sight

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        dead = false;
        health = 100;
        calculator = Calculator.getCalculator(); 
    }

    public double LSV(Vector3 point)
    {
        double PLVS = 0;
        double HLVS = 0;
        double ALVS = 0;
        double l = 4;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        GameObject[] healths = GameObject.FindGameObjectsWithTag("Health");
        GameObject[] ammos = GameObject.FindGameObjectsWithTag("Ammo");

        foreach (var obj in healths)
        {
            HLVS = +Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position), 2)) / (health + 1);
        }

        foreach (var obj in ammos)
        {
            ALVS = +Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position), 2)) / (ammo + 1);
        }

        foreach (var obj in players)
        {
            PLVS = + Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position) / l, 2));
        }
        foreach (var obj in bots)
        {
            PLVS = + Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position) / l, 2));
        }
        //((-obj.getTeam() * team + 1) / 2) *
        double c1 = 0.33;
        double c2 = 0.33;
        double c3 = 0.33;
        return c1 * PLVS + c2 * HLVS + c3 * ALVS;
    }

    public double SV(Vector3 point)
    {
        return calculator.getGVSconstant() * calculator.getGSV(point) + calculator.getGVSconstant() * LSV(point);
    }

    public void Update()
    {
        transform.Translate(new Vector3(1*Time.deltaTime, 0, 0));
        points = new List<Vector3>();
        foreach(Vector3 obj in calculator.getMap())
        {
            if(Vector3.Distance(this.transform.position,obj)<l)
            {
                if (Vector3.Angle(this.transform.forward, obj) < fieldofViewAngle / 2)
                {
                    points.Add(obj);
                }
            }
        }

        double index = 0;
        Vector3 bestPoint = new Vector3(0,0,0);
        foreach(Vector3 point in points)
        {
            double tempSV = SV(point);
            if (index<tempSV)
            {
                index = tempSV;
                bestPoint = point;
            }
        }

        MoveBot(bestPoint);
    }

    public void MoveBot(Vector3 point)
    {
        transform.Rotate((point - new Vector3(transform.rotation.x,transform.rotation.y,transform.rotation.z)) * Time.deltaTime);
        transform.Translate((point - transform.position) * Time.deltaTime);
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Bot")|| other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
            //if ((-other.gameObject.getTeam()*team+1)/2 == 1)
            //{
                Vector3 direction = other.transform.position - transform.position;
                float angle = Vector3.Angle(direction, transform.forward);
                if (angle < fieldofViewAngle / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
                    {
                        if (hit.collider.tag=="Bot" || hit.collider.tag=="Player")
                        {
                            playerInSight = true;
                            rb.transform.Translate(new Vector3(1,1,1));
                        }
                    }
                }
            //}
        }
    }

    //public void Shoot()
    //{
    //    while(playerInSight)
    //    {
    //        transform.Rotate(Vector3.Angle( , transform.forward));
    //        Fire();
    //    }
    //}

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Bot")|| other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
        }
    }

    public int getTeam()
    {
        return team;
    }
}