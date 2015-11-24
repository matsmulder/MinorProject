using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    private bool playerInSight;
    public int health;
    public int ammo;
    private bool dead;
    private Rigidbody rb;
    private int team;
    private List<Vector3> points;
    private Calculator calculator;
    private SphereCollider col;
    private List<Vector3> map;
    //https://unity3d.com/learn/tutorials/projects/stealth/enemy-sight

    public double sightConstant = 8;
    public double playerLengthConstant = 0.75;
    public double cPLSV = 4;
    public double cHLSV = 3;
    public double cALSV = 3;
    public float fieldofViewAngle = 110f;
    public float sensitivity = 4;
    public float maxHealth = 20;
    public float maxAmmo = 20;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        col.radius= (float)sightConstant;
        dead = false;
        health = 100;
        ammo = 20;
        team = -1;
        calculator = Calculator.getCalculator();
        transform.position = new Vector3(0.0f, (float)2, 0.0f);
    }

    public double LSV(Vector3 point)
    {
        double PLSV = 0;
        double HLSV = 0;
        double ALSV = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        GameObject[] healths = GameObject.FindGameObjectsWithTag("Health");
        GameObject[] ammos = GameObject.FindGameObjectsWithTag("Ammo");

        foreach (var obj in healths)
        {
            HLSV = HLSV+maxHealth*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position)/0.75, 2)) / (health + 1);
        }

        foreach (var obj in ammos)
        {
            ALSV = ALSV+maxAmmo*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position)/0.75, 2)) / (ammo + 1);
        }

        foreach (var obj in players)
        {
            PLSV = PLSV+Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position) / playerLengthConstant, 2));
        }
        foreach (var obj in bots)
        {
            PLSV = PLSV+Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position) / playerLengthConstant, 2));
        }
        //((-obj.getTeam() * team + 1) / 2) *
        return cPLSV * PLSV + cHLSV * HLSV + cALSV * ALSV;
    }

    public double SV(Vector3 point)
    {
        return calculator.getLSVconstant() * LSV(point) + calculator.getGSVconstant() * calculator.getGSV(point);
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

        map = calculator.getMap();
        Vector3 bestPoint = transform.position - new Vector3(0.0f, (float)transform.lossyScale.y, 0.0f);
        double index = 0;
        for (int i=0;i!=map.Count;i++)
        {
            if (Vector3.Distance(map[i], this.transform.position - new Vector3(0.0f, (float)transform.lossyScale.y, 0.0f)) < sightConstant)
            {
                if (Vector3.Angle(this.transform.forward,map[i]-(transform.position-new Vector3(0,transform.lossyScale.y,0))) < fieldofViewAngle / 2)
                {
                     double tempSV = SV(map[i]);
                     if (index < tempSV)
                     {
                         index = tempSV;
                         bestPoint = map[i];
                     }
                }
            }
        }
        MoveBot(bestPoint);
    }

    public void MoveBot(Vector3 point)
    {
        transform.Translate((new Vector3(point.x,0.0f,point.z) - new Vector3(transform.position.x,0.0f,transform.position.z))*sensitivity);

        Vector3 aVector = Vector3.Cross(new Vector3(point.x - transform.position.x, 0, point.z - transform.position.z), new Vector3(transform.forward.x, 0, transform.forward.z));
        float a = -aVector.y / Math.Abs(aVector.y);
        if (a < 1)
        {
            transform.Rotate(new Vector3(0.0f, Vector2.Angle(new Vector2(point.x - this.transform.position.x, point.z - this.transform.position.z), new Vector2(this.transform.forward.x, this.transform.forward.z)), 0.0f) * a);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
            //if ((-other.gameObject.getTeam()*team+1)/2 == 1)
            //{
            Vector3 direction = other.transform.position - this.transform.position;
            float angle = Vector3.Angle(direction, this.transform.forward);
            if (angle < fieldofViewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position + this.transform.up, direction.normalized, out hit, col.radius))
                {
                    if (hit.collider.tag == "Bot" || hit.collider.tag == "Player")
                    {
                        playerInSight = true;
                        //rb.transform.Translate(new Vector3(1,1,1));
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
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
        }
    }

    public int getTeam()
    {
        return team;
    }

}