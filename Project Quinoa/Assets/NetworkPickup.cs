using UnityEngine;
using System.Collections;

public class NetworkPickup : Photon.MonoBehaviour {
    public float updatePositionTime, updateRotationTime, updateVelocityTime;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Vector3 realVelocity = Vector3.zero;
    Rigidbody rb;

    void start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log(rb + "raoeu");
    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
           
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.Lerp(GetComponent<Rigidbody>().velocity, realVelocity, updateVelocityTime);
            GetComponent<Rigidbody>().position = Vector3.Lerp(transform.position, realPosition, updatePositionTime);
            GetComponent<Rigidbody>().rotation = Quaternion.Lerp(transform.rotation, realRotation, updateRotationTime);
        }
    }


    void Update()
    {
    //    if (photonView.isMine)
    //    {

    //    }
    //    else
    //    {
    //        transform.position = Vector3.Lerp(transform.position, realPosition, updatePositionTime);
    //        transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, updateRotationTime);
    //    }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
        }
        else
        {
            // Network player, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            realVelocity = (Vector3)stream.ReceiveNext();
        }
    }

}
