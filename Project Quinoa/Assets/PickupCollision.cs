﻿using UnityEngine;
using System.Collections;

public class PickupCollision : MonoBehaviour {

    private scoreManager sm;

    void Start()
    {
        sm =  GameObject.FindObjectOfType<scoreManager>().GetComponent<scoreManager>();
        Debug.Log(sm);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("wholocollider")) //if the pickup is in team Wholo's base
        {
            if(gameObject.tag == "superfood")
            {
                //pickup is in own base
            }

            if (gameObject.tag == "fastfood")
            {
                //burger in Wholo base
                sm.CapturedPickups("fastfood", true);
            }
        }

        if(col.gameObject.CompareTag("trumpcollider")) //if the pickup is in team Trump's base
        {
            if (gameObject.tag == "superfood")
            {
                //quinoa in Trump base
                sm.CapturedPickups("superfood", true);
            }

            if (gameObject.tag == "fastfood")
            {
                //pickup is in own base
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("wholocollider")) //if the pickup was in team Wholo's base
        {
            if (gameObject.tag == "superfood")
            {
                //pickup is in own base
            }

            if (gameObject.tag == "fastfood")
            {
                //burger in Wholo base
                sm.CapturedPickups("fastfood", false);
            }
        }

        if (col.gameObject.CompareTag("trumpcollider")) //if the pickup was in team Trump's base
        {
            if (gameObject.tag == "superfood")
            {
                //quinoa in Trump base
                sm.CapturedPickups("superfood", false);
            }

            if (gameObject.tag == "fastfood")
            {
                //pickup is in own base
            }
        }
    }

}
