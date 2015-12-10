﻿using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class tempBot : MonoBehaviour
{
    private bool playerInSight;
    private Rigidbody rb;
    private SphereCollider col;

    private int health;
    private int counter;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        col.radius = 10;
        counter = 0;
        health = 100;
    }

    public void Update()
    {
        if(health!=0)
        {
            SendMessage("callUp");
        }
        else
        {
            SendMessage("deathReport");
        }
    }

    public void callUp(GameObject bot)
    {
        health = health -10; 
        Debug.Log("tempBot "+health);
    }
    public void deathReport()
    {
        Debug.Log("dead");
        Destroy(this.gameObject);
    }
}
 