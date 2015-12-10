using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class bulletStopper:MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
        
    }
}
