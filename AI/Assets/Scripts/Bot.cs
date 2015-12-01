using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    private bool playerInSight;
    private bool dead;
    private Rigidbody rb;
    private int team;
    private List<Vector3> points;
    private Calculator calculator;
    private SphereCollider col;
    private List<Vector3> map;
    //https://unity3d.com/learn/tutorials/projects/stealth/enemy-sight

    public double sightConstant = 10;
    public double playerConstant = 4;
    public double ammoConstant = 1;
    public double healthConstant = 1;
    private static double cGSV = 0.70;
    private static double cLSV = 2;

    private double cPLSV = 1.5;
    private double cHLSV = 1;
    private double cALSV = 1;
    private float fieldofViewAngle = 190f;
    public float rotateSpeed;
    private float sensitivity = 1;
    private float maxHealth = 100;
    private float maxAmmo = 20;
    public int health = 10;
    public int ammo= 1;
    public Rigidbody prefabBullet;
    private float bulletSpeed=100;
    private float fireRate = 0.2f;
    private bool shootFlag;

    private List<GameObject> targets = new List<GameObject>();
    private GameObject minTarget;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        col.radius= (float)sightConstant;
        dead = false;
        //health = 100;
        //ammo = 0;
        team = -1;
        calculator = Calculator.getCalculator();
        map = calculator.getMap();
        shootFlag = false;
        playerInSight = false;
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
            HLSV = HLSV+maxHealth*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position-new Vector3(0,obj.transform.lossyScale.y/2,0))/healthConstant, 2)) / (health + 1);
        }

        foreach (var obj in ammos)
        {
            ALSV = ALSV+5*maxAmmo*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position-new Vector3(0,obj.transform.lossyScale.y/2,0))/ammoConstant, 2)) / (ammo + 1);
        }

        foreach (var obj in players)
        {
            /*if (((-obj.getTeam() * team + 1) / 2) = 0)
            {
                playerLengthConstant = playerLengthConstant/5;
            }*/
            PLSV = PLSV-(1-Math.Pow(1.9*Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z))/playerConstant,2))*Math.Exp(-Math.Pow(Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) /playerConstant,2));
        }
        foreach (var obj in bots)
        {
            /*if (((-obj.getTeam() * team + 1) / 2) = 0)
           {
               playerLengthConstant = playerLengthConstant/5;
           }*/
            if (Vector3.Distance(obj.transform.position,this.transform.position)!=0)
            {
                PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / playerConstant, 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / playerConstant, 2));
            }
        }

        return cPLSV * PLSV + cHLSV * HLSV + cALSV * ALSV;
    }

    public double SV(Vector3 point)
    {
        return cLSV * this.LSV(point) + cGSV * calculator.getGSV(point);
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

        map = calculator.getMap();
        Vector3 bestPoint = new Vector3(0.0f, 0.0f, 0.0f);
        double index = 0;
        for (int i=0;i!=map.Count;i++)
        {
            if (Vector3.Distance(map[i], this.transform.position - new Vector3(0, transform.lossyScale.y, 0)) < sightConstant)
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

        if (targets.Count > 0)
        {
            minTarget = targets[0];
            foreach (GameObject obj in targets)
            {
                if (Vector3.Distance(this.transform.position, minTarget.transform.position) > Vector3.Distance(this.transform.position, obj.transform.position))
                {
                    minTarget = obj;
                }
            }
            Shoot(minTarget);
        }
    }

    public void MoveBot(Vector3 point)
    {
        transform.Translate((new Vector3(point.x,0.0f,point.z) - new Vector3(transform.position.x,0.0f,transform.position.z))*Time.deltaTime*sensitivity);

        Vector3 aVector = Vector3.Cross(new Vector3(point.x - transform.position.x, 0, point.z - transform.position.z), new Vector3(transform.forward.x, 0, transform.forward.z));
        float a = -aVector.y / Math.Abs(aVector.y);
        float angle = Vector2.Angle(new Vector2(point.x - transform.position.x, point.z - transform.position.z), new Vector2(transform.forward.x, transform.forward.z));
        if (Math.Abs(a) > 1 && playerInSight == false)
        {
            transform.Rotate(new Vector3(0, angle, 0) * Time.deltaTime * a);
        }
    }

    public void Shoot(GameObject target)
    {
        //if (((-other.gameObject.getTeam() * team + 1) / 2 == 1))
        //{
        Vector3 aVector = Vector3.Cross(new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z), new Vector3(transform.forward.x, 0, transform.forward.z));
        float a = -aVector.y / Math.Abs(aVector.y);
        float targetAngle = Vector2.Angle(new Vector2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z), new Vector2(transform.forward.x, transform.forward.z));
        if (Math.Abs(a) > 0.1)
        {
            transform.Rotate(new Vector3(0, targetAngle, 0) * Time.deltaTime * a * rotateSpeed);
        }
        if (shootFlag)
        {
            Rigidbody clone = Instantiate(prefabBullet, new Vector3(rb.position.x, rb.position.y, rb.position.z + 2), Quaternion.identity) as Rigidbody;
            clone.velocity = this.transform.forward * bulletSpeed;
            StartCoroutine(bullet());
        }
        //}
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
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
                        if (shootFlag == false && playerInSight == false)
                        {
                            playerInSight = true;
                            shootFlag = true;
                        }
                        if (targets.IndexOf(hit.transform.gameObject) == -1 && hit.transform.gameObject != this.gameObject)
                        {
                            targets.Add(hit.transform.gameObject);
                        }
                    }
                }
            }
            //}
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
            if (targets.IndexOf(other.gameObject) != -1)
            {
                targets.Remove(other.gameObject);
            }
        }
    }

    IEnumerator bullet()
    {
        shootFlag = false;
        yield return new WaitForSeconds(fireRate);
        if (playerInSight)
        {
            shootFlag = true;
        }
    }

    public int getTeam()
    {
        return team;
    }

}