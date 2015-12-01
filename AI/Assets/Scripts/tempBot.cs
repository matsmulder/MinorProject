using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class tempBot : MonoBehaviour
{
    private bool playerInSight;
    private Rigidbody rb;
    private SphereCollider col;

    private float fieldofViewAngle = 190f;
    public float rotateSpeed;
    public Rigidbody prefabBullet;
    private float bulletSpeed = 100;
    private float fireRate = 1.0f;
    private bool shootFlag;
    private List<GameObject> targets = new List<GameObject>();
    private GameObject minTarget;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        col.radius = 10;
        shootFlag = false;
        playerInSight = false;
    }

    public void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
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

    IEnumerator bullet()
    {
        shootFlag = false;
        yield return new WaitForSeconds(fireRate);
        if (playerInSight)
        {
            shootFlag = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
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
                                    if(targets.IndexOf(hit.transform.gameObject)==-1 && hit.transform.gameObject!=this.gameObject)
                                    {
                                        targets.Add(hit.transform.gameObject);
                                    }
                               }
                       }
               }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Bot") || other.gameObject.CompareTag("Player"))
        {
            playerInSight = false;
            if(targets.IndexOf(other.gameObject)!=-1)
            {
                targets.Remove(other.gameObject);
            }
        }
    }
}
 