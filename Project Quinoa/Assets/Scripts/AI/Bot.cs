using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    //Bot State variables
    private Rigidbody rb;
    private Calculator calculator;
    private SphereCollider col;
    private List<Vector3> points;
    private float maxHealth = 100;
    private float maxAmmo = 20;
    public int health;
    public int ammo;
    public int team;
    private int index;

    //Bot Moving variables
    private float fieldofViewAngle = 190f;
    public float sensitivity;// = 0.75f;
    private bool[] moveBool;// = new bool[25];
    private Vector3[] bestPoints;
    private float[] bestPointsSV;
    private Vector3 bestPoint;

    //Bot Shooting variables
    private float rotateToTargetSpeed = 10;
    private float bulletSpeed = 50;
    private float fireRate = 1;//0.5f;
    private bool playerInSight;
    public Rigidbody prefabBullet;
    private bool shootFlag;
    private List<GameObject> targets;// = new List<GameObject>();
    private GameObject minTarget;

    //State Value Equation constants
    public double sightConstant = 15;
    private double playerConstant = 8;
    private double ammoConstant = 1;
    private double healthConstant = 1;
    private static double cGSV = 0.15;
    private static double cLSV = 2;
    private double cPLSV = 1.5;
    private double cHLSV = 1;
    private double cALSV = 1;
    private String playerA = "playerA";
    private String playerB = "playerB";

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        calculator = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
        index = calculator.addBot(this);
        targets = calculator.getTargets(index);
        col.radius= (float)sightConstant;
        //team = calculator.Teaminator(this.gameObject.tag);
        health = 100;
        //ammo = 0;
        shootFlag = false;
        playerInSight = false;
        moveBool = new bool[calculator.getMasterBoolCount()];
        bestPoint = transform.position;
        bestPoints = new Vector3[moveBool.Length];
        bestPointsSV = new float[moveBool.Length];
    }

    public double LSV(Vector3 point)
    {
        double PLSV = 0;
        double HLSV = 0;
        double ALSV = 0;
        GameObject[] teamMates; 
        GameObject[] opponents;

        if (this.team == 2)
        {
            teamMates = GameObject.FindGameObjectsWithTag(playerB);
            opponents= GameObject.FindGameObjectsWithTag(playerA);
        }
        else
        {
            teamMates = GameObject.FindGameObjectsWithTag(playerA);
            opponents = GameObject.FindGameObjectsWithTag(playerB);
        }

        GameObject[] healths = GameObject.FindGameObjectsWithTag("health");
        GameObject[] ammos = GameObject.FindGameObjectsWithTag("ammo");

        foreach (var obj in healths)
        {
            //HLSV = HLSV+maxHealth*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position-new Vector3(0,obj.transform.lossyScale.y/2,0))/healthConstant, 2)) / (health + 1);
            HLSV = HLSV + ((maxHealth - health) / (health + 1)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / healthConstant, 2)) / (health + 1);
        }

        foreach (var obj in ammos)
        {
            //ALSV = ALSV+5*maxAmmo*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position-new Vector3(0,obj.transform.lossyScale.y/2,0))/ammoConstant, 2)) / (ammo + 1);
            ALSV = ALSV + 5 * ((maxAmmo - ammo) / (ammo + 1)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / ammoConstant, 2)) / (ammo + 1);
        }

        foreach (var obj in teamMates)
        {
            if (Vector3.Distance(obj.transform.position, this.transform.position) != 0)
            {
                PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / (playerConstant/2), 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / (playerConstant/2), 2));
            }
        }
        foreach (var obj in opponents)
        {
            PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / playerConstant, 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, new Vector3(obj.transform.position.x, 0, obj.transform.position.z)) / playerConstant, 2));
        }

        return cPLSV * PLSV + cHLSV * HLSV + cALSV * ALSV;
    }

    public double SV(Vector3 point)
    {
        return cLSV * this.LSV(point) + cGSV * calculator.getGSV(point);
    }

    public void FixedUpdate()
    {
        Debug.Log("Updating");
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

        /*for(int i = 0;i!=moveBool.Length;i++)
        {
            if(moveBool[i])
            {
                Debug.Log(moveBool[i]);
            }
        }*/

        if (health > 0)
        {
            for (int i = 0; i != moveBool.Length - 1; i++)
            {
                if (moveBool[i])
                {
                    CalculateBestPoint(i);
                    StartCoroutine(Clock(i));
                }
            }
            if(moveBool[moveBool.Length-1])
            {
                CalculateBestPoint(moveBool.Length - 1);
                float pointer = Mathf.Max(bestPointsSV);
                bestPoint = bestPoints[Array.IndexOf(bestPointsSV,pointer)];
                StartCoroutine(Clock(moveBool.Length - 1));
            }
           
            MoveBot(bestPoint);

            if (ammo > 0 && shootFlag)
            {
                targets = calculator.getTargets(index);
                if (targets.Count > 0)
                {
                    minTarget = targets[0];
                    foreach (GameObject obj in targets)
                    {
                        if (obj!=null && Vector3.Distance(this.transform.position, minTarget.transform.position) > Vector3.Distance(this.transform.position, obj.transform.position))
                        {
                            minTarget = obj;
                        }
                    }
                    Shoot(minTarget);
                }
            }
        }
        else
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    calculator.deleteTarget(index, this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
        
        if(transform.position.y<-42)
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    calculator.deleteTarget(index, this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
    }

    public void CalculateBestPoint(int j)
    {
        bestPoints[j] = new Vector3(0.0f, 0.0f, 0.0f);
        bestPointsSV[j] = 0;

        for (int i = Mathf.RoundToInt((calculator.getMap().Count-1)*j/moveBool.Length); i != Mathf.RoundToInt((calculator.getMap().Count - 1)*(j+1)/ moveBool.Length); i++)
        {
            if (Vector3.Distance(calculator.getMap()[i], this.transform.position - new Vector3(0, transform.lossyScale.y, 0)) < sightConstant)
            {
                //if (Vector3.Angle(this.transform.forward,calculator.getMap()[i]-(transform.position-new Vector3(0,transform.lossyScale.y,0))) < fieldofViewAngle / 2)
                //{
                double tempSV = SV(calculator.getMap()[i]);
                if (bestPointsSV[j] < tempSV)
                {
                    bestPointsSV[j] = (float) tempSV;
                    bestPoints[j] = calculator.getMap()[i];
                }
                //}
            }
        }
    }

    public void MoveBot(Vector3 point)
    {
        transform.Translate((new Vector3(point.x,-31.33946f,point.z) - new Vector3(transform.position.x,-31.33946f,transform.position.z))*Time.deltaTime*sensitivity);

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
        {
            Vector3 aVector = Vector3.Cross(new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z), new Vector3(transform.forward.x, 0, transform.forward.z));
            float a = -aVector.y / Math.Abs(aVector.y);
            float targetAngle = Vector2.Angle(new Vector2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z), new Vector2(transform.forward.x, transform.forward.z));
            if (Math.Abs(a) > 0.1)
            {
                transform.Rotate(new Vector3(0, targetAngle, 0) * Time.deltaTime * a * rotateToTargetSpeed);
            }
            //if (shootFlag)
            //{
                Rigidbody clone = Instantiate(prefabBullet, new Vector3(rb.position.x, rb.position.y, rb.position.z) + this.transform.forward.normalized, Quaternion.identity) as Rigidbody;
                clone.velocity = new Vector3(target.transform.position.x - this.transform.position.x, 1, target.transform.position.z - this.transform.position.z) * bulletSpeed;
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(rb.position.x, rb.position.y, rb.position.z) + this.transform.forward.normalized, clone.velocity.normalized, out hit, col.radius))
                {
                    if ((team == 2 && hit.collider.tag == playerA)||(team == 1 && hit.collider.tag == playerB))
                    {
                        //Health Reduction Here
                    }
                }
                ammo = ammo - 1;
                StartCoroutine(bullet());
            //}
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if ((team == 2 && other.gameObject.CompareTag(playerA) && Vector3.Distance(other.gameObject.transform.position,this.transform.position)<sightConstant)||(team == 1 && other.gameObject.CompareTag(playerB) && Vector3.Distance(other.gameObject.transform.position, this.transform.position) < sightConstant))
        {
            Vector3 direction = other.transform.position - this.transform.position;
            float angle = Vector3.Angle(direction, this.transform.forward);
            if (angle < fieldofViewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position + this.transform.up, direction.normalized, out hit, col.radius))
                {
                    if ((team == 2 && hit.transform.gameObject.CompareTag(playerA)) || (team == 1 && hit.transform.gameObject.CompareTag(playerB)))
                    {
                        if (shootFlag == false && playerInSight == false)
                        {
                            playerInSight = true;
                            shootFlag = true;
                        }
                        if (targets.IndexOf(hit.transform.gameObject) == -1 && hit.transform.gameObject != this.gameObject)
                        {
                            //targets.Add(hit.transform.gameObject);
                            calculator.addTarget(index,hit.transform.gameObject);
                        }
                    }
                }
            }
        }

        if (other.gameObject.CompareTag("ammo") && Vector3.Distance(this.transform.position,other.transform.position) <= 2)
        {
            ammo = 20;
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("health") && Vector3.Distance(this.transform.position, other.transform.position) <= 2)
        {
            if (health <= 50)
            {
                health = health + 50;
            }
            else
            {
                health = 100;
            }
            Destroy(other.gameObject);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if ((team == 2 && other.gameObject.CompareTag(playerA))||(team == 1 && other.gameObject.CompareTag(playerB)))
        {
            if (targets.IndexOf(other.gameObject) != -1)
            {
                //targets.Remove(other.gameObject);
                calculator.deleteTarget(index,other.gameObject);
            }

            if (targets.Count==0)
            {
                playerInSight = false;
            }
        }
    }

    IEnumerator Clock(int i)
    {
        moveBool[i] = false;
        yield return new WaitForSeconds(1/moveBool.Length);
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

    public void setMoveBool(int i)
    {
        if(i==moveBool.Length-1)
        {
            moveBool[i] = false;
            moveBool[0] = true;
            //Debug.Log(moveBool[0]);
        }
        else
        {
            moveBool[i] = false;
            moveBool[i + 1] = true;
            //Debug.Log(moveBool[i]);
        }
    }

    public void setHealth(int h)
    {
        if (health>h)
        {
            health = health - h;
        }
        else
        {
            health = 0;
        }
    }

    public int getTeam()
    {
        return team;
    }
}