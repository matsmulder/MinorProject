using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private Animator animp;

    // Use this for initialization
    void Start()
    {
        animp = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w") || (Input.GetKey("a")) || (Input.GetKey("s")) || (Input.GetKey("d")))
           
        {
            animp.SetInteger("Animparam", 1);
        }
        else
        {
            animp.SetInteger("Animparam", 0);
        }
        
        if (Input.GetKey("space"))
        {
            animp.SetInteger("Jumpparam", 1);
        }
        else
        {
            animp.SetInteger("Jumpparam", 0);
        }
    }
}