using UnityEngine;
//using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class tempBot2 : MonoBehaviour
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
        health = -100;
    }

    public void Update()
    {
        counter++;
        //SendMessage("callUp");
    }

    public void callUp()
    {
        Debug.Log("tempBot2 " + health);
    }
}
