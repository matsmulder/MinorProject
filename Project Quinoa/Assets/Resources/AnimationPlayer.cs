using UnityEngine;
using System.Collections;

public class AnimationPlayer : MonoBehaviour {

    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKey("w"))
        {
            anim.SetInteger("Animparam", 1);
        } else
            {
                anim.SetInteger("Animparam", 0);
            }
    if (Input.GetKey("s"))
        {
            anim.SetInteger("Animparam", 1);
        }
        else
            {
                anim.SetInteger("Animparam", 0);
            }
    if (Input.GetKey("a"))
        {
            anim.SetInteger("Animparam", 1);
        }
        else
            {
                anim.SetInteger("Animparam", 0);
            }
    if (Input.GetKey("d"))
        {
            anim.SetInteger("Animparam", 1);
        }
        else
            {
                anim.SetInteger("Animparam", 0);
            }
  if (Input.GetKey("space"))
        {
          anim.SetInteger("Jumpparam", 1);
        }
        else
            {
                anim.SetInteger("Jumpparam", 0);
            }
	}
}
