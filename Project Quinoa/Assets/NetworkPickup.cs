using UnityEngine;
using System.Collections;

public class NetworkPickup : Photon.MonoBehaviour {
    public float updatePositionTime, updateRotationTime, updateVelocityTime;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Vector3 realVelocity = Vector3.zero;
    private bool pushing;
    private Transform pickupKind;

    void Start()
    {
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
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            pushing = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            pushing = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // this pickup, send other players our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
        }
        else
        {
            // Network pickup, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realVelocity = (Vector3)stream.ReceiveNext();
        }
    }

}
