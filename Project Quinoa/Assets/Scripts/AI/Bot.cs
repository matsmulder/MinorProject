using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    //Bot State variables
    private Calculator calculator;
    private SphereCollider col;
    private List<Vector3> points;
    private int team;
    private int index;
    private Rigidbody rb;

    //Bot Moving variables
    private float fieldofViewAngle = 190f;
    private float sensitivity = 0.5f;
    private bool[] moveBool;
    private Vector3[] bestPoints;
    private float[] bestPointsSV;
    private Vector3 bestPoint;

    //Bot Shooting variables
    private float rotateToTargetSpeed = 10;
    private bool playerInSight;
    private bool shootFlag;
    private List<GameObject> targets;// = new List<GameObject>();
    private GameObject minTarget;
    private playerShooting plsh;

    //State Value Equation constants
    private double sightConstant = 30;//70;
    private double playerConstant = 8;
    private static double cGSV = 0.6;
    private static double cLSV = 4;
    private double cPLSV = 1.5;
    private double cFLSV = 40;
    private Vector3 goal = new Vector3(13.5f,-30f,-7.5f);
    private List<GameObject> teamMates=new List<GameObject>();
    private List<GameObject> opponents=new List<GameObject>();

    public void Start()
    {
        plsh = GetComponent<playerShooting>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        calculator = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
        index = calculator.addBot(this);
        targets = calculator.getTargets(index);
        col.radius= (float)sightConstant;
        shootFlag = false;
        playerInSight = false;
        moveBool = new bool[calculator.getMasterBoolLength()];
        bestPoint = transform.position;
        bestPoints = new Vector3[moveBool.Length];
        bestPointsSV = new float[moveBool.Length];
        team = GetComponent<TeamMember>().teamID;
        teamFinder();
        if (team == 1)
        {
            goal = new Vector3(-37f, -30f, 28f);
        }
        else if(team == 2)
        {
            goal = new Vector3(64f, -30f, -43f);
        }
    }

    public double LSV(Vector3 point)
    {
        double PLSV = 0;
        double FLSV = 0;

        GameObject[] foods;
        if (team == 1)
        {
            foods = GameObject.FindGameObjectsWithTag("superfood");
        }
        else if(team == 2)
        {
            foods = GameObject.FindGameObjectsWithTag("fastfood");
        }
        else
        {
            foods = new GameObject[0];
        }
        foreach (GameObject food in foods)
        {
            FLSV = FLSV + (1 - Vector3.Distance(point, food.transform.position) / (123.458));
        }

        foreach (GameObject obj in teamMates)
        {
            if (Vector3.Distance(obj.transform.position, this.transform.position) != 0)
            {
                PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (playerConstant), 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (playerConstant), 2));
            }
        }
        foreach (GameObject obj in opponents)
        {
            PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (2*playerConstant), 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (2*playerConstant), 2));
        }

        return cPLSV * PLSV + cFLSV/cLSV * FLSV;
    }

    public double SV(Vector3 point)
    {
        return cLSV * this.LSV(point) + cGSV * calculator.getGSV(point);// + cpGSV * (1-Vector3.Distance(point,goal)/(123.458));
    }

    public void FixedUpdate()
    {
        teamFinder();
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));

        if (this.GetComponent<Health>().getHealthPoints() > 0)
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

            if (shootFlag)
            {
                targets = calculator.getTargets(index);

                if (targets.Count > 0)
                {
                    minTarget = targets[0];
                    foreach (GameObject obj in targets)
                    {
                        if (obj!=null && minTarget!=null && Vector3.Distance(this.transform.position, minTarget.transform.position) > Vector3.Distance(this.transform.position, obj.transform.position))
                        {
                            minTarget = obj;
                        }
                    }
                    Shoot(minTarget);
                }
            }
        }
        
        if(transform.position.y<-42)
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    calculator.deleteTarget(i, this.gameObject);
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
                double tempSV = SV(calculator.getMap()[i]);
                if (bestPointsSV[j] < tempSV)
                {
                    bestPointsSV[j] = (float) tempSV;
                    bestPoints[j] = calculator.getMap()[i];
                }
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
        if (target != null)
        {
            Vector3 aVector = Vector3.Cross(new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z), new Vector3(transform.forward.x, 0, transform.forward.z));
            float a = -aVector.y / Math.Abs(aVector.y);
            float targetAngle = Vector2.Angle(new Vector2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z), new Vector2(transform.forward.x, transform.forward.z));
            if (Math.Abs(a) > 0.1)
            {
                transform.Rotate(new Vector3(0, targetAngle, 0) * Time.deltaTime * a * rotateToTargetSpeed);
            }
            if (plsh.fireFlag)
            {
                plsh.Fire();
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (opponents.IndexOf(other.gameObject)!=-1 && Vector3.Distance(other.gameObject.transform.position,this.transform.position)<sightConstant)
        {
            Vector3 direction = other.transform.position - this.transform.position;
            float angle = Vector3.Angle(direction, this.transform.forward);
            if (angle < fieldofViewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position + this.transform.up, direction.normalized, out hit, col.radius))
                {
                    if (opponents.IndexOf(hit.transform.gameObject)!=-1)
                    {
                        if (shootFlag == false && playerInSight == false)
                        {
                            playerInSight = true;
                            shootFlag = true;
                        }
                        if (targets.IndexOf(hit.transform.gameObject) == -1 && hit.transform.gameObject != this.gameObject)
                        {
                            calculator.addTarget(index,hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (opponents.IndexOf(other.gameObject)!=-1)
        {
            if (targets.IndexOf(other.gameObject) != -1)
            {
                calculator.deleteTarget(index,other.gameObject);
            }

            if (targets.Count==0)
            {
                playerInSight = false;
            }
        }
    }

    public void teamFinder()
    {
        teamMates = new List<GameObject>();
        opponents = new List<GameObject>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (team == 2)
        {
            foreach (GameObject player in players)
            {
                if (player.GetComponent<TeamMember>().teamID == 1)
                {
                    opponents.Add(player);
                }
                else if (player.GetComponent<TeamMember>().teamID == 2)
                {
                    teamMates.Add(player);
                }
            }
        }
        else if (team == 1)
        {
            foreach (GameObject player in players)
            {
                if (player.GetComponent<TeamMember>().teamID == 1)
                {
                    teamMates.Add(player);
                }
                else if (player.GetComponent<TeamMember>().teamID == 2)
                {
                    opponents.Add(player);
                }
            }
        }
    }

    IEnumerator Clock(int i)
    {
        moveBool[i] = false;
        yield return new WaitForSeconds(1/moveBool.Length);
    }

    public void setMoveBool(int i)
    {
        if(i==moveBool.Length-1)
        {
            moveBool[i] = false;
            moveBool[0] = true;
        }
        else
        {
            moveBool[i] = false;
            moveBool[i + 1] = true;
        }
    }

    public int getTeam()
    {
        return team;
    }
}