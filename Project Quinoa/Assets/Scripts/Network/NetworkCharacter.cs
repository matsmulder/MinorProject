using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour
{
    public float updatePositionTime, updateRotationTime;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;


    void Update()
    {
        if(photonView.isMine)
        {
            //do nothing -- playerMovement is moving us
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, updatePositionTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, updateRotationTime);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}