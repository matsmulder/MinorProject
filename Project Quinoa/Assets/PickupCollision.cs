using UnityEngine;
using System.Collections;

public class PickupCollision : MonoBehaviour {

    private scoreManager sm;
    private AudioSource audio1;
    public AudioClip pickupSound, lostPickupSound;

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
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
        }

        if(col.gameObject.CompareTag("trumpcollider")) //if the pickup is in team Trump's base
        {
            if (gameObject.tag == "superfood")
            {
                //quinoa in Trump base
                sm.CapturedPickups("superfood", true);
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
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
                AudioSource.PlayClipAtPoint(lostPickupSound, transform.position);
            }
        }

        if (col.gameObject.CompareTag("trumpcollider")) //if the pickup was in team Trump's base
        {
            if (gameObject.tag == "superfood")
            {
                //quinoa in Trump base
                sm.CapturedPickups("superfood", false);
                AudioSource.PlayClipAtPoint(lostPickupSound, transform.position);
            }

            if (gameObject.tag == "fastfood")
            {
                //pickup is in own base
            }
        }
    }

}
