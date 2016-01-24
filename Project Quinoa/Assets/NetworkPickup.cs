using UnityEngine;
using System.Collections;

public class NetworkPickup : Photon.MonoBehaviour {
    public float updatePositionTime, updateRotationTime, updateVelocityTime;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Vector3 realVelocity = Vector3.zero;
    private bool pushing;
    private Transform pickupKind;
    private bool debugPickupFlag = true, coroutineFlag;
    private Vector3 previousPosition;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousPosition = Vector3.zero;
        pushing = false;

        if(gameObject.transform.FindChild("pickupquinoalowpoly") == null)
        {
            pickupKind = gameObject.transform.FindChild("pickuphamburger");
        }
        else
        {
            pickupKind = gameObject.transform.FindChild("pickupquinoalowpoly");
        }
    }

    void FixedUpdate()
    {
        //if (rb.velocity == Vector3.zero) Debug.Log("noooo");
        //if(previousPosition == transform.position && pushing && rb.velocity != Vector3.zero) //out of sync
        //{
        //    Debug.Log("out of sync");
        //}

        //if (!debugPickupFlag)
        //{
        //    ReSyncPickup();
        //}
        //Debug.Log(pushing);
        pickupKind.gameObject.SetActive(true);
        //if (pushing)
        //{
        //    pickupKind.gameObject.SetActive(false);
        //}
        //else
        //{
        //    pickupKind.gameObject.SetActive(true);
        //}

        //if (photonView.isMine)
        if (photonView.isMine)
        {
            
        }
        else
        {
            if (!pushing)
            {
                GetComponent<Rigidbody>().velocity = Vector3.Lerp(GetComponent<Rigidbody>().velocity, realVelocity, updateVelocityTime);
                GetComponent<Rigidbody>().position = Vector3.Lerp(transform.position, realPosition, updatePositionTime);
                GetComponent<Rigidbody>().rotation = Quaternion.Lerp(transform.rotation, realRotation, updateRotationTime);
            }
            else
            {
                //Debug.Log("yeah something needs to be done here");
            }
        }

        previousPosition = transform.position;
    }

    //[PunRPC]
    //void ReSyncPickup()
    //{

    //}


    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            pushing = true;
            //Debug.Log(pushing);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            pushing = false;
            //Debug.Log(pushing);
        }
    }

    //IEnumerator DebugPickups()
    //{
    //    coroutineFlag = true;
    //    yield return new WaitForSeconds(1f);
    //    debugPickupFlag = false;
    //    coroutineFlag = false;
    //}




    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //debugPickupFlag = true;
        //if(coroutineFlag)
        //{
        //    coroutineFlag = false;
        //    StopCoroutine(DebugPickups());
        //}
        //StartCoroutine(DebugPickups());

        //Debug.Log("serialize");
        if (stream.isWriting)
        {
            if(pushing)
            {
                //Debug.Log("pushing, sending data...");
            }
            else
            {
                //Debug.Log("not pushing, sending data...");
            }
            // this pickup, send other players our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
        }
        else
        {
            if(pushing)
            {
                //Debug.Log("pushing, receiving data...");
            }
            else
            {
                //Debug.Log("not pushing, receiving data...");
            }
            // Network pickup, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realVelocity = (Vector3)stream.ReceiveNext();
        }
    }

}
