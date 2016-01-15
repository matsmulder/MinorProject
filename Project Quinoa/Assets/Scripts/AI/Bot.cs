using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bot : MonoBehaviour{
    //Bot State variables
    private Calculator calculator;
    private SphereCollider col;
    private List<Vector3> points;
    private float maxHealth = 100;
    public int health;
    private int team;
    private int index;
    private Rigidbody rb;
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
    private playerShooting plsh;

    //State Value Equation constants
    public double sightConstant = 15;
    private double playerConstant = 8;
    private double healthConstant = 1;
    private static double cGSV = -0.30;
    private static double cLSV = 2;
    private double cPLSV = 1.5;
    private double cHLSV = 1;
    private double cpGSV = 5;
    private Vector3 goal = new Vector3(13.5f,-30f,-7.5f);
    private List<GameObject> teamMates=new List<GameObject>();
    private List<GameObject> opponents=new List<GameObject>();

    public void Start()
    {
        plsh = GetComponent<playerShooting>();
        //rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        calculator = GameObject.FindGameObjectWithTag("scripts").GetComponent<Calculator>();
        index = calculator.addBot(this);
        targets = calculator.getTargets(index);
        col.radius= (float)sightConstant;
        shootFlag = false;
        playerInSight = false;
        moveBool = new bool[calculator.getMasterBoolCount()];
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
        double HLSV = 0;

        GameObject[] healths = GameObject.FindGameObjectsWithTag("health");

        foreach (GameObject obj in healths)
        {
            //HLSV = HLSV+maxHealth*Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position-new Vector3(0,obj.transform.lossyScale.y/2,0))/healthConstant, 2)) / (health + 1);
            HLSV = HLSV + ((maxHealth - health) / (health + 1)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / healthConstant, 2)) / (health + 1);
        }

        Debug.Log(teamMates.Count);

        foreach (GameObject obj in teamMates)
        {
            if (Vector3.Distance(obj.transform.position, this.transform.position) != 0)
            {
                PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (playerConstant/2), 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / (playerConstant/2), 2));
            }
        }
        //Debug.Log(opponents.Count);
        foreach (GameObject obj in opponents)
        {
            PLSV = PLSV - (1 - Math.Pow(1.9 * Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / playerConstant, 2)) * Math.Exp(-Math.Pow(Vector3.Distance(point, obj.transform.position - new Vector3(0, obj.transform.lossyScale.y / 2, 0)) / playerConstant, 2));
        }

        return cPLSV * PLSV + cHLSV * HLSV;
    }

    public double SV(Vector3 point)
    {
        return cLSV * this.LSV(point) + cGSV * calculator.getGSV(point) + cpGSV * (1-Vector3.Distance(point,goal)/(123.458));
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
                        if (obj!=null && Vector3.Distance(this.transform.position, minTarget.transform.position) > Vector3.Distance(this.transform.position, obj.transform.position))
                        {
                            minTarget = obj;
                        }
                    }
                    //GetComponent<playerShooting>().Fire();
                    Shoot(minTarget);
                }
            }
        }
        /*else
        {
            for (int i = 0; i != calculator.getAllTargets(); i++)
            {
                if (calculator.getTargets(i).IndexOf(this.gameObject) != -1)
                {
                    //calculator.deleteTarget(index, this.gameObject);
                }
            }
            Destroy(this.gameObject);
        }*/
        
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
            //Rigidbody clone = Instantiate(prefabBullet, new Vector3(rb.position.x, rb.position.y, rb.position.z) + this.transform.forward.normalized, Quaternion.identity) as Rigidbody;
            //clone.velocity = new Vector3(target.transform.position.x - this.transform.position.x, 1, target.transform.position.z - this.transform.position.z) * bulletSpeed;
            Debug.Log("shoot!");
            if (plsh.fireFlag)
            {
                plsh.Fire();
            }
                //RaycastHit hit;
                //if (Physics.Raycast(new Vector3(rb.position.x, rb.position.y, rb.position.z) + this.transform.forward.normalized, (new Vector3(target.transform.position.x - this.transform.position.x, 1, target.transform.position.z - this.transform.position.z) * bulletSpeed).normalized, out hit, col.radius))
                //{
                //    if (opponents.IndexOf(hit.transform.gameObject)!=-1)
                //    {
                //        //Health Reduction Here
                //    }
                //}
                //StartCoroutine(bullet());
            //}
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
                            //targets.Add(hit.transform.gameObject);
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
                //targets.Remove(other.gameObject);
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